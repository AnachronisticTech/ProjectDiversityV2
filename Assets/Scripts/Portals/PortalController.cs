using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public sealed class PortalController : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]
    private Transform teleportTo;

    [SerializeField]
    private Animator transitionAnimator;

    private void Start()
    {
        playerController = PlayerController.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerController.transform.tag))
        {
            StartCoroutine(Teleport());
        }
    }

    private IEnumerator Teleport()
    {
        playerController.disabled = true;

        transitionAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1.0f);

        playerController.transform.SetPositionAndRotation(teleportTo.position, teleportTo.rotation);

        transitionAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.0f);

        playerController.disabled = false;
    }
}
