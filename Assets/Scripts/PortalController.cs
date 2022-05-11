using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private Transform teleportTo;

    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Player.tag))
        {
            Debug.Log("Collided with " + this.name + ", teleporting to " + teleportTo.name + " ("+ teleportTo.position +")");

            StartCoroutine(Teleport());
        }
    }

    private IEnumerator Teleport()
    {
        playerMovement.DisableMovement = true;

        yield return new WaitForSeconds(Time.deltaTime);

        Player.transform.SetPositionAndRotation(teleportTo.position, teleportTo.rotation);

        yield return new WaitForSeconds(Time.deltaTime);
        
        playerMovement.DisableMovement = false;
    }
}
