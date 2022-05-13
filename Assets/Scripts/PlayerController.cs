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
public sealed class PlayerController : CharacterActions
{
    public static PlayerController Instance { get; private set; }

    private CharacterController controller;

    [Range(1.0f, 10.0f), Tooltip(StringRepo.PlayerMovement.WalkMultiplierToolTip)]
    public float walkMultiplier = 5.0f;
    [Range(1.0f, 2.0f), Tooltip(StringRepo.PlayerMovement.RunMultiplierToolTip)]
    public float runMultiplier = 1.25f;
    private float currentRunMultiplier;
    [Range(0.125f, 1.0f), Tooltip(StringRepo.PlayerMovement.CrouchMultiplierToolTip)]
    public float crouchMultiplier = 0.3f;
    private float currentCrouchMultiplier;

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
    public bool DisableMovement { get => disabled;  set => disabled = value; }

    private float x, z;
    private Vector3 velocity;
    private Vector3 move;

    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isLongJumping = false;
    private float currentJumpForce;
    public float GetCurrentJumpForce { get => currentJumpForce; }

    private void Awake()
    {
        #region Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        #endregion
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!disabled)
        {
            UpdateInputs();

            UpdateMovements();

            UpdateGravity();
        }
    }

    private void UpdateInputs()
    {
        // inputs & speed calculations
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        currentRunMultiplier = Input.GetButton("Run") ? runMultiplier : 1.0f;

        if (Input.GetButtonDown("Crouch") && isGrounded)
        {
            isCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
        }
        currentCrouchMultiplier = isCrouching ? crouchMultiplier : 1.0f;

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
                velocity.y = CalculateJump(currentJumpForce, maxUnitsJump, shortJumpMultiplier, gravity);

                isLongJumping = false;
                currentJumpForce = 0.0f;
            }
        }
    }

    private void UpdateMovements()
    {
        // movement
        move = CalculateMove(x, z, currentCrouchMultiplier, currentRunMultiplier, walkMultiplier);
        controller.Move(move * Time.deltaTime);
    }

    private void UpdateGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayerMask);
        velocity.y = CalculateGravity(isGrounded, velocity, gravity);

        controller.Move(velocity * Time.deltaTime);
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
