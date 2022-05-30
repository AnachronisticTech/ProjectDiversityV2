using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(POV))]

public sealed class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private CharacterController characterController;
    private GravityController gravityController;
    
    [HideInInspector, Range(0.5f, 10.0f)]
    public float secondsPerStatsUpdate = 2.0f;
    private float currentStatUpdateTimer = 0.0f;
    private Stats playerStats;
    private float _health;
    private float _walkSpeed;
    private float _runSpeed;
    private float _crouchSpeed;
    private float _maxJumpChargeTime;
    private float _maxJumpUnits;
    private float _shortJumpUnits;

    [Header("Interact Data")]
    public Transform interactTransformPivot;
    [Range(0.5f, 2.5f)]
    public float interactRange = 1.0f;
    [HideInInspector]
    public GameObject target;

    private POV pov;
    private RaycastHit hit;

    private bool disabled = false;
    public bool DisableMovement { get => disabled;  set => disabled = value; }

    private Vector2 xz;
    public Vector2 GetXZ { get => xz; }
    private Vector3 velocity;
    private Vector3 move;
    public Vector3 GetMove { get => move; }
    private float currentRunMultiplier;
    private float currentCrouchMultiplier;
    private bool isCrouching = false;
    public bool GetIfCrouching { get => isCrouching; }
    private bool isLongJumping = false;
    public bool GetIfIsLongJumping { get => isLongJumping; }
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

        playerStats = GetComponent<Stats>();

        pov = GetComponent<POV>();

        if (interactTransformPivot == null)
            interactTransformPivot = transform;
    }

    private void FixedUpdate()
    {
        target = pov.focusedObject;
    }

    private void Update()
    {
        if (!disabled)
        {
            UpdateInputs();
            UpdateMovements();
            UpdateGravity();
        }

        currentStatUpdateTimer += Time.deltaTime;
        if (currentStatUpdateTimer >= secondsPerStatsUpdate)
        {
            UpdateStats();
            currentStatUpdateTimer = 0.0f;
        }
    }

    private void UpdateInputs()
    {
        // interact click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, interactRange))
            {
                if (hit.transform.gameObject == target)
                {
                    Debug.Log("Clicked on: " + target.name);
                }
            }
        }

        // direction inputs
        xz.x = Input.GetAxis("Horizontal");
        xz.y = Input.GetAxis("Vertical");

        // assign run multiplier while holding Run button
        currentRunMultiplier = Input.GetButton("Run") ? _runSpeed : 1.0f;

        // assign the crouch speed while holding Crouch button
        if (Input.GetButtonDown("Crouch") && gravityController.GetIsGrounded)
        {
            isCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
        }
        currentCrouchMultiplier = GetIfCrouching ? _crouchSpeed : 1.0f;

        // if player is grounded then he can jump
        if (gravityController.GetIsGrounded)
        {
            // when player initiates a jump then long jump calculations start
            if (Input.GetButtonDown("Jump"))
            {
                isLongJumping = true;
            }

            // adjusts the amount of long jump the player applies to jump 
            if (GetIfIsLongJumping && GetIfCrouching)
            {
                currentJumpForce += Time.deltaTime * _maxJumpChargeTime;
            }

            // when player stops jumping then all calculations for long jump are applied 
            if (Input.GetButtonUp("Jump"))
            {
                currentJumpForce = Mathf.Clamp(currentJumpForce, _maxJumpUnits * _shortJumpUnits, _maxJumpUnits);
                velocity.y = Mathf.Sqrt(currentJumpForce * -2.0f * gravityController.gravity); 

                // reset jump values/states
                isLongJumping = false;
                currentJumpForce = 0.0f;
            }
        }
    }

    private void UpdateMovements()
    {
        move = currentCrouchMultiplier * currentRunMultiplier * _walkSpeed * (Vector3.Normalize(transform.right * GetXZ.x + transform.forward * GetXZ.y));
        characterController.Move(GetMove * Time.deltaTime);
    }

    private void UpdateGravity()
    {
        velocity.y = gravityController.UpdateGravity(velocity);
        characterController.Move(velocity * Time.deltaTime);
    }

    private void UpdateStats()
    {
        Debug.Log("Updating stats");

        _health = playerStats.statsDict[StatRepo.Stats.Health].GetValue();
        _walkSpeed = playerStats.statsDict[StatRepo.Stats.WalkSpeed].GetValue();
        _runSpeed = playerStats.statsDict[StatRepo.Stats.RunSpeed].GetValue();
        _crouchSpeed = playerStats.statsDict[StatRepo.Stats.CrouchSpeed].GetValue();
        _maxJumpChargeTime = playerStats.statsDict[StatRepo.Stats.MaxJumpChargeTime].GetValue();
        _maxJumpUnits = playerStats.statsDict[StatRepo.Stats.MaxJumpUnits].GetValue();
        _shortJumpUnits = playerStats.statsDict[StatRepo.Stats.ShortJumpUnits].GetValue();
    }

    public void Die()
    {
        Debug.Log(name + " has died.");

        this.enabled = false;
        //Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (interactTransformPivot != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactTransformPivot.position, interactRange);
        }
    }
}
