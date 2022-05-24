using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public sealed class MoveToGoal : Agent
{
    [Header("Observations")]
    [SerializeField]
    private Transform goalTransform;

    [Header("Settings")]
    [SerializeField, Range(50.0f, 250.0f)]
    private float moveForce = 150.0f;
    [SerializeField, Range(100.0f, 500.0f)]
    private float rotateForce = 250.0f;

    [Header("Visual Indications")]
    [SerializeField]
    private Color winColor = Color.green;
    [SerializeField]
    private Color loseColor = Color.red;
    [SerializeField]
    private MeshRenderer floorMeshRenderer;
    private Color defaultColor;
    
    // private cache variables
    private Rigidbody rb = null;
    private float rotateInput;
    private bool hasWon = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        defaultColor = floorMeshRenderer.material.color;
    }

    public override void OnEpisodeBegin()
    {
        if (hasWon)
        {
            StartCoroutine(IndicateEpisodeState(winColor));
            hasWon = false;
        }
        else
        {
            StartCoroutine(IndicateEpisodeState(loseColor));
        }

        transform.localPosition = new Vector3(Random.Range(-3.5f, 3.5f), 0.0f, Random.Range(8.5f, -8.5f));
        goalTransform.localPosition = new Vector3(Random.Range(-14.0f, -6.0f), 0.0f, Random.Range(8.5f, -8.5f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goalTransform.localPosition);
    }

    //
    //public override void OnActionReceived(ActionBuffers actions)
    //{
    //    float moveX = actions.ContinuousActions[0];
    //    float moveZ = actions.ContinuousActions[1];
    //
    //    transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed; 
    //}
    //
    //public override void Heuristic(in ActionBuffers actionsOut)
    //{
    //    ActionSegment<float> continiousActions = actionsOut.ContinuousActions;
    //
    //    continiousActions[0] = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
    //    continiousActions[1] = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;
    //}

    public override void OnActionReceived(ActionBuffers actions)
    {
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continiousActions = actionsOut.ContinuousActions;

        // TODO: finish this
        continiousActions[0] = Input.GetAxisRaw("Horizontal")
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = moveForce * Time.deltaTime * transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.velocity = -moveForce * Time.deltaTime * transform.forward;
        }
        else
        {
            rb.velocity = 0.95f * Time.deltaTime * rb.velocity;
        }

        if (Input.GetKey(KeyCode.A))
        {
            rotateInput -= rotateForce * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotateInput += rotateForce * Time.deltaTime;
        }
        rb.MoveRotation(Quaternion.identity * Quaternion.AngleAxis(rotateInput, transform.up));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasWon = true;
            SetReward(1.0f);
    
            EndEpisode();
        }
        
        if (other.CompareTag("Wall"))
        {
            SetReward(-1.0f);
    
            EndEpisode();
        }
    }

    private IEnumerator IndicateEpisodeState(Color color)
    {
        floorMeshRenderer.material.color = color;
    
        yield return new WaitForSeconds(3.0f);
    
        floorMeshRenderer.material.color = defaultColor;
    }

}
