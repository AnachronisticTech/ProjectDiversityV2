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

    [SerializeField]
    private float statUpdatesPerSecond = 2.0f;
    private float currentUpdateStatTick = 0.0f;
    
    private Stats playerStats;
    private float _health;
    private float _walkSpeed;
    private float _runSpeed;
    private float _crouchSpeed;
    private float _maxJumpChargeTime;
    private float _maxJumpUnits;
    private float _shortJumpUnits;

    private float currentRunMultiplier;
    private float currentCrouchMultiplier;

    #region temp
    /**
    [Header(StringRepo.Controllers.NavigationLabel)]
    
    [Range(1.0f, 10.0f), Tooltip(StringRepo.Controllers.WalkMultiplierToolTip)]
    public float walkMultiplier = 5.0f;
    [Range(1.0f, 2.0f), Tooltip(StringRepo.Controllers.RunMultiplierToolTip)]
    public float runMultiplier = 1.25f;
    [Header(StringRepo.Controllers.JumpingLabel)]
    
    [Range(0.1f, 2.0f), Tooltip(StringRepo.Controllers.MaxJumpToolTip)]
    public float maxUnitsJump = 0.75f;
    [Range(0.25f, 1.0f), Tooltip(StringRepo.Controllers.ShortJumpMultiplierToolTip)]
    public float shortJumpMultiplier = 0.5f;
    
    [Header(StringRepo.Controllers.CrouchingLabel)]
    
    [Range(0.125f, 1.0f), Tooltip(StringRepo.Controllers.CrouchMultiplierToolTip)]
    public float crouchMultiplier = 0.3f;
    [Range(0.25f, 2.0f), Tooltip(StringRepo.Controllers.LongJumpChargeTime)]
    public float longJumpChargeTime = 0.5f;
    */
    #endregion

    [Header("Interact Data")]
    [SerializeField, Range(0.5f, 2.5f)]
    private float interactRange = 1.0f;
    public GameObject target;

    private POV pov;
    private RaycastHit hit;

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

        playerStats = GetComponent<Stats>();

        pov = GetComponent<POV>();
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

        currentUpdateStatTick += Time.deltaTime;
        if (currentUpdateStatTick >= statUpdatesPerSecond)
        {
            UpdateStats();
        }
    }

    private void UpdateInputs()
    {
        #region Mouse Inputs

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

        #endregion

        #region Inputs & Speed inputs

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        //if (Input.GetButton("Run"))
        //{
        //    if (playerStats.GetStatsDict.TryGetValue("RunSpeed", out Stat runSpeed))
        //    {
        //        currentRunMultiplier = runSpeed.GetValue();
        //    }
        //    else
        //    {
        //        Debug.LogError("RunSpeed stat not found in " + this);
        //    }
        //}
        //else
        //{
        //    currentRunMultiplier = 1.0f;
        //}
        //currentRunMultiplier = Input.GetButton("Run") ? runMultiplier : 1.0f;
        currentRunMultiplier = Input.GetButton("Run") ? _runSpeed : 1.0f;

        if (Input.GetButtonDown("Crouch") && gravityController.GetIsGrounded)
        {
            isCrouching = true;

            //if (playerStats.GetStatsDict.TryGetValue("CrouchSpeed", out Stat crouchSpeed))
            //{
            //    currentCrouchMultiplier = crouchSpeed.GetValue();
            //}
            //else
            //{
            //    Debug.LogError("CrouchSpeed stat not found in " + this);
            //}
            currentCrouchMultiplier = _crouchSpeed;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;

            currentCrouchMultiplier = 1.0f;
        }
        //currentCrouchMultiplier = isCrouching ? crouchMultiplier : 1.0f;

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
                //if (playerStats.GetStatsDict.TryGetValue("MaxJumpChargeTime", out Stat longJumpChargeTime))
                //{
                //    currentJumpForce += Time.deltaTime * longJumpChargeTime.GetValue();
                //}
                //else
                //{
                //    Debug.LogError("MaxJumpChargeTime stat not found in " + this);
                //}
                //currentJumpForce += Time.deltaTime * longJumpChargeTime;
                currentJumpForce += Time.deltaTime * _maxJumpChargeTime;
            }

            if (Input.GetButtonUp("Jump"))
            {
                //if (playerStats.GetStatsDict.TryGetValue("MaxJumpUnits", out Stat maxJumpUnits) && playerStats.GetStatsDict.TryGetValue("ShortJumpUnits", out Stat shortJumpUnits))
                //{
                //    currentJumpForce = Mathf.Clamp(currentJumpForce, maxJumpUnits.GetValue() * shortJumpUnits.GetValue(), maxJumpUnits.GetValue());
                //}
                //else
                //{
                //    Debug.LogError("MaxJumpUnits stat not found in " + this);
                //}
                //currentJumpForce = Mathf.Clamp(currentJumpForce, maxUnitsJump * shortJumpMultiplier, maxUnitsJump);
                currentJumpForce = Mathf.Clamp(currentJumpForce, _maxJumpUnits * _shortJumpUnits, _maxJumpUnits);
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

        if (playerStats.GetStatsDict.TryGetValue("WalkSpeed", out Stat walkSpeed))
        {
            move = currentCrouchMultiplier * currentRunMultiplier * walkSpeed.GetValue() * (Vector3.Normalize(transform.right * x + transform.forward * z));
        }
        else
        {
            Debug.LogError("WalkSpeed stat not found in " + this);
        }
        //move = currentCrouchMultiplier * currentRunMultiplier * walkMultiplier * (Vector3.Normalize(transform.right * x + transform.forward * z));
        characterController.Move(move * Time.deltaTime);
        
        #endregion
    }

    private void UpdateGravity()
    {
        velocity.y = gravityController.UpdateGravity(velocity);

        characterController.Move(velocity * Time.deltaTime);
    }

    private void UpdateStats()
    {
        _health = playerStats.GetStatsDict["Health"].GetValue();
        _walkSpeed = playerStats.GetStatsDict["WalkSpeed"].GetValue();
        _runSpeed = playerStats.GetStatsDict["RunSpeed"].GetValue();
        _crouchSpeed = playerStats.GetStatsDict["CrouchSpeed"].GetValue();
        _maxJumpChargeTime = playerStats.GetStatsDict["MaxJumpChargeTime"].GetValue();
        _maxJumpUnits = playerStats.GetStatsDict["MaxJumpUnits"].GetValue();
        _shortJumpUnits = playerStats.GetStatsDict["ShortJumpUnits"].GetValue();
    }

    public void Die()
    {
        Debug.Log(name + " has died.");

        this.enabled = false;
        //Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
