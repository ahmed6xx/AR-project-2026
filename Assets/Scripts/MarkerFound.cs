using UnityEngine;
using Vuforia;

public class MarkerFound : MonoBehaviour
{
    private Renderer[] allRenderers;
    private Material[][] savedMaterials;

    void Awake()
    {
        // Save all materials BEFORE Vuforia touches anything
        allRenderers = GetComponentsInChildren<Renderer>();
        savedMaterials = new Material[allRenderers.Length][];

        for (int i = 0; i < allRenderers.Length; i++)
        {
            savedMaterials[i] = allRenderers[i].materials;
        }
    }

    void Start()
    {
        GetComponent<ObserverBehaviour>().OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            // Restore saved materials when marker detected
            for (int i = 0; i < allRenderers.Length; i++)
            {
                allRenderers[i].materials = savedMaterials[i];
            }
            Debug.Log("Materials restored!");
        }
    }
}