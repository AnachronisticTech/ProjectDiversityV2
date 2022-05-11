using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalVFXSetup : MonoBehaviour
{
    [System.Serializable]
    private class Portals
    {
        public Renderer frameRenderer;
        public Renderer backdropRenderer;
    }

    [Header("Shared materials")]
    [SerializeField]
    private Material frameMaterial;
    [SerializeField]
    private Material backdropMaterial;

    [Header("Portal references")]
    [SerializeField]
    private Portals firstPortal;
    [SerializeField]
    private Portals secondPortal;

    private Portals[] portalsArray = new Portals[2];

    private void Start()
    {
        if (frameMaterial != null && backdropMaterial != null)
        {
            portalsArray[0] = firstPortal;
            portalsArray[1] = secondPortal;

            for (int i = 0; i < 2; i++)
            {
                portalsArray[i].frameRenderer.material = frameMaterial;
                portalsArray[i].backdropRenderer.material = backdropMaterial;
            }
        }
        else
        {
            Debug.LogError("Materials for " + this + " are missing!");
        }
    }
}
