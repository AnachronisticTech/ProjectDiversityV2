using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(POV))]

// TODO: PLayer does not move!

public sealed class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private CharacterController characterController;
    private GravityController gravityController;

    private StatsScriptableObject stats;

    //[Header(StringRepo.Controllers.NavigationLabel)]
    //
    //[Range(1.0f, 10.0f), Tooltip(StringRepo.Controllers.WalkMultiplierToolTip)]
    //public float walkMultiplier = 5.0f;
    //[Range(1.0f, 2.0f), Tooltip(StringRepo.Controllers.RunMultiplierToolTip)]
    //public float runMultiplier = 1.25f;
    private float currentRunMultiplier;

    //[Header(StringRepo.Controllers.JumpingLabel)]
    //
    //[Range(0.1f, 2.0f), Tooltip(StringRepo.Controllers.MaxJumpToolTip)]
    //public float maxUnitsJump = 0.75f;
    //[Range(0.25f, 1.0f), Tooltip(StringRepo.Controllers.ShortJumpMultiplierToolTip)]
    //public float shortJumpMultiplier = 0.5f;
    
    //[Header(StringRepo.Controllers.CrouchingLabel)]
    //
    //[Range(0.125f, 1.0f), Tooltip(StringRepo.Controllers.CrouchMultiplierToolTip)]
    //public float crouchMultiplier = 0.3f;
    private float currentCrouchMultiplier;
    //[Range(0.25f, 2.0f), Tooltip(StringRepo.Controllers.LongJumpChargeTime)]
    //public float longJumpChargeTime = 0.5f;

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

        stats = GetComponent<Stats>().stats;
        foreach (KeyValuePair<string, Stat> statEntry in stats.statsDict)
        {
            Debug.Log(this.name + " has " + statEntry.Value.GetName() + " as a stat.");
        }

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

        if (Input.GetButton("Run"))
        {
            if (stats.statsDict.TryGetValue("RunSpeed", out Stat runSpeed))
            {
                currentRunMultiplier = runSpeed.GetValue();
            }
            else
            {
                Debug.LogError("RunSpeed stat not found in " + this);
            }
        }
        else
        {
            currentRunMultiplier = 1.0f;
        }
        //currentRunMultiplier = Input.GetButton("Run") ? runMultiplier : 1.0f;

        if (Input.GetButtonDown("Crouch") && gravityController.GetIsGrounded)
        {
            isCrouching = true;

            if (stats.statsDict.TryGetValue("CrouchSpeed", out Stat crouchSpeed))
            {
                currentCrouchMultiplier = crouchSpeed.GetValue();
            }
            else
            {
                Debug.LogError("CrouchSpeed stat not found in " + this);
            }
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
                if (stats.statsDict.TryGetValue("MaxJumpChargeTime", out Stat longJumpChargeTime))
                {
                    currentJumpForce += Time.deltaTime * longJumpChargeTime.GetValue();
                }
                else
                {
                    Debug.LogError("MaxJumpChargeTime stat not found in " + this);
                }
                //currentJumpForce += Time.deltaTime * longJumpChargeTime;
            }

            if (Input.GetButtonUp("Jump"))
            {
                if (stats.statsDict.TryGetValue("MaxJumpUnits", out Stat maxJumpUnits) && stats.statsDict.TryGetValue("ShortJumpUnits", out Stat shortJumpUnits))
                {
                    currentJumpForce = Mathf.Clamp(currentJumpForce, maxJumpUnits.GetValue() * shortJumpUnits.GetValue(), maxJumpUnits.GetValue());
                }
                //currentJumpForce = Mathf.Clamp(currentJumpForce, maxUnitsJump * shortJumpMultiplier, maxUnitsJump);
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

        if (stats.statsDict.TryGetValue("WalkSpeed", out Stat walkSpeed))
        {
            move = currentCrouchMultiplier * currentRunMultiplier * walkSpeed.GetValue() * (Vector3.Normalize(transform.right * x + transform.forward * z));
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
