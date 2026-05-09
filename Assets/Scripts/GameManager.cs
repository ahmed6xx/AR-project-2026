using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Buildings")]
    public GameObject eco_Building_Grid;
    public GameObject eco_Building_Slope;
    public GameObject eco_Building_Terrace;

    [Header("Particles")]
    public GameObject fireParticle;
    public GameObject electricityParticle;

    [Header("Repair Button")]
    public GameObject repairButtonCanvas;

    private int currentIncident = 0;
    private GameObject spawnedFire;
    private GameObject spawnedElectricity;

    void Start()
    {
        repairButtonCanvas.SetActive(false);
        Invoke(nameof(TriggerIncident1), 2f);
    }

    // ─── REPAIR ───────────────────────────────────────────────────

    public void OnRepairClicked()
    {
        Debug.Log("Repair clicked! Current incident: " + currentIncident);
        if (currentIncident == 1) ResolveIncident1();
        else if (currentIncident == 2) ResolveIncident2();
        else if (currentIncident == 3) ResolveIncident3();
    }

    // ─── INCIDENT 1 ───────────────────────────────────────────────

    void TriggerIncident1()
    {
        currentIncident = 1;

        spawnedFire = Instantiate(fireParticle);
        spawnedFire.transform.SetParent(eco_Building_Grid.transform);
        spawnedFire.transform.localPosition = new Vector3(-0.2f, -1.2f, -8.9f);
        spawnedFire.transform.localRotation = Quaternion.Euler(-89.98f, 0f, -92.097f);
        spawnedFire.transform.localScale = new Vector3(0.00825986f, 0.00825986f, 0.00825986f);

        repairButtonCanvas.SetActive(true);
        Debug.Log("Incident 1: Fire on Grid Building");
    }

    void ResolveIncident1()
    {
        if (spawnedFire != null) Destroy(spawnedFire);
        repairButtonCanvas.SetActive(false);
        Debug.Log("Incident 1 resolved!");
        Invoke(nameof(TriggerIncident2), 2f);
    }

    // ─── INCIDENT 2 ───────────────────────────────────────────────

    void TriggerIncident2()
    {
        currentIncident = 2;

        spawnedElectricity = Instantiate(electricityParticle);
        spawnedElectricity.transform.SetParent(eco_Building_Slope.transform);
        spawnedElectricity.transform.localPosition = new Vector3(-0.7f, 22.2f, -1.1f);
        spawnedElectricity.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        spawnedElectricity.transform.localScale = new Vector3(3f, 3f, 3f);

        repairButtonCanvas.SetActive(true);
        Debug.Log("Incident 2: Electricity on Slope Building");
    }

    void ResolveIncident2()
    {
        if (spawnedElectricity != null) Destroy(spawnedElectricity);
        repairButtonCanvas.SetActive(false);
        Debug.Log("Incident 2 resolved!");
        Invoke(nameof(TriggerIncident3), 2f);
    }

    // ─── INCIDENT 3 ───────────────────────────────────────────────

    void TriggerIncident3()
    {
        currentIncident = 3;
        repairButtonCanvas.SetActive(true);
        Debug.Log("Incident 3: Terrace Building");
    }

    void ResolveIncident3()
    {
        repairButtonCanvas.SetActive(false);
        Debug.Log("All incidents resolved!");
        // TODO: victory screen
    }

    // ─── Keyboard test ────────────────────────────────────────────

    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.rKey.wasPressedThisFrame) OnRepairClicked();
    }
}