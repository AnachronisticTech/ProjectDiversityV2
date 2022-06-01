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
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(POV))]
public sealed class EnemyController : MonoBehaviour
{
    public Stats EnemyStats { get; private set; }

    [Header("Interact Data")]
    public Transform interactTranformPivot;
    [Range(0.5f, 2.5f)]
    public float interactRange = 1.0f;
    [HideInInspector]
    public GameObject target = null;
    [HideInInspector]
    public GameObject cachedTarget = null;
    [HideInInspector]
    public PlayerController targetController = null;

    [HideInInspector]
    public float currentAttackTimer = 0.0f;
    [HideInInspector, Range(0.0f,100.0f)]
    public float attackTypeChance = 0.0f;

    private POV pov;

    private void Start()
    {
        EnemyStats = GetComponent<Stats>();

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

            if (target != null)
            {
                if (target.TryGetComponent(out PlayerController playerController))
                    targetController = playerController;
            }
            else
            {
                targetController = null;
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
        if (currentAttackTimer >= EnemyStats.statsDict[StatRepo.AttackSpeed].GetValue())
        {
            float amount;
            attackTypeChance = Random.Range(0.0f, 100.0f);
            if (attackTypeChance >= (EnemyStats.statsDict[StatRepo.BigAttackChance].GetMaxValue() - EnemyStats.statsDict[StatRepo.BigAttackChance].GetValue()))
                amount = EnemyStats.statsDict[StatRepo.BigAttack].GetValue();
            else if (attackTypeChance >= (EnemyStats.statsDict[StatRepo.MediumAttackChance].GetMaxValue() - EnemyStats.statsDict[StatRepo.MediumAttackChance].GetValue()))
                amount = EnemyStats.statsDict[StatRepo.MediumAttack].GetValue();
            else
                amount = EnemyStats.statsDict[StatRepo.SmallAttack].GetValue();

            Debug.Log(name + " dealed " + amount + " damage to " + target + " with current health of " + targetController.PlayerStats.statsDict[StatRepo.Health].GetValue());

            if (targetController.PlayerStats.statsDict[StatRepo.Health].DecreaseValue(amount))
            {
                targetController.Die();
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
