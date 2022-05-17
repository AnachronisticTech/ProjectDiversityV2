using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
public sealed class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private CharacterController characterController;
    private GravityController gravityController;

    [Header(StringRepo.Movement.NavigationLabel)]

    [Range(1.0f, 10.0f), Tooltip(StringRepo.Movement.WalkMultiplierToolTip)]
    public float walkMultiplier = 5.0f;
    [Range(1.0f, 2.0f), Tooltip(StringRepo.Movement.RunMultiplierToolTip)]
    public float runMultiplier = 1.25f;
    private float currentRunMultiplier;

    [Header(StringRepo.Movement.JumpingLabel)]

    [Range(0.1f, 2.0f), Tooltip(StringRepo.Movement.MaxJumpToolTip)]
    public float maxUnitsJump = 0.75f;
    [Range(0.25f, 1.0f), Tooltip(StringRepo.Movement.ShortJumpMultiplierToolTip)]
    public float shortJumpMultiplier = 0.5f;
    
    [Header(StringRepo.Movement.CrouchingLabel)]

    [Range(0.125f, 1.0f), Tooltip(StringRepo.Movement.CrouchMultiplierToolTip)]
    public float crouchMultiplier = 0.3f;
    private float currentCrouchMultiplier;
    [Range(0.25f, 2.0f), Tooltip(StringRepo.Movement.LongJumpChargeTime)]
    public float longJumpChargeTime = 0.5f;

    private bool disabled = false;
    public bool DisableMovement { get => disabled;  set => disabled = value; }

    private float x, z;
    private Vector3 velocity;
    private Vector3 move;

    private bool isCrouching = false;
    private bool isLongJumping = false;
    private float currentJumpForce;

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
        characterController = GetComponent<CharacterController>();
        gravityController = GetComponent<GravityController>();
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
        #region Inputs & Speed inputs

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        currentRunMultiplier = Input.GetButton("Run") ? runMultiplier : 1.0f;

        if (Input.GetButtonDown("Crouch") && gravityController.GetIsGrounded)
        {
            isCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
        }
        currentCrouchMultiplier = isCrouching ? crouchMultiplier : 1.0f;

        #endregion

        #region Jump inputs

        if (gravityController.GetIsGrounded)
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
                velocity.y = Mathf.Sqrt(currentJumpForce * -2.0f * gravityController.gravity); 

                isLongJumping = false;
                currentJumpForce = 0.0f;
            }
        }

        #endregion
    }

    private void UpdateMovements()
    {
        #region movement calculation

        move = currentCrouchMultiplier * currentRunMultiplier * walkMultiplier * (Vector3.Normalize(transform.right * x + transform.forward * z));
        characterController.Move(move * Time.deltaTime);
        
        #endregion
    }

    private void UpdateGravity()
    {
        velocity.y = gravityController.UpdateGravity(velocity);

        characterController.Move(velocity * Time.deltaTime);
    }
}
