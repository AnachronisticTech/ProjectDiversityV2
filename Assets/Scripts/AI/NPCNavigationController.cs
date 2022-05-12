using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class NPCNavigationController : MonoBehaviour
{
    private System.Random random;
    
    [Header("Character Movement")]

    [SerializeField, Range(0.0f, 1.0f)]
    private float stopDistance = 0.5f;
    [SerializeField, Range(1.0f, 20.0f)]
    private float rotationSpeed = 5.0f;
    [SerializeField, Range(1.0f, 5.0f)]
    private float movementSpeed = 3.0f;
    private float currentStoppingSpeed;

    [Header("Waypoint Navigation")]

    [SerializeField]
    private Waypoint currentWaypoint;
    public Waypoint SetCurrentWaypoint { set => currentWaypoint = value; }
    private Vector3 destination;
    private bool reachedDestination;

    [SerializeField, Range(0.0f, 0.99f)]
    private float chanceOfFlippingDirection = 0.25f;
    public float SetChanceOfFlippingDirection { set => chanceOfFlippingDirection = value; }
    private bool isMovingClockwise = false;

    [SerializeField]
    private bool hasWaitBeforeNextWaypoint = false;
    [SerializeField, ConditionalHide(nameof(hasWaitBeforeNextWaypoint), true), Range(0.0f, 0.99f)]
    private float chanceToWait = 0.25f;
    [SerializeField, ConditionalHide(nameof(hasWaitBeforeNextWaypoint), true), Range(0.0f, 20.0f)]
    private float minWaitTime = 0.0f;
    [SerializeField, ConditionalHide(nameof(hasWaitBeforeNextWaypoint), true), Range(0.0f, 20.0f)]
    private float maxWaitTime = 5.0f;

    private bool availableToWait = false;
    private bool isWaiting = false;

    private void Awake()
    {
        random = new System.Random();
    }

    private void Start()
    {
        if (currentWaypoint != null)
            SetDestination(currentWaypoint.GetPosition());

        if (hasWaitBeforeNextWaypoint && minWaitTime >= maxWaitTime)
        {
            Debug.LogWarning("Min wait time is greater than max wait time, please assign the timings right.");
            minWaitTime = maxWaitTime - 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != destination)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopDistance)
            {
                reachedDestination = false;

                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);

                if (!isWaiting)
                {
                    if (hasWaitBeforeNextWaypoint && availableToWait)
                    {
                        if (random.NextDouble() <= chanceToWait)
                        {
                            StartCoroutine(Wait());
                        }

                        availableToWait = false;
                    }

                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.Translate(movementSpeed * Time.deltaTime * Vector3.forward);

                    currentStoppingSpeed = movementSpeed;
                }
                else
                {
                    currentStoppingSpeed = Mathf.Clamp(currentStoppingSpeed -= Time.deltaTime * 5.0f, 0.0f, movementSpeed);
                    transform.Translate(currentStoppingSpeed * Time.deltaTime * Vector3.forward);
                }
            }
            else
            {
                reachedDestination = true;
                availableToWait = true;
            }
        }

        if (reachedDestination)
        {
            isMovingClockwise = random.NextDouble() <= chanceOfFlippingDirection;

            currentWaypoint = isMovingClockwise ? currentWaypoint.previousWaypoint : currentWaypoint.nextWaypoint;

            SetDestination(currentWaypoint.GetPosition());
        }
    }

    private void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }

    private IEnumerator Wait()
    {
        isWaiting = true;

        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        isWaiting = false;
    }
}
