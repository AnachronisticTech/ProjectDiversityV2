using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this EntitySpawnerV2 do]
/// </summary>
public sealed class EntitySpawnerV2 : MonoBehaviour, ISerializationCallbackReceiver
{
    [System.Serializable]
    private class SpawnPoints
    {
        [Header("Spawn point name")]
        public string name = "New spawn point";

        [Header("Position references")]
        public Transform waypointParent = null;
        public Transform entityParent = null;

        public enum EntityType
        {
            Passive,
            Enemy,
        }
        [Header("Object settings")]
        public EntityType type = EntityType.Passive;

        [Header("Direction settings")]
        public bool canFlipDirection = false;
        [ConditionalHide(nameof(canFlipDirection), true), Range(0.0f, 1.0f)]
        public float minChance = 0.0f;
        [ConditionalHide(nameof(canFlipDirection), true), Range(0.0f, 1.0f)]
        public float maxChance = 1.0f;

        [Header("Spawn settings")]
        [Range(1, 50)]
        public int spawnCount = 1;
        public List<GameObject> characterPrefabs = new();

        [Header("Show Debug data")]
        public bool showDebugData = false;
        [ConditionalHide(nameof(showDebugData), true)]
        public bool isInitialized = false;
    }

    [SerializeField]
    private SpawnPoints[] spawnPoints;

#if UNITY_EDITOR
    /// <summary>
    ///     These methods are called from unity before and after it starts to serialize the new values given.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Because the class that is used in the list does not inherit from MonoBehaviour unity doesn't
    ///         know what the default values are not tries to get them. By doing this method we can manually
    ///         call the constructor of the class before the serialization so unity can see them on the
    ///         serialization phase.
    ///     </para>
    ///     <para>
    ///         Note that these methods execute more times that you think from Unity! Use them with caution to avoid
    ///         serializing data more times that you want/need!
    ///     </para>
    /// </remarks>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.html"/>
    public void OnBeforeSerialize()
    {
        if (spawnPoints == null)
            return;
    
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!spawnPoints[i].isInitialized)
            {
                Debug.Log("Marking new instance of class as initialized.");
    
                spawnPoints[i].isInitialized = true;
            }
        }
    }
    public void OnAfterDeserialize()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!spawnPoints[i].isInitialized)
            {
                Debug.Log("Passing default values to new SpawnPoint element.");
                
                spawnPoints[i] = new();
            }
        }
    }
#endif

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
