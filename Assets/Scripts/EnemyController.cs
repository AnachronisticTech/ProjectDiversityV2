using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this Enemy do]
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(POV))]
public sealed class EnemyController : MonoBehaviour
{
    public float health = 100.0f;
    [SerializeField]
    private float attackDamage = 5.0f;
    [SerializeField]
    private float attackSpeed = 2.0f;
    
    [SerializeField]
    private float currentAttackTimer = 0.0f;

    private POV pov;

    [SerializeField, Range(0.5f, 2.5f)]
    private float interactRange = 1.0f;

    private void Start()
    {
        pov = GetComponent<POV>();
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
                Attack(attackDamage);
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

        health -= amount;

        if (health <= 0.0f)
        {
            Die();
        }
    }

    public void Attack(float amount)
    {
        currentAttackTimer += Time.deltaTime;
        if (currentAttackTimer >= attackSpeed)
        {
            Debug.Log("fighting... Dealed: " + amount + " damage to -> " + pov.focusedObject);
            currentAttackTimer = 0;
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
