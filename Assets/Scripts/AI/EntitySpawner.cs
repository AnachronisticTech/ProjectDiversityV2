using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EntitySpawner : MonoBehaviour
{
    // TODO: convert this to a custom editor in order to control the different object types
    [System.Serializable]
    private class SpawnPoints
    {
        [Header("Spawn point name")]
        public string name;

        [Header("Position references")]
        public Transform waypointParent;
        public Transform entityParent;

        [Header("Object settings")]
        public bool isNPC;

        [Header("Direction settings")]
        [ConditionalHide(nameof(isNPC), true)]
        public bool canFlipDirection;
        [ConditionalHide(nameof(canFlipDirection), true), Range(0.0f, 0.99f)]
        public float minChance;
        [ConditionalHide(nameof(canFlipDirection), true), Range(0.0f, 0.99f)]
        public float maxChance;

        [Header("Spawn settings")]
        [Range(1, 50)]
        public int spawnCount;
        public List<GameObject> characterPrefabs;

        public SpawnPoints()
        {
            name = "New spawn point";

            waypointParent = null;
            entityParent = null;

            isNPC = false;

            canFlipDirection = false;
            minChance = 0.0f;
            maxChance = 0.99f;

            spawnCount = 1;
            characterPrefabs = new List<GameObject>();
        }
    }

    [SerializeField]
    private List<SpawnPoints> spawnPoints = new List<SpawnPoints>() { new SpawnPoints() };

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            int spawned = 0;

            while (spawned < spawnPoints[i].spawnCount)
            {
                int selectedIndex = Random.Range(0, spawnPoints[i].characterPrefabs.Count);

                GameObject characterObject = Instantiate(spawnPoints[i].characterPrefabs[selectedIndex]);
                characterObject.transform.SetParent(spawnPoints[i].entityParent, false);

                Transform child = spawnPoints[i].waypointParent.GetChild(Random.Range(0, transform.childCount - 1));

                if (characterObject.TryGetComponent(out NPCNavigationController waypointNavigator))
                {
                    NPCNavigationController characterWaypointNavigator = waypointNavigator;
                    
                    characterWaypointNavigator.SetCurrentWaypoint = child.GetComponent<Waypoint>();

                    float chanceToFlipDirection = 0.0f;
                    if (spawnPoints[i].canFlipDirection)
                    {
                        chanceToFlipDirection = Random.Range(spawnPoints[i].minChance, spawnPoints[i].maxChance);
                    }
                    characterWaypointNavigator.SetChanceOfFlippingDirection = chanceToFlipDirection;
                }
                
                characterObject.transform.position = child.position;

                yield return new WaitForEndOfFrame();

                spawned++;
            }
        }
    }
}
