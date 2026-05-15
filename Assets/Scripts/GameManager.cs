using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Vuforia;

public class GameManager : MonoBehaviour
{
    [Header("Buildings (6 total)")]
    public GameObject[] buildings; // Drag all 6 buildings here
    public float Icon_height = 0.015f;

    [Header("Particles (one prefab per incident type)")]
    public GameObject fireParticle;
    public GameObject electricityParticle;
    public GameObject floodParticle;

    [Header("Icons (one prefab per incident type)")]
    public GameObject fireIcon;
    public GameObject electricityIcon;
    public GameObject floodIcon;

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
    private GameObject spawnedIcon;          // ← tracks the active icon
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

        // Pick correct prefabs and audio
        GameObject particlePrefab = null;
        GameObject iconPrefab = null;

        switch (currentIncidentType)
        {
            case "Fire":
                particlePrefab = fireParticle;
                iconPrefab     = fireIcon;
                currentAudio   = fireAudio;
                break;
            case "Electricity":
                particlePrefab = electricityParticle;
                iconPrefab     = electricityIcon;
                currentAudio   = electricityAudio;
                break;
            case "Flood":
                particlePrefab = floodParticle;
                iconPrefab     = floodIcon;
                currentAudio   = floodAudio;
                break;
        }

        // ── Spawn particle ──
        if (spawnedParticle != null) Destroy(spawnedParticle);

        spawnedParticle = Instantiate(particlePrefab, currentBuilding.transform);

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

        // ── Spawn icon ──
        if (spawnedIcon != null) Destroy(spawnedIcon);

        if (iconPrefab != null)
        {
            // Parent to ImageTarget so icon follows the marker with the camera
            Transform targetParent = imageTarget != null ? imageTarget.transform : null;
            spawnedIcon = Instantiate(iconPrefab, targetParent);

            // Convert building world pos into ImageTarget local space
            Vector3 buildingLocalPos = targetParent != null
                ? targetParent.InverseTransformPoint(currentBuilding.transform.position)
                : currentBuilding.transform.position;

            // Use only the building's own renderer to avoid picking up spawned particle bounds
            Renderer buildingRenderer = currentBuilding.GetComponent<Renderer>();
            float topY = buildingLocalPos.y;
            if (buildingRenderer != null)
            {
                float localTop = targetParent != null
                    ? targetParent.InverseTransformPoint(new Vector3(0f, buildingRenderer.bounds.max.y, 0f)).y
                    : buildingRenderer.bounds.max.y;
                topY = localTop;
            }

            spawnedIcon.transform.localPosition = new Vector3(buildingLocalPos.x, topY + Icon_height, buildingLocalPos.z);
            spawnedIcon.transform.localRotation = Quaternion.Euler(90f, 0f, 180f);
            spawnedIcon.transform.localScale    = new Vector3(0.00502276f, 0.0001f, 0.00502276f);
            Debug.Log("Icon local pos " + spawnedIcon.transform.localPosition + " world pos " + spawnedIcon.transform.position);
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
        if (spawnedIcon != null)     Destroy(spawnedIcon);      // ← destroy icon too

        currentAudio.Stop();
        repairButtonCanvas.SetActive(false);
        SetNotification(currentIncidentType + " at " + currentBuilding.name + " Resolved!");
        Debug.Log("Incident " + currentIncident + " resolved!");

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