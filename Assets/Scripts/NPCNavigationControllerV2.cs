using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

[Obsolete("Use this navigation controller until V3 is done.")]

/// <summary>
///     [What does this NPCNavigationControllerV2 do]
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
public sealed class NPCNavigationControllerV2 : MonoBehaviour
{
    public enum BehaviourMode
    {
        partol,
        focused
    }

    private System.Random random;

    private CharacterController characterController;
    private GravityController gravityController;

    public BehaviourMode behaviorMode = BehaviourMode.partol;

    #region Visibility Variables

    [Range(1.0f, 15.0f)]
    public float viewDistance = 5.0f;
    [Range(5.0f, 270.0f)]
    public float viewAngle = 45.0f;
    [Range(0.1f, 5.0f)]
    public float viewHeight = 1.0f;

    public LayerMask interestLayerMask;
    public LayerMask sightBlockLayerMask;
    [Range(1, 30)]
    public int scansPerSecond = 10;

    private readonly Collider[] inSightCollidersCache = new Collider[3];
    private int scansCount;
    private float scanInterval;
    private float scanTimer;
    private Mesh lineOfSightMesh;

    private GameObject inSightObject;
    public GameObject focusedObject;
    private bool followFocusedObject = false;
    [Range(10.0f, 25.0f)]
    public float forgetFocusedObjectRange = 17.0f;

    public bool showVisibilityGizmos = false;
    public Color focusAreaGizmoColor = Color.green;
    public Color unfocusAreaGizmoColor = Color.red;
    [Range(3, 50)]
    public int visionAreaSegments = 10;

    #endregion

    #region Destination Variables

    private Vector3 destination;
    public string GetDestinationString { get => destination.ToString(); }
    private bool reachedDestination;

    #endregion

    #region Target Variables

    //public Enemy targetEntity;

    #endregion

    #region Waypoint Variables

    public Waypoint currentWaypoint;
    [Range(0.0f, 0.99f)]
    public float chanceOfFlippingDirection = 0.25f;
    private bool isMovingClockwise = false;

    #endregion

    #region Navigation Variables

    [Range(0.0f, 1.0f)]
    public float stopDistance = 0.5f;
    [Range(1.0f, 20.0f)]
    public float rotationSpeed = 5.0f;
    [Range(1.0f, 5.0f)]
    public float movementSpeed = 3.0f;
    
    #endregion

    #region Jump Variables

    public Transform jumpCheck;
    public LayerMask jumpLayerMask;
    [Range(0.1f, 2.0f), Tooltip(StringRepo.Controllers.MaxJumpToolTip)]
    public float maxUnitsJump = 0.75f;
    private const float jumpDistance = 0.3f;
    
    #endregion

    private Vector3 velocity;
    public string GetVelocityString { get => velocity.ToString(); }

    private void OnValidate()
    {
        lineOfSightMesh = EditorTools.DrawWedgeMesh(viewAngle, viewDistance, viewHeight, visionAreaSegments);
        scanInterval = 1.0f / scansPerSecond;
    }

    private void Awake()
    {
        random = new System.Random();
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        gravityController = GetComponent<GravityController>();

        if (currentWaypoint != null)
        {
            SetDestination(currentWaypoint.GetPosition());
        }

        scanInterval = 1.0f / scansPerSecond;
    }

    private void Update()
    {
        ScanViewSight();
        
        UpdateNavigation();

        UpdateGravity();
    }

    private void FixedUpdate()
    {
        if (gravityController.GetIsGrounded)
        {
            if (Physics.CheckSphere(jumpCheck.position, jumpDistance, jumpLayerMask))
            {
                velocity.y = Mathf.Sqrt(maxUnitsJump * -2.0f * gravityController.gravity);
            }
        }

        if (followFocusedObject && focusedObject != null)
        {
            // Set destination to target position
            SetDestination(focusedObject.transform.position);

            // if target gets out of the unfocused area
            if (Physics.CheckSphere(transform.position, forgetFocusedObjectRange, interestLayerMask))
            {
                // forget the target
                focusedObject = null;
                followFocusedObject = false;

                // set state to partol
                behaviorMode = BehaviourMode.partol;
            }
        }
    }

    private void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }

    private void ScanViewSight()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;

            // scan area
            scansCount = Physics.OverlapSphereNonAlloc(transform.position, viewDistance, inSightCollidersCache, interestLayerMask, QueryTriggerInteraction.Collide);

            inSightObject = null;
            for (int i = 0; i < scansCount; i++)
            {
                GameObject obj = inSightCollidersCache[i].gameObject;
                if (obj != null && IsInSight(obj))
                {
                    inSightObject = obj;
                    focusedObject = inSightObject;
                    behaviorMode = BehaviourMode.focused;
                }
                else if (behaviorMode == BehaviourMode.focused)
                {
                    followFocusedObject = true;
                }
            }
        }
    }

    private bool IsInSight(GameObject obj)
    {
        //if (obj.TryGetComponent(out Entity interactable))
        //{
        //    interactable.int
        //}

        Vector3 origin = transform.position;
        Vector3 destination = obj.transform.position;
        Vector3 direction = destination - origin;
        if (direction.y < 0 || direction.y > viewHeight)
        {
            return false;
        }

        direction.y = 0.0f;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
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

    private void UpdateNavigation()
    {
        switch (behaviorMode)
        {
            case BehaviourMode.partol:
                {
                    if (transform.position != destination)
                    {
                        Vector3 destinationDirection = destination - transform.position;
                        destinationDirection.y = 0.0f;

                        float destinationDistance = destinationDirection.magnitude;

                        if (destinationDistance >= stopDistance)
                        {
                            reachedDestination = false;

                            Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);

                            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                            characterController.Move(movementSpeed * Time.deltaTime * transform.forward);
                        }
                        else
                        {
                            reachedDestination = true;
                        }
                    }

                    if (reachedDestination)
                    {
                        isMovingClockwise = random.NextDouble() <= chanceOfFlippingDirection;

                        currentWaypoint = isMovingClockwise ? currentWaypoint.previousWaypoint : currentWaypoint.nextWaypoint;

                        SetDestination(currentWaypoint.GetPosition());
                    }
                }
                break;
            case BehaviourMode.focused:
                {

                }
                break;
            default:
                break;
        }
    }

    private void UpdateGravity()
    {
        velocity.y = gravityController.UpdateGravity(velocity);

        characterController.Move(velocity * Time.deltaTime);
    }

    public void ClearStoredNavigationData()
    {
        int cacheCollidersCount = 0;
        for (int i = 0; i < inSightCollidersCache.Length; i++)
            if (inSightCollidersCache[i] != null)
                cacheCollidersCount++;
    
        bool hasObjectInSight = false;
        if (inSightObject != null)
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

    private void OnDrawGizmos()
    {
        if (jumpCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(jumpCheck.position, jumpDistance);
        }

        if (showVisibilityGizmos)
        {
            if (lineOfSightMesh != null)
            {
                Gizmos.color = focusAreaGizmoColor;
                Gizmos.DrawMesh(lineOfSightMesh, transform.position, transform.rotation);
            }
            
            Gizmos.DrawWireSphere(transform.position, viewDistance);

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
            if (inSightObject != null)
            {
                Vector3 inSightGizmoPos = new Vector3(inSightObject.transform.position.x,
                                                      inSightObject.transform.position.y + inSightObject.transform.localScale.y,
                                                      inSightObject.transform.position.z);
                Gizmos.DrawSphere(inSightGizmoPos, 0.2f);
            }

            Gizmos.color = unfocusAreaGizmoColor;
            Gizmos.DrawWireSphere(transform.position, forgetFocusedObjectRange);

            Gizmos.color = Color.blue;
            if (focusedObject != null)
            {
                if (Vector3.Distance(transform.position, focusedObject.transform.position) > viewDistance && Vector3.Distance(transform.position, focusedObject.transform.position) < forgetFocusedObjectRange)
                {
                    Vector3 inSightGizmoPos = new Vector3(focusedObject.transform.position.x,
                                                          focusedObject.transform.position.y + focusedObject.transform.localScale.y,
                                                          focusedObject.transform.position.z);
                    Gizmos.DrawSphere(inSightGizmoPos, 0.2f);
                }
            }
        }
    }
}
