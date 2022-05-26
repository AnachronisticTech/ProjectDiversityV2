using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this Enemy do]
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(POV))]
public sealed class EnemyController : MonoBehaviour
{
    private StatsScriptableObject stats;

    [SerializeField, Range(0.5f, 2.5f)]
    private float interactRange = 1.0f;

    private float currentAttackTimer = 0.0f;
    private POV pov;

    private void Start()
    {
        pov = GetComponent<POV>();

        stats = GetComponent<Stats>().stats;
        foreach (KeyValuePair<string, Stat> statEntry in stats.statsDict)
        {
            Debug.Log(this.name + " has " + statEntry.Value.GetName() + " as a stat.");
        }
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
                if (stats.statsDict.TryGetValue("AttackDamage", out Stat attackDamage))
                {
                    Attack(attackDamage.GetValue());
                }
                else
                {
                    Debug.LogError("AttackDamage stat not found in " + this);
                }
            }
        }
    }

    public void Idle()
    {
        // cycle between this and patrolling
    }

    public void TakeDamage(float amount)
    {
        if (stats.statsDict.TryGetValue("Health", out Stat health))
        {
            Debug.Log("Enemy " + this.name + " took " + amount + " damage.");
        
            if (health.DecreaseValue(amount))
            {
                Die();
            }
        }
        else
        {
            Debug.LogError("AttackDamage stat not found in " + this);
        }
    }

    public void Attack(float amount)
    {
        currentAttackTimer += Time.deltaTime;
        if (stats.statsDict.TryGetValue("AttackSpeed", out Stat attackSpeed))
        {
            if (currentAttackTimer >= attackSpeed.GetValue())
            {
                Debug.Log("fighting... Dealed: " + amount + " damage to -> " + pov.focusedObject);
                
                StatsScriptableObject targetStats = PlayerController.Instance.GetComponent<StatsScriptableObject>();
                if (targetStats.statsDict.TryGetValue("Health", out Stat health))
                {
                    if (health.DecreaseValue(amount))
                    {
                        PlayerController.Instance.Die();
                    }
                }

                currentAttackTimer = 0;
            }
        }
        else
        {
            Debug.LogError("AttackSpeed stat not found in " + this);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
