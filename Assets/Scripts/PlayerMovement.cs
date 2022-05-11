using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Planets
{
    Moon,
    Mars,
    Venus,
    Earth,
    Jupiter,
    Custom
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    [Range(1.0f, 10.0f), Tooltip(StringRepo.PlayerMovement.WalkMultiplierToolTip)]
    public float walkMultiplier = 5.0f;
    [Range(1.0f, 2.0f), Tooltip(StringRepo.PlayerMovement.RunMultiplierToolTip)]
    public float runMultiplier = 1.25f;
    [Range(0.125f, 1.0f), Tooltip(StringRepo.PlayerMovement.CrouchMultiplierToolTip)]
    public float crouchMultiplier = 0.3f;

    [Range(groundDistance, 2.0f), Tooltip(StringRepo.PlayerMovement.MaxJumpToolTip)]
    public float maxUnitsJump = 0.75f;
    [Range(0.25f, 1.0f), Tooltip(StringRepo.PlayerMovement.ShortJumpMultiplierToolTip)]
    public float shortJumpMultiplier = 0.5f;
    [Range(0.25f, 2.0f), Tooltip(StringRepo.PlayerMovement.LongJumpChargeTime)]
    public float longJumpChargeTime = 0.5f;

    [Tooltip(StringRepo.PlayerMovement.PlanetToolTip)]
    public Planets planet = Planets.Earth;
    public float gravity = -9.81f;

    [Tooltip(StringRepo.PlayerMovement.GroundTransformToolTip)]
    public Transform groundCheck;
    [Tooltip(StringRepo.PlayerMovement.CollisionLayerMaskToolTip)]
    public LayerMask groundLayerMask;
    private const float groundDistance = 0.3f;

    [SerializeField]
    private bool disabled = false;
    public bool DisableMovement { set => disabled = value; }

    private float x, z;
    private Vector3 velocity;
    private Vector3 move;

    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isLongJumping = false;
    private float currentJumpForce;
    public float GetCurrentJumpForce { get => currentJumpForce; }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!disabled)
        {
            // inputs & speed calculations
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");

            float currentRunMultiplier = Input.GetButton("Run") ? runMultiplier : 1.0f;

            if (Input.GetButtonDown("Crouch") && isGrounded)
            {
                isCrouching = true;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                isCrouching = false;
            }
            float currentCrouchMultiplier = isCrouching ? crouchMultiplier : 1.0f;

            // jump
            if (isGrounded)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    isLongJumping = true;
                }

                if (isLongJumping && isCrouching)
                {
                    currentJumpForce += Time.deltaTime * longJumpChargeTime;
                }

                if (Input.GetButtonUp("Jump"))
                {
                    currentJumpForce = Mathf.Clamp(currentJumpForce, maxUnitsJump * shortJumpMultiplier, maxUnitsJump);
                    velocity.y = Mathf.Sqrt(currentJumpForce * -2.0f * gravity);

                    isLongJumping = false;
                    currentJumpForce = 0.0f;
                }
            }

            // movement
            move = currentCrouchMultiplier * currentRunMultiplier * walkMultiplier * (Vector3.Normalize(transform.right * x + transform.forward * z));
            controller.Move(move * Time.deltaTime);

            // gravity
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayerMask);
            if (isGrounded && velocity.y < 0.0f)
            {
                velocity.y = gravity;
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
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
