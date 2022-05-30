using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this POV do]
/// </summary>
public sealed class POV : MonoBehaviour
{
    [SerializeField]
    private Transform pivot;
    [SerializeField, Range(1.0f, 15.0f)]
    private float viewDistance = 5.0f;
    public float GetViewDistance { get => viewDistance; }
    [Range(5.0f, 25.0f)]
    public float forgetFocusedObjectRange = 17.0f;
    [SerializeField, Range(5.0f, 270.0f)]
    private float viewAngle = 45.0f;
    [SerializeField, Range(0.1f, 5.0f)] 
    private float viewHeight = 1.0f;
    [SerializeField]
    private LayerMask interestLayerMask;
    [SerializeField]
    private LayerMask sightBlockLayerMask;

    private readonly Collider[] inSightCollidersCache = new Collider[3];
    private int scansCount;
    private float scanInterval;
    private float scanTimer;
    private Mesh lineOfSightMesh;

    [HideInInspector, Range(0.05f, 10.0f)]
    public float scansPerSeconds = 0.5f;
    [HideInInspector]
    public List<GameObject> inSightObjects = new List<GameObject>();
    [HideInInspector]
    public GameObject focusedObject;
    [HideInInspector]
    public bool showVisibilityGizmos = false;
    [HideInInspector]
    public Color focusAreaGizmoColor = Color.green;
    [HideInInspector]
    public Color unfocusAreaGizmoColor = Color.red;
    [HideInInspector, Range(3, 30)]
    public int visionAreaSegments = 10;

    private void OnValidate()
    {
        lineOfSightMesh = EditorTools.DrawWedgeMesh(viewAngle, viewDistance, viewHeight, visionAreaSegments);
        scanInterval = 1.0f / scansPerSeconds;
    }

    private void Start()
    {
        if (pivot == null)
            pivot = transform;

        scanInterval = 1.0f / scansPerSeconds;
    }

    private void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;

            // scan area
            scansCount = Physics.OverlapSphereNonAlloc(pivot.position, viewDistance, inSightCollidersCache, interestLayerMask, QueryTriggerInteraction.Collide);

            inSightObjects.Clear();
            for (int i = 0; i < scansCount; i++)
            {
                GameObject obj = inSightCollidersCache[i].gameObject;
                if (obj != null && obj != gameObject && IsInSight(obj))
                {
                    inSightObjects.Add(obj);
                }
            }
            focusedObject = SortingTools.GetNearestGameObject(pivot, inSightObjects);
        }
    }

    private bool IsInSight(GameObject obj)
    {
        Vector3 origin = pivot.position;
        Vector3 destination = obj.transform.position;
        Vector3 direction = destination - origin;
        if (direction.y < 0 || direction.y > viewHeight)
        {
            return false;
        }

        direction.y = 0.0f;
        float deltaAngle = Vector3.Angle(direction, pivot.forward);
        if (deltaAngle > (viewAngle / 2.0f))
        {
            return false;
        }

        origin.y += viewHeight / 2.0f;
        destination.y = origin.y;
        if (Physics.Linecast(origin, destination, sightBlockLayerMask))
        {
            return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (showVisibilityGizmos && pivot != null)
        {
            if (lineOfSightMesh != null)
            {
                Gizmos.color = focusAreaGizmoColor;
                Gizmos.DrawMesh(lineOfSightMesh, pivot.position, pivot.rotation);
            }

            Gizmos.DrawWireSphere(pivot.position, viewDistance);

            Gizmos.color = Color.red;
            for (int i = 0; i < scansCount; i++)
            {
                if (inSightCollidersCache[i] != null)
                {
                    Vector3 inRangeGizmoPos = new Vector3(inSightCollidersCache[i].transform.position.x,
                                                          inSightCollidersCache[i].transform.position.y + inSightCollidersCache[i].transform.localScale.y,
                                                          inSightCollidersCache[i].transform.position.z);
                    Gizmos.DrawSphere(inRangeGizmoPos, 0.2f);
                }
            }

            Gizmos.color = Color.green;
            for (int i = 0; i < inSightObjects.Count; i++)
            {
                Vector3 inSightGizmoPos = new Vector3(inSightObjects[i].transform.position.x,
                                                      inSightObjects[i].transform.position.y + inSightObjects[i].transform.localScale.y,
                                                      inSightObjects[i].transform.position.z);
                Gizmos.DrawSphere(inSightGizmoPos, 0.2f);
            }

            Gizmos.color = unfocusAreaGizmoColor;
            Gizmos.DrawWireSphere(pivot.position, forgetFocusedObjectRange);

            Gizmos.color = Color.blue;
            if (focusedObject != null)
            {
                if (Vector3.Distance(pivot.position, focusedObject.transform.position) > viewDistance && Vector3.Distance(pivot.position, focusedObject.transform.position) < forgetFocusedObjectRange)
                {
                    Vector3 inSightGizmoPos = new Vector3(focusedObject.transform.position.x,
                                                          focusedObject.transform.position.y + focusedObject.transform.localScale.y,
                                                          focusedObject.transform.position.z);
                    Gizmos.DrawSphere(inSightGizmoPos, 0.2f);
                }
            }
        }
    }

    public void ClearStoredNavigationData()
    {
        int cacheCollidersCount = 0;
        for (int i = 0; i < inSightCollidersCache.Length; i++)
            if (inSightCollidersCache[i] != null)
                cacheCollidersCount++;

        bool hasObjectInSight = false;
        if (inSightObjects.Count >= 1)
            hasObjectInSight = true;

        if (cacheCollidersCount > 0 && hasObjectInSight)
            Debug.Log("Cleared " + cacheCollidersCount + " in sight cache colliders and cleared focused in sight object.");
        else if (cacheCollidersCount > 0 && !hasObjectInSight)
            Debug.Log("Cleared " + cacheCollidersCount + " in sight cache colliders.");
        else if (cacheCollidersCount < 0 && hasObjectInSight)
            Debug.Log("Cleared focused in sight object");
        else
            Debug.Log("No objects found to clear.");
    }
}
