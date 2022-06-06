using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     [What does this GravityController do]
/// </summary>
public sealed class GravityController : MonoBehaviour
{
    [Tooltip(StringRepo.Physics.PlanetToolTip)]
    public float gravity = -9.81f;

    [Tooltip(StringRepo.Physics.GroundTransformToolTip)]
    public Transform groundCheck;
    [Tooltip(StringRepo.Physics.CollisionLayerMaskToolTip)]
    public LayerMask groundLayerMask;

    private const float groundDistance = 0.3f;

    private bool isGrounded = false;
    public bool GetIsGrounded { get => isGrounded; }

    public float UpdateGravity(Vector3 velocity)
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayerMask);

        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = gravity;
        }
        velocity.y += gravity * Time.deltaTime;

        return velocity.y;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
