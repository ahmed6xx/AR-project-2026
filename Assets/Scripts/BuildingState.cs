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
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetState("Normal");
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetState("Incident");
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetState("Resolved");
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