using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

// TODO: Update stats every time there is an update on the equipment that affects them rather than updating them on every click.

/// <summary>
///     [What does this Enemy do]
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(StatsList))]
[RequireComponent(typeof(POV))]
public sealed class EnemyController : MonoBehaviour
{
    public StatsList EnemyStats { get; private set; }

    [Header("Interact Data")]
    public Transform interactTranformPivot;
    [Range(0.5f, 2.5f)]
    public float interactRange = 1.0f;
    [HideInInspector]
    public GameObject target = null;
    private GameObject cachedTarget = null;

    [HideInInspector]
    public PlayerController playerController = null;
    [HideInInspector]
    public EnemyController enemyController = null;

    [HideInInspector]
    public float currentAttackTimer = 0.0f;
    [HideInInspector, Range(0.0f,100.0f)]
    public float attackTypeChance = 0.0f;

    private POV pov;

    private void Start()
    {
        EnemyStats = GetComponent<StatsList>();

        pov = GetComponent<POV>();

        if (interactTranformPivot == null)
            interactTranformPivot = transform;
    }

    private void FixedUpdate()
    {
        target = pov.focusedObject;
        if (target != cachedTarget)
        {
            cachedTarget = target;

            playerController = null;
            enemyController = null;
            
            if (target != null)
            {
                Debug.Log("Fetching controller for Target: " + target);

                if (target.TryGetComponent(out PlayerController playerController))
                    this.playerController = playerController;
                if (target.TryGetComponent(out EnemyController enemyController))
                    this.enemyController = enemyController;
            }
        }
    }

    private void Update()
    {
        if (target == null)
        {
            Idle();
        }
        else 
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance <= interactRange)
            {
                Attack();
            }
        }
    }

    public void Idle()
    {
        // cycle between this and patrolling
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Enemy " + this.name + " took " + amount + " damage.");
        if (EnemyStats.statsDict[StatRepo.Health].DecreaseValue(amount))
        {
            Die();
        }
    }

    public void Attack()
    {
        currentAttackTimer += Time.deltaTime;
        if (currentAttackTimer >= EnemyStats.statsDict[StatRepo.AttackSpeed].GetValue)
        {
            float amount;
            attackTypeChance = Random.Range(0.0f, 100.0f);
            if (attackTypeChance >= (EnemyStats.statsDict[StatRepo.BigAttackChance].GetMaxValue - EnemyStats.statsDict[StatRepo.BigAttackChance].GetValue))
                amount = EnemyStats.statsDict[StatRepo.BigAttack].GetValue;
            else if (attackTypeChance >= (EnemyStats.statsDict[StatRepo.MediumAttackChance].GetMaxValue - EnemyStats.statsDict[StatRepo.MediumAttackChance].GetValue))
                amount = EnemyStats.statsDict[StatRepo.MediumAttack].GetValue;
            else
                amount = EnemyStats.statsDict[StatRepo.SmallAttack].GetValue;

            if (playerController != null && enemyController == null)
            {
                Debug.Log(name + " dealed " + amount + " damage to " + target + " with current health of " + playerController.PlayerStats.statsDict[StatRepo.Health].GetValue);

                if (playerController.PlayerStats.statsDict[StatRepo.Health].DecreaseValue(amount))
                    playerController.Die();
            }
            else if (playerController == null && enemyController != null)
            {
                Debug.Log(name + " dealed " + amount + " damage to " + target + " with current health of " + enemyController.EnemyStats.statsDict[StatRepo.Health].GetValue);

                if (enemyController.EnemyStats.statsDict[StatRepo.Health].DecreaseValue(amount))
                    enemyController.Die();
            }
            else
            {
                Debug.LogWarning(name + " failed to deal damage to " + target + ", target does not have a valid controller!");
            }
            
            currentAttackTimer = 0.0f;
        }
    }

    public void Die()
    {
        Debug.Log(name + " has died.");

        PlayerController.Instance.target = null;

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (interactTranformPivot != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactTranformPivot.position, interactRange);
        }
    }
}
