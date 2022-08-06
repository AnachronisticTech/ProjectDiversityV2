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
    [Header("Size and Scale")]
    [SerializeField]
    private bool useSeed = false;
    [ConditionalHide(nameof(useSeed), true)]
    public short seed = 0;
    public Vector2Int size = new(100, 100);
    [SerializeField, Range(0.01f, 0.5f)]
    private float perlinScale = 0.255f;

    private float[,] noiseMap;
    private float[,] falloffMap;

    [Header("Material")]
    [SerializeField]
    private Material terrainMaterial;
    [SerializeField]
    private FilterMode terrainMaterialFilterMode = FilterMode.Point;

    [Header("Levels and Colors")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float waterLevel = 0.3f;
    [SerializeField]
    private Gradient waterGradientColor;

    [SerializeField, Range(0.0f, 1.0f)]
    private float sandLevel = 0.35f;
    [SerializeField]
    private Gradient sandGradientColor;
    
    [SerializeField, Range(0.0f, 1.0f)]
    private float greeneryLevel = 0.8f;
    [SerializeField]
    private Gradient greeneryGradientColor;
    
    [SerializeField, Range(0.0f, 1.0f)]
    private float snowLevel = 0.9f;
    [SerializeField]
    private Gradient rockySnowGradientColor;
    [SerializeField]
    private Gradient snowGradientColor;

    private PW_CellInfo[,] grid;

    [Space(height: 10.0f)]
    [Header("Debugging")]
    [SerializeField]
    private Color unknownColor;
    private Vector2Int sizeBounds = new(1, 500);
    public Vector2Int GetSizeBounds { get { return sizeBounds; } }

    private Vector3 totalGenerateAreaPosition;
    private Vector3 totalGenerateAreaSize;

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

        if (!useSeed)
            seed = (short)Random.Range(short.MinValue, short.MaxValue);

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                float noiseValue = Mathf.PerlinNoise(x * perlinScale + seed, y * perlinScale + seed);
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

                PW_CellInfo.Type cellType = noiseValue <= waterLevel    ? PW_CellInfo.Type.Water    :
                                            noiseValue <= sandLevel     ? PW_CellInfo.Type.Sand     :
                                            noiseValue <= greeneryLevel ? PW_CellInfo.Type.Greenery :
                                            noiseValue <= snowLevel     ? PW_CellInfo.Type.RockySnow:
                                                                          PW_CellInfo.Type.Snow     ;

                PW_CellInfo cell = new(cellType);

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

        MeshRenderer _ = gameObject.AddComponent<MeshRenderer>();

        if (showCalculateTime)
            drawTerrainMeshTime.Stop();
    }

    private void DrawTerrainTexture()
    {
        if (showCalculateTime)
            drawTerrainTextureTime = System.Diagnostics.Stopwatch.StartNew();

        Texture2D texture = new(size.x, size.y);
        Color32[] colorMap = new Color32[size.x * size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                PW_CellInfo cell = grid[x, y];

                switch (cell.GetCellType)
                {
                    case PW_CellInfo.Type.Unknown:
                        colorMap[y * size.x + x] = unknownColor;
                        break;
                    case PW_CellInfo.Type.Water:
                        colorMap[y * size.x + x] = waterGradientColor.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
                        break;
                    case PW_CellInfo.Type.Sand:
                        colorMap[y * size.x + x] = sandGradientColor.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
                        break;
                    case PW_CellInfo.Type.Greenery:
                        colorMap[y * size.x + x] = greeneryGradientColor.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
                        break;
                    case PW_CellInfo.Type.RockySnow:
                        colorMap[y * size.x + x] = rockySnowGradientColor.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
                        break;
                    case PW_CellInfo.Type.Snow:
                        colorMap[y * size.x + x] = snowGradientColor.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
                        break;
                    default:
                        Debug.LogErrorFormat("Failed to color mesh on grid[{0},{1}]", x, y);
                        break;
                }
            }
        }
        texture.filterMode = terrainMaterialFilterMode;
        texture.SetPixels32(colorMap);
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

    private bool firstSeedCollected = false;
    private float seed;

    private void OnEnable()
    {
        root = (PW_GridGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        //root.seed = Mathf.Clamp(root.seed, -10000.0f, 10000.0f);

        int clampedSizeX = Values.EnsureNumericInRange(root.size.x, root.GetSizeBounds.x, root.GetSizeBounds.y);
        int clampedSizeY = Values.EnsureNumericInRange(root.size.y, root.GetSizeBounds.x, root.GetSizeBounds.y);
        root.size = new(clampedSizeX, clampedSizeY);

        base.OnInspectorGUI();

        EditorTools.Line();

        if (!firstSeedCollected)
        {
            seed = root.seed;
            firstSeedCollected = Application.isPlaying;
        }

        GUI.enabled = Application.isPlaying;
        if(GUILayout.Button(new GUIContent("Regenerate world", "This is only available in play mode")))
        {
            root.RegenerateWorld();
            seed = root.seed;
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.FloatField(new GUIContent("Grid Seed"), seed);
        GUI.enabled = Application.isPlaying;
        GUILayout.EndHorizontal();
    }
}
