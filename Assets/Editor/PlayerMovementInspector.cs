using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController)), CanEditMultipleObjects]
public class PlayerMovementInspector : Editor
{
    PlayerController root;

    private SerializedProperty walkMultiplier;
    private SerializedProperty runMultiplier;
    private SerializedProperty crouchMultiplier;

    private SerializedProperty maxUnitsJump;
    private SerializedProperty shortJumpMultiplier;
    private SerializedProperty longJumpChargeTime;

    private SerializedProperty planet;
    private SerializedProperty gravity;

    private SerializedProperty groundCheck;
    private SerializedProperty groundLayerMask;

    private const float sectionSpacing = 10.0f;

    private void OnEnable()
    {
        root = (PlayerController)target;

        walkMultiplier = serializedObject.FindProperty(nameof(root.walkMultiplier));
        runMultiplier = serializedObject.FindProperty(nameof(root.runMultiplier));
        crouchMultiplier = serializedObject.FindProperty(nameof(root.crouchMultiplier));
    
        maxUnitsJump = serializedObject.FindProperty(nameof(root.maxUnitsJump));
        shortJumpMultiplier = serializedObject.FindProperty(nameof(root.shortJumpMultiplier));
        longJumpChargeTime = serializedObject.FindProperty(nameof(root.longJumpChargeTime));
    
        planet = serializedObject.FindProperty(nameof(root.planet));
        gravity = serializedObject.FindProperty(nameof(root.gravity));
    
        groundCheck = serializedObject.FindProperty(nameof(root.groundCheck));
        groundLayerMask = serializedObject.FindProperty(nameof(root.groundLayerMask));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField(StringRepo.PlayerMovement.NavigationLabel, EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(walkMultiplier);
        EditorGUILayout.PropertyField(runMultiplier);
        EditorGUILayout.PropertyField(crouchMultiplier);

        GUILayout.Space(sectionSpacing);

        EditorGUILayout.LabelField(StringRepo.PlayerMovement.JumpingLabel, EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(maxUnitsJump);
        GUI.enabled = false;
        EditorGUILayout.LabelField(root.GetCurrentJumpForce.ToString(), new GUIStyle(GUI.skin.textField), GUILayout.Width(50.0f));
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(shortJumpMultiplier);
        EditorGUILayout.PropertyField(longJumpChargeTime);

        GUILayout.Space(sectionSpacing);

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(planet);
        float chosenGravity = 0.0f;
        switch (root.planet)
        {
            case Planets.Moon:
                chosenGravity = -1.62f;
                break;
            case Planets.Mars:
                chosenGravity = -3.7f;
                break;
            case Planets.Venus:
                chosenGravity = -8.87f;
                break;
            case Planets.Earth:
                chosenGravity = -9.81f;
                break;
            case Planets.Jupiter:
                chosenGravity = -24.5f;
                break;
        }
        if (root.planet == Planets.Custom)
        {
            EditorGUILayout.PropertyField(gravity, GUIContent.none, GUILayout.Width(50.0f));
        }
        else
        {
            GUI.enabled = false;
            root.gravity = chosenGravity;
            EditorGUILayout.LabelField(chosenGravity.ToString(), new GUIStyle(GUI.skin.textField), GUILayout.Width(50.0f));
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField(new GUIContent(StringRepo.PlayerMovement.CollisionLabel), EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(groundCheck);
        EditorGUILayout.PropertyField(groundLayerMask);

        serializedObject.ApplyModifiedProperties();
    }
}
