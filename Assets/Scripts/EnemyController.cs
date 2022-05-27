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
    //private Dictionary<string, Stat> enemyStats;

    [SerializeField, Range(0.5f, 2.5f)]
    private float interactRange = 1.0f;

    private float currentAttackTimer = 0.0f;
    private POV pov;

    private void Start()
    {
        pov = GetComponent<POV>();

        //enemyStats = GetComponent<Stats>().GetStatsDict;
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
                //if (enemyStats.TryGetValue("AttackDamage", out Stat attackDamage))
                //{
                //    Attack(attackDamage.GetValue());
                //}
                //else
                //{
                //    Debug.LogError("AttackDamage stat not found in " + this);
                //}
            }
        }
    }

    public void Idle()
    {
        // cycle between this and patrolling
    }

    public void TakeDamage(float amount)
    {
        //if (enemyStats.TryGetValue("Health", out Stat health))
        //{
        //    Debug.Log("Enemy " + this.name + " took " + amount + " damage.");
        //
        //    if (health.DecreaseValue(amount))
        //    {
        //        Die();
        //    }
        //}
        //else
        //{
        //    Debug.LogError("AttackDamage stat not found in " + this);
        //}
    }

    public void Attack(float amount)
    {
        currentAttackTimer += Time.deltaTime;
        //if (enemyStats.TryGetValue("AttackSpeed", out Stat attackSpeed))
        //{
        //    if (currentAttackTimer >= attackSpeed.GetValue())
        //    {
        //        Debug.Log("fighting... Dealed: " + amount + " damage to -> " + pov.focusedObject);
        //
        //        if (PlayerController.Instance.playerStats.TryGetValue("Health", out Stat health))
        //        {
        //            if (health.DecreaseValue(amount))
        //            {
        //                PlayerController.Instance.Die();
        //            }
        //        }
        //
        //        currentAttackTimer = 0;
        //    }
        //}
        //else
        //{
        //    Debug.LogError("AttackSpeed stat not found in " + this);
        //}
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
