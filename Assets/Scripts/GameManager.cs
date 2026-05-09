using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Buildings")]
    public GameObject eco_Building_Grid;
    public GameObject eco_Building_Slope;
    public GameObject eco_Building_Terrace;

    [Header("Particles")]
    public GameObject fireParticle;
    public GameObject electricityParticle;
    public GameObject floodParticle;

    [Header("Sounds")]
    public AudioClip fireSound;
    public AudioClip electricitySound;
    public AudioClip floodSound;

    [Header("Notification")]
    public TMP_Text notificationText;
    public GameObject repairButtonCanvas;

    private int currentIncident = 0;
    private GameObject spawnedFire;
    private GameObject spawnedElectricity;
    private GameObject spawnedFlood;

    private AudioSource fireAudio;
    private AudioSource electricityAudio;
    private AudioSource floodAudio;

    void Start()
    {
        // Create dedicated AudioSources on this GameObject
        fireAudio        = CreateLoopingAudio(fireSound);
        electricityAudio = CreateLoopingAudio(electricitySound);
        floodAudio       = CreateLoopingAudio(floodSound);

        repairButtonCanvas.SetActive(false);
        Invoke(nameof(TriggerIncident1), 2f);
    }

    AudioSource CreateLoopingAudio(AudioClip clip)
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip        = clip;
        src.loop        = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D — change to 1f if you want 3D positional audio
        return src;
    }

    void SetNotification(string message)
    {
        if (notificationText != null)
            notificationText.text = message;
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
        spawnedFire.transform.localScale    = new Vector3(0.00825986f, 0.00825986f, 0.00825986f);

        fireAudio.Play();
        repairButtonCanvas.SetActive(true);
        SetNotification("Fire at Grid Building!\nClick Repair to fix.");
        Debug.Log("Incident 1: Fire on Grid Building");
    }

    void ResolveIncident1()
    {
        if (spawnedFire != null) Destroy(spawnedFire);
        fireAudio.Stop();
        repairButtonCanvas.SetActive(false);
        SetNotification("Incident 1 Resolved!");
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
        spawnedElectricity.transform.localScale    = new Vector3(3f, 3f, 3f);

        electricityAudio.Play();
        repairButtonCanvas.SetActive(true);
        SetNotification("Electricity Failure at Slope Building!\nClick Repair to fix.");
        Debug.Log("Incident 2: Electricity on Slope Building");
    }

    void ResolveIncident2()
    {
        if (spawnedElectricity != null) Destroy(spawnedElectricity);
        electricityAudio.Stop();
        repairButtonCanvas.SetActive(false);
        SetNotification("Incident 2 Resolved!");
        Debug.Log("Incident 2 resolved!");
        Invoke(nameof(TriggerIncident3), 2f);
    }

    // ─── INCIDENT 3 ───────────────────────────────────────────────

    void TriggerIncident3()
    {
        currentIncident = 3;

        spawnedFlood = Instantiate(floodParticle);
        spawnedFlood.transform.SetParent(eco_Building_Terrace.transform);
        spawnedFlood.transform.localPosition = new Vector3(0f, 20.3f, 7f);
        spawnedFlood.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        spawnedFlood.transform.localScale    = new Vector3(0.003f, 0.003f, 0.003f);

        floodAudio.Play();
        repairButtonCanvas.SetActive(true);
        SetNotification("Flood at Terrace Building!\nClick Repair to fix.");
        Debug.Log("Incident 3: Flood on Terrace Building");
    }

    void ResolveIncident3()
    {
        if (spawnedFlood != null) Destroy(spawnedFlood);
        floodAudio.Stop();
        repairButtonCanvas.SetActive(false);
        SetNotification("All Incidents Resolved! City is Safe.");
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