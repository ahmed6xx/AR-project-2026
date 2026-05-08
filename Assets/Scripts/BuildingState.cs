using UnityEngine;

public class BuildingState : MonoBehaviour
{
    // --- Drag your 3 materials here in the Inspector ---
    public Material normalMaterial;
    public Material incidentMaterial;
    public Material resolvedMaterial;

    // The Renderer on your house mesh
    private Renderer buildingRenderer;

    void Start()
    {
        // Grab the Renderer component from this GameObject
        buildingRenderer = GetComponent<Renderer>();

        // Always start in Normal state
        SetState("Normal");
    }
    void Update()
    {
        // Debug: Check if Update is even being called
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Update running - key detected");
        }

        // Debug: Check if this script is still active
        if (!enabled)
        {
            Debug.LogWarning("BuildingState Update called but script is disabled");
            return;
        }

        // Debug: Check if GameObject is active
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("BuildingState: GameObject is inactive in hierarchy!");
            return;
        }

        // Debug: Check if renderer still exists
        if (buildingRenderer == null)
        {
            Debug.LogError("BuildingState: buildingRenderer is null!");
            return;
        }

        // Debug: Log input detection
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q pressed");
            SetState("Normal");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W pressed");
            SetState("Incident");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");
            SetState("Resolved");
        }
    }

    public void SetState(string state)
    {
        switch (state)
        {
            case "Normal":
                buildingRenderer.material = normalMaterial;
                Debug.Log("House → Normal");
                break;

            case "Incident":
                buildingRenderer.material = incidentMaterial;
                Debug.Log("House → Incident");
                break;

            case "Resolved":
                buildingRenderer.material = resolvedMaterial;
                Debug.Log("House → Resolved");
                break;

            default:
                Debug.LogWarning("Unknown state: " + state);
                break;
        }
    }
}