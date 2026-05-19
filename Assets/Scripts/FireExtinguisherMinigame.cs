using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FireExtinguisherMinigame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject minigamePanel;
    public RectTransform extinguisher;      // the extinguisher RectTransform
    public RectTransform sprayBeam;         // the white beam child of extinguisher
    public FirePoint[] firePoints;          // drag all FirePoint objects here
    public TMP_Text instructionText;
    public Image progressBar;

    [Header("Settings")]
    public float horizontalLimit = 350f;   // how far left/right the extinguisher can slide
    public float extinguishDelay = 0.4f;   // seconds beam must overlap a fire to extinguish it

    [Header("References")]
    public GameManager gameManager;

    // Internals
    private bool isDragging = false;
    private float dragOffsetX;
    private Canvas parentCanvas;
    private int extinguishedCount = 0;
    private float[] overlapTimers;          // one timer per fire point

    // ─── LIFECYCLE ────────────────────────────────────────────────

    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    void Start() => minigamePanel.SetActive(false);

    void Update()
    {
        if (!minigamePanel.activeSelf) return;
        CheckSprayOverlaps();
    }

    // ─── OPEN / CLOSE ─────────────────────────────────────────────

    public void OpenMinigame()
    {
        extinguishedCount = 0;
        overlapTimers = new float[firePoints.Length];

        foreach (var fp in firePoints) fp.Reset();

        extinguisher.anchoredPosition = new Vector2(0, extinguisher.anchoredPosition.y);

        if (progressBar != null) progressBar.fillAmount = 0f;
        if (instructionText != null) instructionText.text = "Éteignez tous les foyers !";

        minigamePanel.SetActive(true);
    }

    public void CloseMinigame() => minigamePanel.SetActive(false);

    // ─── DRAG EVENTS (put EventTrigger on the Extinguisher Image) ─

    public void OnBeginDrag(BaseEventData data)
    {
        var ped = data as PointerEventData;
        if (ped == null) return;

        isDragging = true;

        // Record where inside the extinguisher the player clicked
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            extinguisher.parent as RectTransform,
            ped.position,
            ped.pressEventCamera,
            out Vector2 localPoint
        );
        dragOffsetX = extinguisher.anchoredPosition.x - localPoint.x;
    }

    public void OnDrag(BaseEventData data)
    {
        if (!isDragging) return;
        var ped = data as PointerEventData;
        if (ped == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            extinguisher.parent as RectTransform,
            ped.position,
            ped.pressEventCamera,
            out Vector2 localPoint
        );

        float newX = Mathf.Clamp(localPoint.x + dragOffsetX, -horizontalLimit, horizontalLimit);
        extinguisher.anchoredPosition = new Vector2(newX, extinguisher.anchoredPosition.y);
    }

    public void OnEndDrag(BaseEventData data) => isDragging = false;

    // ─── OVERLAP DETECTION ────────────────────────────────────────

    void CheckSprayOverlaps()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            var fp = firePoints[i];
            if (fp.isExtinguished) continue;

            if (BeamOverlapsFire(fp.GetComponent<RectTransform>()))
            {
                overlapTimers[i] += Time.deltaTime;
                if (overlapTimers[i] >= extinguishDelay)
                    ExtinguishFire(i);
            }
            else
            {
                overlapTimers[i] = 0f; // reset if beam moves away
            }
        }
    }

    bool BeamOverlapsFire(RectTransform fireRect)
    {
        // Compare X positions in the panel's local space
        // The beam is a vertical strip — only X overlap matters
        float beamWorldX = sprayBeam.position.x;
        float beamHalfWidth = (sprayBeam.rect.width * sprayBeam.lossyScale.x) / 2f;

        float fireWorldX = fireRect.position.x;
        float fireHalfWidth = (fireRect.rect.width * fireRect.lossyScale.x) / 2f;

        return Mathf.Abs(beamWorldX - fireWorldX) < (beamHalfWidth + fireHalfWidth);
    }

    void ExtinguishFire(int index)
    {
        firePoints[index].Extinguish();
        extinguishedCount++;

        if (progressBar != null)
            progressBar.fillAmount = (float)extinguishedCount / firePoints.Length;

        if (instructionText != null)
            instructionText.text = $"Foyers éteints : {extinguishedCount}/{firePoints.Length}";

        if (extinguishedCount >= firePoints.Length) Win();
    }

    // ─── WIN ──────────────────────────────────────────────────────

    void Win()
    {
        isDragging = false;
        if (instructionText != null) instructionText.text = "Incendie maîtrisé ! ✓";
        Invoke(nameof(FinishMinigame), 0.8f);
    }

    void FinishMinigame()
    {
        CloseMinigame();
        gameManager.ResolveCurrentIncident();
    }
}