using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Vuforia;

public class GameManager : MonoBehaviour
{
    [Header("Buildings (6 total)")]
    public GameObject[] buildings; // Drag all 6 buildings here

    [Header("Particles (one prefab per incident type)")]
    public GameObject fireParticle;
    public GameObject electricityParticle;
    public GameObject floodParticle;

    [Header("Sounds")]
    public AudioClip fireSound;
    public AudioClip electricitySound;
    public AudioClip floodSound;
    public AudioClip buttonClickSound;

    [Header("UI")]
    public TMP_Text notificationText;
    public GameObject repairButtonCanvas;

    [Header("Marker")]
    public ObserverBehaviour imageTarget;

    // Internal
    private int currentIncident = 0;
    private GameObject spawnedParticle;
    private GameObject currentBuilding;
    private string currentIncidentType;

    private AudioSource currentAudio;
    private AudioSource fireAudio;
    private AudioSource electricityAudio;
    private AudioSource floodAudio;
    private AudioSource buttonAudio;

    private bool markerDetected = false;

    void Start()
    {
        fireAudio = CreateAudio(fireSound);
        electricityAudio = CreateAudio(electricitySound);
        floodAudio = CreateAudio(floodSound);
        buttonAudio = CreateAudio(buttonClickSound);

        repairButtonCanvas.SetActive(false);
        SetNotification("Scan the marker to begin...");

        // Listen for marker detection
        if (imageTarget != null)
            imageTarget.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (!markerDetected &&
            (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED))
        {
            markerDetected = true;
            SetNotification("City loaded! Monitoring...");
            Invoke(nameof(TriggerRandomIncident), 2f);
        }
    }

    // ─── RANDOM INCIDENT ─────────────────────────────────────────

    void TriggerRandomIncident()
    {
        currentIncident++;

        // Pick random building
        int buildingIndex = Random.Range(0, buildings.Length);
        currentBuilding = buildings[buildingIndex];

        // Pick random incident type
        int incidentType = Random.Range(0, 3);
        string[] types = { "Fire", "Electricity", "Flood" };
        currentIncidentType = types[incidentType];

        // Stop previous audio
        fireAudio.Stop();
        electricityAudio.Stop();
        floodAudio.Stop();

        // Spawn correct particle
        GameObject prefab = null;
        switch (currentIncidentType)
        {
            case "Fire":
                prefab = fireParticle;
                currentAudio = fireAudio;
                break;
            case "Electricity":
                prefab = electricityParticle;
                currentAudio = electricityAudio;
                break;
            case "Flood":
                prefab = floodParticle;
                currentAudio = floodAudio;
                break;
        }

        if (spawnedParticle != null) Destroy(spawnedParticle);

        spawnedParticle = Instantiate(prefab);
        spawnedParticle.transform.SetParent(currentBuilding.transform);

        switch (currentIncidentType)
        {
            case "Fire":
                spawnedParticle.transform.localPosition = Vector3.zero;
                spawnedParticle.transform.localRotation = Quaternion.Euler(-89.98f, 0f, -92.097f);
                spawnedParticle.transform.localScale = new Vector3(0.00825986f, 0.00825986f, 0.00825986f);
                break;
            case "Electricity":
                spawnedParticle.transform.localPosition = new Vector3(-0.7f, 22.2f, -1.1f);
                spawnedParticle.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                spawnedParticle.transform.localScale = new Vector3(3f, 3f, 3f);
                break;
            case "Flood":
                spawnedParticle.transform.localPosition = new Vector3(0f, 20.3f, 7f);
                spawnedParticle.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                spawnedParticle.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                break;
        }

        currentAudio.Play();
        repairButtonCanvas.SetActive(true);
        SetNotification(currentIncidentType + " at " + currentBuilding.name + "!\nClick Repair to fix.");
        Debug.Log("Incident " + currentIncident + ": " + currentIncidentType + " on " + currentBuilding.name);
    }

    // ─── REPAIR ──────────────────────────────────────────────────

    public void OnRepairClicked()
    {
        buttonAudio.PlayOneShot(buttonClickSound);

        if (spawnedParticle != null) Destroy(spawnedParticle);
        currentAudio.Stop();
        repairButtonCanvas.SetActive(false);
        SetNotification(currentIncidentType + " at " + currentBuilding.name + " Resolved!");
        Debug.Log("Incident " + currentIncident + " resolved!");

        // Trigger next incident after random delay between 3 and 8 seconds
        float delay = Random.Range(3f, 8f);
        Invoke(nameof(TriggerRandomIncident), delay);
    }

    // ─── HELPERS ─────────────────────────────────────────────────

    void SetNotification(string message)
    {
        if (notificationText != null)
            notificationText.text = message;
    }

    AudioSource CreateAudio(AudioClip clip)
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;
        return src;
    }

    // ─── Keyboard test ────────────────────────────────────────────

    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.rKey.wasPressedThisFrame) OnRepairClicked();
    }
}