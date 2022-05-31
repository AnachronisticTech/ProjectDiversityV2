using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     [What does this Enemy do]
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(POV))]
public sealed class EnemyController : MonoBehaviour, IStats
{
    [Header("Interact Data")]
    public Transform interactTranformPivot;
    [Range(0.5f, 2.5f)]
    public float interactRange = 1.0f;
    [HideInInspector]
    public GameObject target;

    [HideInInspector, Range(0.5f, 10.0f)]
    public float secondsPerStatsUpdate = 2.0f;
    private float currentStatUpdateTimer = 0.0f;
    public Stats enemyStats;
    #region Stats
    private float _health;
    private float _walkSpeed;
    private float _runSpeed;
    private float _maxJumpUnits;
    private float _attackSpeed;
    private float _smallAttack;
    #endregion

    [HideInInspector]
    public float currentAttackTimer = 0.0f;
    private POV pov;

    private void Start()
    {
        enemyStats = GetComponent<Stats>();

        pov = GetComponent<POV>();

        if (interactTranformPivot == null)
            interactTranformPivot = transform;
    }

    private void FixedUpdate()
    {
        target = pov.focusedObject;
    }

    private void Update()
    {
        if (pov.focusedObject == null)
        {
            Idle();
        }
        else 
        {
            float distance = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);
            if (distance <= interactRange)
            {
                Attack(_smallAttack);
            }
        }

        currentStatUpdateTimer += Time.deltaTime;
        if (currentStatUpdateTimer >= secondsPerStatsUpdate)
        {
            UpdateStats();
            currentStatUpdateTimer = 0.0f;
        }
    }

    public void Idle()
    {
        // cycle between this and patrolling
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Enemy " + this.name + " took " + amount + " damage.");
        if (enemyStats.statsDict[StatRepo.Health].DecreaseValue(amount))
        {
            Die();
        }
    }

    public void Attack(float amount)
    {
        currentAttackTimer += Time.deltaTime;
        if (currentAttackTimer >= enemyStats.statsDict[StatRepo.AttackSpeed].GetValue())
        {
            Debug.Log("fighting... Dealed: " + amount + " damage to -> " + target);

            if (target.GetComponent<PlayerController>().playerStats.statsDict[StatRepo.Health].DecreaseValue(amount))
            {
                PlayerController.Instance.Die();
            }
        
            currentAttackTimer = 0;
        }
    }

    public void UpdateStats()
    {
        Debug.Log("Updating " + name + " stats");

        _health = enemyStats.statsDict[StatRepo.Health].GetValue();
        _walkSpeed = enemyStats.statsDict[StatRepo.WalkSpeed].GetValue();
        _runSpeed = enemyStats.statsDict[StatRepo.RunSpeed].GetValue();
        _maxJumpUnits = enemyStats.statsDict[StatRepo.MaxJumpUnits].GetValue();
        _attackSpeed = enemyStats.statsDict[StatRepo.AttackSpeed].GetValue();
        _smallAttack = enemyStats.statsDict[StatRepo.SmallAttack].GetValue();
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
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}
