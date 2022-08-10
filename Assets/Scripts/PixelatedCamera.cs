using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using HelperNamespace;
using UnityEditor;

/// <summary>
///     [What does this Pixelated do]
/// </summary>
public sealed class PixelatedCamera : MonoBehaviour
{
    public enum PixelScreenMode { Resize, Scale }

    private static PixelatedCamera main;

    private Camera renderCamera;
    private RenderTexture renderTexture;
    private Vector2Int screensize = new();

    [Header("Screen scaling settings")]
    public PixelScreenMode mode;

    public Vector2Int targetScreenSize = new( 256, 144 );  // Only used with PixelScreenMode.Resize
    public uint screenScaleFactor = 1;  // Only used with PixelScreenMode.Scale

    private readonly RawImage display;

    private void Awake()
    {
        // Try to set as main pixel camera
        if (main == null) 
            main = this;
    }

    private void Start()
    {
        // Initialize the system
        Init();
    }

    private void Update()
    {
        // Re initialize system if the screen has been resized
        if (CheckScreenResize()) Init();
    }

    public void Init()
    {
        // Initialize the camera and get screen size values
        if (!renderCamera) renderCamera = GetComponent<Camera>();
        screensize.x = Screen.width;
        screensize.y = Screen.height;

        // Prevent any error
        if (screenScaleFactor < 1) screenScaleFactor = 1;
        if (targetScreenSize.x < 1) targetScreenSize.x = 1;
        if (targetScreenSize.y < 1) targetScreenSize.y = 1;

        // Calculate the render texture size
        int width = mode == PixelScreenMode.Resize ? (int)targetScreenSize.x : screensize.x / (int)screenScaleFactor;
        int height = mode == PixelScreenMode.Resize ? (int)targetScreenSize.y : screensize.y / (int)screenScaleFactor;

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

    public bool CheckScreenResize()
    {
        // Check whether the screen has been resized
        return Screen.width != screensize.x || Screen.height != screensize.y;
    }
}

[CustomEditor(typeof(PixelatedCamera)), CanEditMultipleObjects]
public sealed class PixelatedCameraInspector : Editor
{
    PixelatedCamera root;

    SerializedProperty mode;
    SerializedProperty targetScreenSize;
    SerializedProperty screenScaleFactor;

    private void OnEnable()
    {
        root = (PixelatedCamera)target;

        mode = serializedObject.FindProperty(nameof(root.mode));
        targetScreenSize = serializedObject.FindProperty(nameof(root.targetScreenSize));
        screenScaleFactor = serializedObject.FindProperty(nameof(root.screenScaleFactor));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(mode);

        switch (root.mode)
        {
            case PixelatedCamera.PixelScreenMode.Resize:
                {
                    EditorGUILayout.PropertyField(targetScreenSize);
                }
                break;
            case PixelatedCamera.PixelScreenMode.Scale:
                {
                    EditorGUILayout.PropertyField(screenScaleFactor);
                }
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
