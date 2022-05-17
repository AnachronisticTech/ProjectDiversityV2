using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this NPCNavigationControllerV2 do]
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
public sealed class NPCNavigationControllerV2 : MonoBehaviour
{
    private enum BehaviourMode
    {
        partol,
        focused
    }

    private System.Random random;

    private CharacterController characterController;
    private GravityController gravityController;

    [SerializeField]
    private BehaviourMode behaviorMode = BehaviourMode.partol;

    [Header(StringRepo.Physics.VisibilityLabel)]
    #region Visibility Variables

    [SerializeField, Range(1.0f, 10.0f)]
    private float viewDistance = 2.0f;
    [SerializeField, Range(5.0f, 180.0f)]
    private float viewAngle = 45.0f;
    [SerializeField, Range(0.1f, 5.0f)]
    private float viewHeight = 1.0f;
    [SerializeField]
    private Color lineOfSightGizmoColor = Color.red;

    [SerializeField, Range(1, 100)]
    private int scansPerSecond = 10;
    [SerializeField]
    private LayerMask interestLayerMask;
    [SerializeField]
    private LayerMask sightBlockLayerMask;

    private Collider[] inSightCollidersCache = new Collider[3];
    private int scansCount;
    private float scanInterval;
    private float scanTimer;
    private Mesh lineOfSightMesh;

    private GameObject inSightObject;
    
    #endregion

    [Header(StringRepo.Waypoint.WaypointLabel)]
    #region Waypoint Variables

    [SerializeField]
    private Waypoint currentWaypoint;
    private Vector3 destination;
    private bool reachedDestination;

    [SerializeField, Range(0.0f, 0.99f)]
    private float chanceOfFlippingDirection = 0.25f;
    private bool isMovingClockwise = false;

    #endregion

    [Header(StringRepo.Movement.NavigationLabel)]
    #region Navigation Variables

    [SerializeField, Range(0.0f, 1.0f)]
    private float stopDistance = 0.5f;
    [SerializeField, Range(1.0f, 20.0f)]
    private float rotationSpeed = 5.0f;
    [SerializeField, Range(1.0f, 5.0f)]
    private float movementSpeed = 3.0f;
    
    #endregion

    [Header(StringRepo.Movement.JumpingLabel)]
    #region Jump Variables

    [SerializeField]
    private Transform jumpCheck;
    [SerializeField]
    private LayerMask jumpLayerMask;
    [Range(0.1f, 2.0f), Tooltip(StringRepo.Movement.MaxJumpToolTip)]
    public float maxUnitsJump = 0.75f;
    private const float jumpDistance = 0.3f;
    
    #endregion

    private Vector3 velocity;

    private void OnValidate()
    {
        lineOfSightMesh = HelperNamespace.EditorTools.DrawWedgeMesh(viewAngle, viewDistance, viewHeight);
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
                }
            }
        }
    }

    private bool IsInSight(GameObject obj)
    {
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
            // TODO: rotates towards the waypoint but moves to wrong position.
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
                            characterController.Move(movementSpeed * Time.deltaTime * Vector3.forward);
                        }
                    }
                    else
                    {
                        reachedDestination = true;
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

    private void OnDrawGizmos()
    {
        if (jumpCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(jumpCheck.position, jumpDistance);
        }

        if (lineOfSightMesh != null)
        {
            Gizmos.color = lineOfSightGizmoColor;
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
    }
}
