using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    /// <summary>
    ///     Applies jump force to character.
    /// </summary>
    /// <returns>Returns a float that corrisponds to velocity Y axis.</returns>
    public float CalculateJump(float currentJumpForce, float maxUnitsJump, float shortJumpMultiplier, float gravity)
    {
        currentJumpForce = Mathf.Clamp(currentJumpForce, maxUnitsJump * shortJumpMultiplier, maxUnitsJump);

        return Mathf.Sqrt(currentJumpForce * -2.0f * gravity); ;
    }

    /// <summary>
    ///     Applies movement force to character.
    /// </summary>
    /// <returns>Returns a Vector3, use this to move CharacterController.</returns>
    public Vector3 CalculateMove(float xMoveInput, float zMoveInput, float currentCrouchMultiplier, float currentRunMultiplier, float walkMultiplier)
    {
        return currentCrouchMultiplier * currentRunMultiplier * walkMultiplier * (Vector3.Normalize(transform.right * xMoveInput + transform.forward * zMoveInput)); ;
    }

    /// <summary>
    ///     Applies gravity forces to the character.
    /// </summary>
    /// <returns>Returns a float that corrisponds to the velocity Y axis.</returns>
    public float CalculateGravity(bool isGrounded, Vector3 velocity, float gravity)
    {
        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = gravity;
        }
        velocity.y += gravity * Time.deltaTime;

        return velocity.y;
    }
}
