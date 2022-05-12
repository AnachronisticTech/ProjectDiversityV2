using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class PortalController : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]
    private Transform teleportTo;

    //[SerializeField]
    //private ParticleSystem transitionParticle;

    private const float wait = 0.5f;

    private void Start()
    {
        playerController = PlayerController.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerController.transform.tag))
        {
            Debug.Log("Collided with " + this.name + ", teleporting to " + teleportTo.name + " ("+ teleportTo.position +")");

            StartCoroutine(Teleport());
        }
    }

    private IEnumerator Teleport()
    {
        playerController.DisableMovement = true;

        yield return new WaitForSeconds(wait);

        //transitionParticle.Play();
        playerController.transform.SetPositionAndRotation(teleportTo.position, teleportTo.rotation);

        yield return new WaitForSeconds(wait);
        
        playerController.DisableMovement = false;
    }
}
