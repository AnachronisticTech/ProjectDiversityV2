/*
 * Script developed by Andreas Monoyios
 * GitHub: https://github.com/AMonoyios?tab=repositories
 * 
 * Reference: Sebastian Lague
 * Source: https://www.youtube.com/watch?v=73Dc5JTCmKI&ab_channel=SebastianLague
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     [What does this Vision do]
/// </summary>
public sealed class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private float viewRadius;
    public float GetViewRadius { get { return viewRadius; } }
    [SerializeField, Range(0.0f, 360.0f)]
    private float viewAngle;
    public float GetViewAngle { get { return viewAngle; } }

    [SerializeField]
    private LayerMask interactLayerMask;
    [SerializeField]
    private LayerMask obstacleLayerMask;

    [SerializeField]
    private List<GameObject> interactableObjectsInRange = new();
    public List<Transform> GetTransformsOfInteractablesInViewRange() 
    {
        List<Transform> result = new();

        foreach (GameObject interactable in interactableObjectsInRange)
            result.Add(interactable.transform);

        return result;
    }

    [SerializeField, Range(0.0f, 1.0f)]
    private float fieldOfViewMeshResolution = 0.5f;
    [SerializeField]
    private MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    [SerializeField, Range(0, 10)]
    private int edgeDetectionPrecision = 5;

    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal = false)
    {
        if (!angleIsGlobal)
            angleDegrees += transform.eulerAngles.y;
        
        return new(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

    private void Start()
    {
        viewMesh = new()
        {
            name = "ViewMesh"
        };
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine(FindInterctablesWithDelay(0.2f));
    }

    private void Update()
    {
        DrawFieldOfViewMesh();
    }

    private IEnumerator FindInterctablesWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindInteractableObjectsInView();
        }
    }
    private void FindInteractableObjectsInView()
    {
        interactableObjectsInRange.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, interactLayerMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2.0f)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleLayerMask))
                {
                    interactableObjectsInRange.Add(target.gameObject);
                }
            }
        }
    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = DirFromAngle(globalAngle, true);

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, viewRadius, obstacleLayerMask))
            return new(true, hit.point, hit.distance, globalAngle);
        else
            return new(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
    }
    private void DrawFieldOfViewMesh()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * (fieldOfViewMeshResolution / 2.0f));
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new();

        ViewCastInfo oldViewCast = new();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;

            ViewCastInfo newViewCast = ViewCast(angle);
            
            if (i > 0)
            {
                if (oldViewCast.hit != newViewCast.hit)
                {
                    //FindEdge(oldViewCast, newViewCast);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count - 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
    //private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    //{
    //    float minAngle = minViewCast.angle;
    //    float maxAngle = maxViewCast.angle;
    //
    //    Vector3 minPoint = minViewCast.point;
    //    Vector3 maxPoint = maxViewCast.point;
    //
    //    for (int i = 0; i < edgeDetectionPrecision; i++)
    //    {
    //        float angle = (minAngle + maxAngle) / 2.0f;
    //
    //        ViewCastInfo newViewCast = ViewCast(angle);
    //
    //        if (newViewCast.hit == minViewCast.hit)
    //        {
    //            minAngle = angle;
    //            minPoint = newViewCast.point;
    //        }
    //        else
    //        {
    //            maxAngle = angle;
    //            maxPoint = newViewCast.point;
    //        }
    //    }
    //}

    private struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }
    private struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
