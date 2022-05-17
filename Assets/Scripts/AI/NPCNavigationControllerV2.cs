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

    [SerializeField, Range(1.0f, 10.0f)]
    private float viewDistance = 2.0f;
    [SerializeField, Range(15.0f, 270.0f)]
    private float viewAngle = 45.0f;
    [SerializeField, Range(1, 20)]
    private int rays = 5;
    [SerializeField]
    private Vector3 offset = Vector3.one;

    [Header(StringRepo.Waypoint.WaypointLabel)]

    [SerializeField]
    private Waypoint currentWaypoint;
    private Vector3 destination;
    private bool reachedDestination;

    [SerializeField, Range(0.0f, 0.99f)]
    private float chanceOfFlippingDirection = 0.25f;
    private bool isMovingClockwise = false;

    [Header(StringRepo.Movement.NavigationLabel)]

    [SerializeField, Range(0.0f, 1.0f)]
    private float stopDistance = 0.5f;
    [SerializeField, Range(1.0f, 20.0f)]
    private float rotationSpeed = 5.0f;
    [SerializeField, Range(1.0f, 5.0f)]
    private float movementSpeed = 3.0f;

    [Header(StringRepo.Movement.JumpingLabel)]

    [SerializeField]
    private Transform jumpCheck;
    [SerializeField]
    private LayerMask jumpLayerMask;
    [Range(0.1f, 2.0f), Tooltip(StringRepo.Movement.MaxJumpToolTip)]
    public float maxUnitsJump = 0.75f;
    private const float jumpDistance = 0.3f;

    // ----------

    private Vector3 velocity;

    private void Awake()
    {
        random = new System.Random();
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        gravityController = GetComponent<GravityController>();

        if (currentWaypoint != null)
            SetDestination(currentWaypoint.GetPosition());
    }

    private void Update()
    {
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

        Gizmos.color = Color.red;
        float delta = viewAngle / (float)rays;
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2.0f, transform.position.z);
        for (int i = 0; i <= rays; i++)
        {
            Vector3 direction = Quaternion.Euler(0, i * delta, 0) * transform.right * -1.0f + offset;
            Gizmos.DrawRay(startPos, direction * viewDistance);
        }
    }
}
