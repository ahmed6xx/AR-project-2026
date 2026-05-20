using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FireExtinguisherMinigame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject minigamePanel;
    public RectTransform extinguisher;
    public RectTransform smokeImage;        // can be anywhere in hierarchy
    public FirePoint[] firePoints;
    public TMP_Text instructionText;
    public Image progressBar;

    [Header("Settings")]
    public float horizontalLimit = 300f;
    public float smokeFallSpeed = 500f;
    public float smokeHitRadius = 55f;
    public float smokeSpawnOffsetY = 0f;   // tweak in Inspector to align with extinguisher nozzle

    [Header("References")]
    public GameManager gameManager;

    private bool isDragging = false;
    private float dragOffsetX = 0f;
    private Canvas parentCanvas;
    private RectTransform panelRect;
    private int extinguishedCount = 0;

    private bool smokeActive = false;
    private Vector2 smokeLocalPos;          // always in panelRect local space

    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        panelRect = minigamePanel.GetComponent<RectTransform>();
    }

    void Start()
    {
        minigamePanel.SetActive(false);
        if (smokeImage != null) smokeImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!minigamePanel.activeSelf) return;
        if (smokeActive) UpdateSmokeFall();
    }

    // ─── OPEN / CLOSE ─────────────────────────────────────────────

    public void OpenMinigame()
    {
        extinguishedCount = 0;
        smokeActive = false;
        foreach (var fp in firePoints) fp.Reset();
        extinguisher.anchoredPosition = new Vector2(0f, extinguisher.anchoredPosition.y);
        if (smokeImage != null) smokeImage.gameObject.SetActive(false);
        if (progressBar != null) progressBar.fillAmount = 0f;
        if (instructionText != null) instructionText.text = "Éteignez tous les foyers !";
        minigamePanel.SetActive(true);
    }

    public void CloseMinigame() => minigamePanel.SetActive(false);

    // ─── DRAG ─────────────────────────────────────────────────────

    public void OnBeginDrag(BaseEventData data)
    {
        var ped = data as PointerEventData;
        if (ped == null) return;
        isDragging = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            extinguisher.parent as RectTransform,
            ped.position, ped.pressEventCamera, out Vector2 localPoint);
        dragOffsetX = extinguisher.anchoredPosition.x - localPoint.x;
    }

    public void OnDrag(BaseEventData data)
    {
        if (!isDragging) return;
        var ped = data as PointerEventData;
        if (ped == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            extinguisher.parent as RectTransform,
            ped.position, ped.pressEventCamera, out Vector2 localPoint);
        float newX = Mathf.Clamp(localPoint.x + dragOffsetX, -horizontalLimit, horizontalLimit);
        extinguisher.anchoredPosition = new Vector2(newX, extinguisher.anchoredPosition.y);

        // Launch a new smoke cloud each time the previous one is gone
        if (!smokeActive) LaunchSmoke();
    }

    public void OnEndDrag(BaseEventData data)
    {
        isDragging = false;
    }

    // ─── SMOKE LAUNCH ─────────────────────────────────────────────

    void LaunchSmoke()
    {
        if (smokeActive) return;

        smokeImage.SetParent(panelRect, worldPositionStays: false);

        // anchoredPosition is already in panel-local space — no conversion needed
        float spawnX = extinguisher.anchoredPosition.x;
        float spawnY = extinguisher.anchoredPosition.y - (extinguisher.rect.height * 0.5f) + smokeSpawnOffsetY;

        smokeLocalPos = new Vector2(spawnX, spawnY);
        smokeImage.anchoredPosition = smokeLocalPos;
        smokeImage.gameObject.SetActive(true);
        smokeActive = true;

        Debug.Log($"[Smoke] spawned at anchoredPos={smokeLocalPos}, extinguisher.rect.height={extinguisher.rect.height}");
    }

    // ─── SMOKE FALL ───────────────────────────────────────────────

    void UpdateSmokeFall()
    {
        // Only Y changes — X is locked at launch, so fall is perfectly vertical
        smokeLocalPos.y -= smokeFallSpeed * Time.deltaTime;
        smokeImage.anchoredPosition = smokeLocalPos;

        Camera cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null : parentCanvas.worldCamera;

        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i].isExtinguished) continue;

            // Convert fire world pos → panel-local (same space as smokeLocalPos)
            Vector2 fireScreen = RectTransformUtility.WorldToScreenPoint(
                cam, firePoints[i].GetComponent<RectTransform>().position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panelRect, fireScreen, cam, out Vector2 fireLocal);

            if (Vector2.Distance(smokeLocalPos, fireLocal) < smokeHitRadius)
            {
                ExtinguishFire(i);
                ResetSmoke();
                return;
            }
        }

        if (smokeLocalPos.y < -(panelRect.rect.height * 0.5f) - 100f)
            ResetSmoke();
    }

    void ResetSmoke()
    {
        smokeActive = false;
        smokeImage.gameObject.SetActive(false);
    }

    // ─── EXTINGUISH / WIN ─────────────────────────────────────────

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

    void Win()
    {
        isDragging = false;
        smokeActive = false;
        if (smokeImage != null) smokeImage.gameObject.SetActive(false);
        if (instructionText != null) instructionText.text = "Incendie maîtrisé ! ✓";
        Invoke(nameof(FinishMinigame), 0.8f);
    }

    void FinishMinigame()
    {
        CloseMinigame();
        gameManager.ResolveCurrentIncident();
    }
}