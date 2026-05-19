using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Vuforia;

public class GameManager : MonoBehaviour
{
    [Header("Buildings (6 total)")]
    public GameObject[] buildings;
    public float Icon_height = 0.015f;

    [Header("Cars")]
    public GameObject[] cars;

    [Header("Particles (one prefab per incident type)")]
    public GameObject fireParticle;
    public GameObject electricityParticle;
    public GameObject floodParticle;
    public GameObject smokeParticle;

    [Header("Icons (one prefab per incident type)")]
    public GameObject fireIcon;
    public GameObject electricityIcon;
    public GameObject floodIcon;
    public GameObject carCrashIcon;

    [Header("Sounds")]
    public AudioClip fireSound;
    public AudioClip electricitySound;
    public AudioClip floodSound;
    public AudioClip buttonClickSound;
    public AudioClip carCrashSound;
    public AudioClip ambianceSound;
    [Range(0f, 1f)] public float ambianceVolume = 0.3f;

    [Header("UI")]
    public TMP_Text notificationText;
    public GameObject repairButtonCanvas;
    public TMP_Text incidentTypeText;
    public TMP_Text solvedCountText;

    [Header("Marker")]
    public ObserverBehaviour imageTarget;

    [Header("Minigames")]
    public WireMinigame wireMinigame;
    public ValveMinigame valveMinigame;

    // Internal
    private int currentIncident = 0;
    private int solvedCount = 0;
    private GameObject spawnedParticle;
    private GameObject spawnedIcon;
    private GameObject currentBuilding;
    private string currentIncidentType;

    private AudioSource currentAudio;
    private AudioSource fireAudio;
    private AudioSource electricityAudio;
    private AudioSource floodAudio;
    private AudioSource buttonAudio;
    private AudioSource ambianceAudio;
    private AudioSource carCrashAudio;

    private bool markerDetected = false;
    private bool carscrashed = false;
    private GameObject crashedCar;
    private GameObject spawnedSmoke;

    // ── GUARD : empêche plusieurs incidents en même temps ──────────
    private bool incidentActive = false;
    private bool nextIncidentScheduled = false;

    void Start()
    {
        fireAudio = CreateAudio(fireSound);
        electricityAudio = CreateAudio(electricitySound);
        floodAudio = CreateAudio(floodSound);
        buttonAudio = CreateAudio(buttonClickSound);
        ambianceAudio = CreateAudio(ambianceSound);
        carCrashAudio = CreateAudio(carCrashSound);
        carCrashAudio.loop = false;
        ambianceAudio.loop = true;
        ambianceAudio.volume = ambianceVolume;

        repairButtonCanvas.SetActive(false);
        SetNotification("Scan the marker to begin...");
        UpdateIncidentUI("--", false);
        UpdateSolvedUI();

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
            ambianceAudio.Play();
            ScheduleNextIncident(2f);
        }
    }

    // ─── SCHEDULE (une seule invocation à la fois) ────────────────

    void ScheduleNextIncident(float delay)
    {
        if (nextIncidentScheduled) return;   // déjà programmé → on ignore
        nextIncidentScheduled = true;
        Invoke(nameof(TriggerRandomIncident), delay);
    }

    // ─── CARS ────────────────────────────────────────────────────

    void CrashCars()
    {
        if (carscrashed) return;
        carscrashed = true;

        int index = Random.Range(0, cars.Length);
        crashedCar = cars[index];

        Animator anim = crashedCar.GetComponent<Animator>();
        if (anim != null) anim.enabled = false;

        if (smokeParticle != null)
        {
            if (spawnedSmoke != null) Destroy(spawnedSmoke);
            spawnedSmoke = Instantiate(smokeParticle, crashedCar.transform);
            spawnedSmoke.transform.localPosition = new Vector3(-0.01762f, 0.0039f, -0.02355f);
            spawnedSmoke.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            spawnedSmoke.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);
        }

        if (carCrashAudio != null && carCrashSound != null)
            carCrashAudio.PlayOneShot(carCrashSound);

        if (spawnedIcon != null) Destroy(spawnedIcon);
        if (carCrashIcon != null)
        {
            Transform targetParent = imageTarget != null ? imageTarget.transform : null;
            spawnedIcon = Instantiate(carCrashIcon, targetParent);

            Vector3 carLocalPos = targetParent != null
                ? targetParent.InverseTransformPoint(crashedCar.transform.position)
                : crashedCar.transform.position;

            Renderer carRenderer = crashedCar.GetComponent<Renderer>();
            float carTopY = carLocalPos.y;
            if (carRenderer != null)
            {
                float localTop = targetParent != null
                    ? targetParent.InverseTransformPoint(new Vector3(0f, carRenderer.bounds.max.y, 0f)).y
                    : carRenderer.bounds.max.y;
                carTopY = localTop;
            }

            spawnedIcon.transform.localPosition = new Vector3(carLocalPos.x, carTopY + 0.0001f + Icon_height, carLocalPos.z);
            spawnedIcon.transform.localRotation = Quaternion.Euler(90f, 0f, 180f);
            spawnedIcon.transform.localScale = new Vector3(0.00502276f, 0.0001f, 0.00502276f);
        }

        SetNotification("Car breakdown on the road!\nClick Repair to fix.");
        UpdateIncidentUI("Car Crash", true);
        Debug.Log("Car broke down: " + crashedCar.name);
    }

    void ResumeCars()
    {
        carscrashed = false;

        if (crashedCar != null)
        {
            Animator anim = crashedCar.GetComponent<Animator>();
            if (anim != null) anim.enabled = true;
            crashedCar = null;
        }

        if (spawnedSmoke != null) { Destroy(spawnedSmoke); spawnedSmoke = null; }
        if (spawnedIcon != null) { Destroy(spawnedIcon); spawnedIcon = null; }

        Debug.Log("Car repaired, moving again!");
    }

    // ─── RANDOM INCIDENT ─────────────────────────────────────────

    void TriggerRandomIncident()
    {
        nextIncidentScheduled = false;   // le prochain peut maintenant être programmé

        if (incidentActive)
        {
            // Un incident est déjà en cours, on réessaie plus tard
            Debug.LogWarning("TriggerRandomIncident called but incident already active. Retrying...");
            ScheduleNextIncident(2f);
            return;
        }

        incidentActive = true;
        currentIncident++;

        if (!carscrashed && Random.Range(0, 4) == 0)
        {
            currentIncidentType = "CarCrash";
            currentBuilding = null;
            CrashCars();
            repairButtonCanvas.SetActive(true);
            return;
        }

        int buildingIndex = Random.Range(0, buildings.Length);
        currentBuilding = buildings[buildingIndex];

        int incidentType = Random.Range(0, 3);
        string[] types = { "Fire", "Electricity", "Flood" };
        currentIncidentType = types[incidentType];

        fireAudio.Stop();
        electricityAudio.Stop();
        floodAudio.Stop();

        GameObject particlePrefab = null;
        GameObject iconPrefab = null;

        switch (currentIncidentType)
        {
            case "Fire":
                particlePrefab = fireParticle;
                iconPrefab = fireIcon;
                currentAudio = fireAudio;
                break;
            case "Electricity":
                particlePrefab = electricityParticle;
                iconPrefab = electricityIcon;
                currentAudio = electricityAudio;
                break;
            case "Flood":
                particlePrefab = floodParticle;
                iconPrefab = floodIcon;
                currentAudio = floodAudio;
                break;
        }

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

        if (spawnedIcon != null) Destroy(spawnedIcon);
        if (iconPrefab != null)
        {
            Transform targetParent = imageTarget != null ? imageTarget.transform : null;
            spawnedIcon = Instantiate(iconPrefab, targetParent);

            Vector3 buildingLocalPos = targetParent != null
                ? targetParent.InverseTransformPoint(currentBuilding.transform.position)
                : currentBuilding.transform.position;

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
            spawnedIcon.transform.localScale = new Vector3(0.00502276f, 0.0001f, 0.00502276f);
        }

        currentAudio.Play();
        repairButtonCanvas.SetActive(true);
        SetNotification(currentIncidentType + " at " + currentBuilding.name + "!\nClick Repair to fix.");
        UpdateIncidentUI(currentIncidentType, true);
        Debug.Log("Incident " + currentIncident + ": " + currentIncidentType + " on " + currentBuilding.name);
    }

    // ─── REPAIR ──────────────────────────────────────────────────

    public void OnRepairClicked()
    {
        buttonAudio.PlayOneShot(buttonClickSound);

        if (currentIncidentType == "Electricity")
        {
            if (wireMinigame != null)
            {
                wireMinigame.OpenMinigame();
                repairButtonCanvas.SetActive(false);
                return;
            }
        }

        if (currentIncidentType == "Flood")
        {
            if (valveMinigame != null)
            {
                valveMinigame.OpenMinigame();
                repairButtonCanvas.SetActive(false);
                return;
            }
        }

        ResolveCurrentIncident();
    }

    public void ResolveCurrentIncident()
    {
        // Annule toute invocation pendante avant d'en programmer une nouvelle
        CancelInvoke(nameof(TriggerRandomIncident));
        nextIncidentScheduled = false;

        if (currentIncidentType == "CarCrash")
        {
            ResumeCars();
            SetNotification("Traffic cleared! Car moving again.");
        }
        else
        {
            if (spawnedParticle != null) Destroy(spawnedParticle);
            if (spawnedIcon != null) Destroy(spawnedIcon);
            if (currentAudio != null) currentAudio.Stop();
            SetNotification(currentIncidentType + " at " + currentBuilding.name + " Resolved!");
        }

        repairButtonCanvas.SetActive(false);
        solvedCount++;
        incidentActive = false;   // ← l'incident est terminé
        UpdateSolvedUI();
        UpdateIncidentUI("--", false);
        Debug.Log("Incident " + currentIncident + " resolved!");

        float delay = Random.Range(3f, 6f);
        ScheduleNextIncident(delay);
    }

    // ─── HELPERS ─────────────────────────────────────────────────

    void SetNotification(string message)
    {
        if (notificationText != null)
            notificationText.text = message;
    }

    void UpdateIncidentUI(string incidentType, bool active)
    {
        if (incidentTypeText != null)
        {
            incidentTypeText.text = active ? "Current Incident: " + incidentType : "No Active Incident";
            incidentTypeText.color = active ? GetIncidentColor(incidentType) : Color.white;
        }
    }

    Color GetIncidentColor(string type)
    {
        switch (type)
        {
            case "Fire": return new Color(1f, 0.35f, 0f);
            case "Electricity": return new Color(1f, 0.9f, 0f);
            case "Flood": return new Color(0.2f, 0.6f, 1f);
            case "Car Crash": return new Color(1f, 0.2f, 0.2f);
            default: return Color.white;
        }
    }

    void UpdateSolvedUI()
    {
        if (solvedCountText != null)
            solvedCountText.text = "Incidents Solved: " + solvedCount;
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
        if (ambianceAudio != null) ambianceAudio.volume = ambianceVolume;
    }
}