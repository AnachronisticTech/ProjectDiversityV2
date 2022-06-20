using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Update stats every time there is an update on the equipment that affects them rather than updating them on every click.

/// <summary>
///     [What does this PlayerController do]
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(StatsList))]
[RequireComponent(typeof(POV))]
public sealed class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
      
    private CharacterController characterController;
    private GravityController gravityController;
    
    public StatsList PlayerStats { get; private set; }
    
    [Header("Interact Data")]
    public Transform interactTransformPivot = null;
    [Range(0.5f, 2.5f)]
    public float interactRange = 1.0f;
    [HideInInspector]
    public GameObject target = null;
    
    private POV pov;
    private RaycastHit hit;

    [HideInInspector]
    public bool disabled = false;

    public Vector2 XZ { get; private set; }
    private Vector3 velocity = Vector3.zero;
    public Vector3 Move { get; private set; }

    private const float _defaultMultiplier = 1.0f;
    
    public bool IsCrouching { get; private set; }
    public bool IsLongJumping { get; private set; }
    
    private float currentRunMultiplier = _defaultMultiplier;
    private float currentCrouchMultiplier = _defaultMultiplier;
    private float currentJumpForce = _defaultMultiplier;
    private float _maxJumpChargeTime = _defaultMultiplier;

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

        PlayerStats = GetComponent<StatsList>();

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
        XZ = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // assign run multiplier while holding Run button
        float _runSpeed = PlayerStats.statsDict[StatRepo.RunSpeed].GetValue;
        currentRunMultiplier = Input.GetButton("Run") ? _runSpeed : _defaultMultiplier;

        // assign the crouch speed while holding Crouch button
        if (Input.GetButtonDown("Crouch") && gravityController.GetIsGrounded)
        {
            IsCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            IsCrouching = false;
        }
        currentCrouchMultiplier = IsCrouching ? PlayerStats.statsDict[StatRepo.CrouchSpeed].GetValue : _defaultMultiplier;

        // if player is grounded then he can jump
        if (gravityController.GetIsGrounded)
        {
            // when player initiates a jump then long jump calculations start
            if (Input.GetButtonDown("Jump"))
            {
                IsLongJumping = true;
                _maxJumpChargeTime = PlayerStats.statsDict[StatRepo.MaxJumpChargeTime].GetValue;
            }

            // adjusts the amount of long jump the player applies to jump 
            if (IsLongJumping && IsCrouching)
            {
                currentJumpForce += Time.deltaTime * _maxJumpChargeTime;
            }

            // when player stops jumping then all calculations for long jump are applied 
            if (Input.GetButtonUp("Jump"))
            {
                float _maxJumpUnits = PlayerStats.statsDict[StatRepo.MaxJumpUnits].GetValue;
                float _shortJumpUnits = PlayerStats.statsDict[StatRepo.ShortJumpUnits].GetValue;

                currentJumpForce = Mathf.Clamp(currentJumpForce, 
                                               _maxJumpUnits * _shortJumpUnits, 
                                               _maxJumpUnits);
                velocity.y = Mathf.Sqrt(currentJumpForce * -2.0f * gravityController.gravity); 

                // reset jump values/states
                IsLongJumping = false;
                currentJumpForce = 0.0f;
            }
        }
    }

    private void UpdateMovements()
    {
        float _walkSpeed = PlayerStats.statsDict[StatRepo.WalkSpeed].GetValue;

        Move = currentCrouchMultiplier * currentRunMultiplier * _walkSpeed * (Vector3.Normalize(transform.right * XZ.x + transform.forward * XZ.y));
        characterController.Move(Move * Time.deltaTime);
    }

    private void UpdateGravity()
    {
        velocity.y = gravityController.UpdateGravity(velocity);
        characterController.Move(velocity * Time.deltaTime);
    }

    public void Die()
    {
        Debug.Log(name + " has died.");

        gameObject.SetActive(false);
        //this.enabled = false;
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
