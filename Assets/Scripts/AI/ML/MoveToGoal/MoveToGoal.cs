using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public sealed class MoveToGoal : Agent
{
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private Color winColor = Color.green;
    [SerializeField]
    private Color loseColor = Color.red;

    [SerializeField]
    private MeshRenderer floorMeshRenderer;

    [SerializeField]
    private float moveSpeed = 5.0f;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3.5f, 3.5f), 0.0f, Random.Range(8.5f, -8.5f));
        targetTransform.localPosition = new Vector3(Random.Range(-14.0f, -6.0f), 0.0f, Random.Range(8.5f, -8.5f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;    
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continiousActions = actionsOut.ContinuousActions;

        continiousActions[0] = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
        continiousActions[1] = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetReward(+1.0f);

            floorMeshRenderer.material.color = winColor;

            EndEpisode();
        }
        
        if (other.CompareTag("Wall"))
        {
            SetReward(-1.0f);

            floorMeshRenderer.material.color = loseColor;

            EndEpisode();
        }
    }
}
