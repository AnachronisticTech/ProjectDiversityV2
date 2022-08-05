using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using HelperNamespace;

/// <summary>
///     [What does this PW_Grid do]
/// </summary>
public sealed class PW_GridGenerator : MonoBehaviour
{
    [Header("Grid setup")]
    public Vector2Int size = new(100, 100);

    [SerializeField]
    private Color unknownColor;
    [SerializeField, Range(0.0f, 1.0f)]
    private float waterLevel = 0.3f;
    [SerializeField]
    private Color waterColor;
    [SerializeField, Range(0.0f, 1.0f)]
    private float sandLevel = 0.35f;
    [SerializeField]
    private Color sandColor;
    [SerializeField, Range(0.0f, 1.0f)]
    private float greeneryLevel = 0.8f;
    [SerializeField]
    private Color greeneryColor;
    [SerializeField, Range(0.0f, 1.0f)]
    private float snowLevel = 0.9f;
    [SerializeField]
    private Color snowColor;

    [SerializeField, Range(0.01f, 0.5f)]
    private float perlinScale = 0.255f;

    private float[,] noiseMap;
    private float[,] falloffMap;

    [SerializeField]
    private Material terrainMaterial;

    private PW_CellInfo[,] grid;

    private Vector3 totalGenerateAreaPosition;
    private Vector3 totalGenerateAreaSize;

    [Header("Debugging")]

    [SerializeField]
    private Vector2Int sizeBounds = new(1, 500);
    public Vector2Int GetSizeBounds { get { return sizeBounds; } }

    [SerializeField]
    private bool showCalculateTime = true;
    private System.Diagnostics.Stopwatch landNoiseCalculateTime;
    private System.Diagnostics.Stopwatch falloffCalculateTime;
    private System.Diagnostics.Stopwatch generateLandTime;
    private System.Diagnostics.Stopwatch drawTerrainMeshTime;
    private System.Diagnostics.Stopwatch drawTerrainTextureTime;
    private long totalGenerateTime = 0;

    private void Start()
    {
        CreateWorld();
    }

    private void CreateWorld()
    {
        CalculateNoiseMap();
        CalculateBaseFalloutMap();
        GenerateBase();
        DrawTerrainMesh();
        DrawTerrainTexture();

        if (showCalculateTime)
            ShowGenerateTime();
    }

    public void RegenerateWorld()
    {
        HelperNamespace.Console.ClearLog();

        DestroyImmediate(gameObject.GetComponent<MeshFilter>());
        DestroyImmediate(gameObject.GetComponent<MeshRenderer>());

        CreateWorld();
    }

    private void CalculateNoiseMap()
    {
        if (showCalculateTime)
            landNoiseCalculateTime = System.Diagnostics.Stopwatch.StartNew();

        noiseMap = new float[size.x, size.y];

        float perlinXOffset = UnityEngine.Random.Range(-10000f, 10000f);
        float perlinYOffset = UnityEngine.Random.Range(-10000f, 10000f);

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                float noiseValue = Mathf.PerlinNoise(x * perlinScale + perlinXOffset, y * perlinScale + perlinYOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        if (showCalculateTime)
            landNoiseCalculateTime.Stop();
    }

    private void CalculateBaseFalloutMap()
    {
        if (showCalculateTime)
            falloffCalculateTime = System.Diagnostics.Stopwatch.StartNew();

        falloffMap = new float[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                float xv = x / (float)size.x * 2 - 1;
                float yv = y / (float)size.y * 2 - 1;

                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));

                falloffMap[x, y] = Mathf.Pow(v, 3.0f) / (Mathf.Pow(v, 3.0f) + Mathf.Pow(2.2f - 2.2f * v, 3.0f));
            }
        }

        if (showCalculateTime)
            falloffCalculateTime.Stop();
    }

    private void GenerateBase()
    {
        if (showCalculateTime)
            generateLandTime = System.Diagnostics.Stopwatch.StartNew();
        
        grid = new PW_CellInfo[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y];

                PW_CellInfo.Type cellType = PW_CellInfo.Type.Unknown;

                if (noiseValue <= waterLevel)
                {
                    cellType = PW_CellInfo.Type.Water;
                }
                else if (noiseValue <= sandLevel)
                {
                    cellType = PW_CellInfo.Type.Sand;
                }
                else if (noiseValue <= greeneryLevel)
                {
                    cellType = PW_CellInfo.Type.Greenery;
                }
                else if (noiseValue <= snowLevel)
                {
                    cellType = PW_CellInfo.Type.Snow;
                }

                PW_CellInfo cell = new();
                cell.CellType = cellType;

                grid[x, y] = cell;
            }
        }

        if (showCalculateTime)
            generateLandTime.Stop();
    }

    private void DrawTerrainMesh()
    {
        if (showCalculateTime)
            drawTerrainMeshTime = System.Diagnostics.Stopwatch.StartNew();

        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                //PW_CellInfo cell = grid[x, y];
                //
                //if (!cell.IsWater)
                //{
                    Vector3 a = new(x - 0.5f, 0, y + 0.5f);
                    Vector3 b = new(x + 0.5f, 0, y + 0.5f);
                    Vector3 c = new(x - 0.5f, 0, y - 0.5f);
                    Vector3 d = new(x + 0.5f, 0, y - 0.5f);

                    Vector2 uvA = new(x / (float)size.x, y / (float)size.y);
                    Vector2 uvB = new((x + 1) / (float)size.x, y / (float)size.y);
                    Vector2 uvC = new(x / (float)size.x, (y + 1) / (float)size.y);
                    Vector2 uvD = new((x + 1) / (float)size.x, (y + 1) / (float)size.y);

                    Vector3[] v = new[] { a, b, c, b, d, c };
                    Vector2[] uv = new[] { uvA, uvB, uvC, uvB, uvD, uvC };

                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[k]);
                    }
                //}
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Debug.Log(mesh.vertices.Length);

        if (showCalculateTime)
            drawTerrainMeshTime.Stop();
    }

    private void DrawTerrainTexture()
    {
        if (showCalculateTime)
            drawTerrainTextureTime = System.Diagnostics.Stopwatch.StartNew();

        Texture2D texture = new(size.x, size.y);
        Color[] colorMap = new Color[size.x * size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                PW_CellInfo cell = grid[x, y];

                switch (cell.CellType)
                {
                    case PW_CellInfo.Type.Unknown:
                        colorMap[y * size.x + x] = unknownColor;
                        break;
                    case PW_CellInfo.Type.Water:
                        colorMap[y * size.x + x] = waterColor;
                        break;
                    case PW_CellInfo.Type.Sand:
                        colorMap[y * size.x + x] = sandColor;
                        break;
                    case PW_CellInfo.Type.Greenery:
                        colorMap[y * size.x + x] = greeneryColor;
                        break;
                    case PW_CellInfo.Type.Snow:
                        colorMap[y * size.x + x] = snowColor;
                        break;
                    default:
                        break;
                }
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = terrainMaterial;
        meshRenderer.material.mainTexture = texture;

        if (showCalculateTime)
            drawTerrainTextureTime.Stop();
    }

    private void ShowGenerateTime()
    {
        Debug.LogFormat("Base Perlin calculated in {0}ms", landNoiseCalculateTime.ElapsedMilliseconds);
        Debug.LogFormat("Base fallout calculated in {0}ms", falloffCalculateTime.ElapsedMilliseconds);
        Debug.LogFormat("Base generated in {0}ms", generateLandTime.ElapsedMilliseconds);
        Debug.LogFormat("Terrain mesh drawed in {0}ms", drawTerrainMeshTime.ElapsedMilliseconds);
        Debug.LogFormat("Terrain mesh drawed in {0}ms", drawTerrainTextureTime.ElapsedMilliseconds);

        totalGenerateTime = landNoiseCalculateTime.ElapsedMilliseconds + 
                            falloffCalculateTime.ElapsedMilliseconds + 
                            generateLandTime.ElapsedMilliseconds + 
                            drawTerrainMeshTime.ElapsedMilliseconds + 
                            drawTerrainTextureTime.ElapsedMilliseconds;
        Debug.LogFormat("Total generated in {0}ms", totalGenerateTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        totalGenerateAreaPosition = new(transform.position.x + (size.x / 2.0f), transform.position.y + 0.5f, transform.position.z + (size.y / 2.0f));
        totalGenerateAreaSize = new(size.x, 1.0f, size.y);

        Gizmos.DrawWireCube(totalGenerateAreaPosition, totalGenerateAreaSize);
    }
}

[CustomEditor(typeof(PW_GridGenerator)), CanEditMultipleObjects]
public sealed class PW_GridGeneratorEditor : Editor
{
    PW_GridGenerator root;

    private void OnEnable()
    {
        root = (PW_GridGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        int clampedSizeX = Values.EnsureNumericInRange(root.size.x, root.GetSizeBounds.x, root.GetSizeBounds.y);
        int clampedSizeY = Values.EnsureNumericInRange(root.size.y, root.GetSizeBounds.x, root.GetSizeBounds.y);
        root.size = new(clampedSizeX, clampedSizeY);
        
        //if (root.size.x < 1)
        //    root.size.x = 1;
        //if (root.size.y < 1)
        //    root.size.y = 1;

        base.OnInspectorGUI();

        EditorTools.Line();

        GUI.enabled = Application.isPlaying;
        if(GUILayout.Button(new GUIContent("Regenerate world", "This is only available in play mode")))
            root.RegenerateWorld();
    }
}
