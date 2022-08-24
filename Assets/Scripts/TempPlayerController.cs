/*
 * Script developed by Andreas Monoyios
 * GitHub: https://github.com/AMonoyios?tab=repositories
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Temporary code to move player to the world space
/// </summary>
[RequireComponent(typeof(CharacterController))]
public sealed class TempPlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5.0f;

    private CharacterController characterController;
    private Vector2 input;
    private Vector3 movement;

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        input = new(Input.GetAxis(horizontal), Input.GetAxis(vertical));
    }

    private void FixedUpdate()
    {
        /* Code curtesy of "about game making"
         * Source: https://www.youtube.com/watch?v=VslgzNfibhs&ab_channel=aboutgamemaking
         */
        movement = characterController.transform.forward * input.y;
        characterController.transform.Rotate((100.0f * Time.fixedDeltaTime) * input.x * Vector3.up);
        characterController.Move(moveSpeed * Time.fixedDeltaTime * movement);
    }
}
