/*
 * Script developed by Andreas Monoyios
 * GitHub: https://github.com/AMonoyios?tab=repositories
 */

using UnityEngine.UI;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
///     [What does this Pixelated do]
/// </summary>
public sealed class RetroEffectSettings : MonoBehaviour
{
    private Camera renderCamera;
    private RenderTexture renderTexture;

    [Header("Screen scaling settings")]
    [SerializeField]
    private Vector2Int targetScreenSize = new(256, 224);

    [SerializeField]
    private RawImage display;

    private void OnValidate()
    {
        targetScreenSize.Clamp(new(256, 224), new(640, 480));
    }

    private void Start()
    {
        // Initialize the system
        Init();
    }

    public void Init()
    {
        // Initialize the camera and get screen size values
        if (!renderCamera) renderCamera = GetComponent<Camera>();

        // Calculate the render texture size
        int width = targetScreenSize.x;
        int height = targetScreenSize.y;
        Debug.LogFormat("Screen resolution is: {0} x {1}", width, height);

        // Initialize the render texture
        renderTexture = new RenderTexture(width, height, 24)
        {
            filterMode = FilterMode.Point,
            antiAliasing = 1
        };

        // Set the render texture as the camera's output
        renderCamera.targetTexture = renderTexture;

        // Attaching texture to the display UI RawImage
        display.texture = renderTexture;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RetroEffectSettings)), CanEditMultipleObjects]
public sealed class RetroEffectSettingsInspector : Editor
{
    RetroEffectSettings root;

    private void OnEnable()
    {
        root = (RetroEffectSettings)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(5.0f);

        if (GUILayout.Button(new GUIContent("Update camera", "This is only available in edit mode")))
            root.Init();
    }
}
#endif