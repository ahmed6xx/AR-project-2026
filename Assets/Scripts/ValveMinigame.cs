using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ValveMinigame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject minigamePanel;
    public RectTransform valveImage;   // ← glisse ton Image ici dans l'Inspector
    public TMP_Text instructionText;
    public Image progressBar;

    [Header("Settings")]
    public float degreesRequired = 540f;   // 1.5 tours pour résoudre

    [Header("References")]
    public GameManager gameManager;

    // Internals
    private bool isDragging = false;
    private Vector2 lastMousePos;
    private Vector2 valveCenter;
    private float totalRotation = 0f;
    private float displayAngle = 0f;

    // ─── LIFECYCLE ────────────────────────────────────────────────

    void Start() => minigamePanel.SetActive(false);

    // ─── OPEN / CLOSE ─────────────────────────────────────────────

    public void OpenMinigame()
    {
        totalRotation = 0f;
        displayAngle = 0f;

        if (valveImage != null)
            valveImage.localRotation = Quaternion.identity;

        if (progressBar != null) progressBar.fillAmount = 0f;
        if (instructionText != null) instructionText.text = "Tournez la vanne !";

        minigamePanel.SetActive(true);
    }

    public void CloseMinigame() => minigamePanel.SetActive(false);

    // ─── DRAG EVENTS (Event Trigger sur l'Image dans l'Inspector) ─

    public void OnBeginDrag(BaseEventData data)
    {
        var ped = data as PointerEventData;
        if (ped == null) return;

        isDragging = true;
        valveCenter = RectTransformUtility.WorldToScreenPoint(null, valveImage.position);
        lastMousePos = ped.position;
    }

    public void OnDrag(BaseEventData data)
    {
        if (!isDragging) return;
        var ped = data as PointerEventData;
        if (ped == null) return;

        valveCenter = RectTransformUtility.WorldToScreenPoint(null, valveImage.position);

        Vector2 fromVec = lastMousePos - valveCenter;
        Vector2 toVec = ped.position - valveCenter;
        float angleDelta = Vector2.SignedAngle(fromVec, toVec);

        // Seulement sens horaire
        if (angleDelta < 0)
        {
            totalRotation += Mathf.Abs(angleDelta);
            displayAngle += angleDelta;
            valveImage.localRotation = Quaternion.Euler(0f, 0f, displayAngle);
        }

        lastMousePos = ped.position;

        if (progressBar != null)
            progressBar.fillAmount = Mathf.Clamp01(totalRotation / degreesRequired);

        if (instructionText != null)
        {
            int pct = Mathf.Min(Mathf.RoundToInt(totalRotation / degreesRequired * 100f), 100);
            instructionText.text = "Fermeture... " + pct + "%";
        }

        if (totalRotation >= degreesRequired) Win();
    }

    public void OnEndDrag(BaseEventData data) => isDragging = false;

    // ─── WIN ──────────────────────────────────────────────────────

    void Win()
    {
        isDragging = false;
        if (instructionText != null) instructionText.text = "Vanne fermée ! ✓";
        Invoke(nameof(FinishMinigame), 0.8f);
    }

    void FinishMinigame()
    {
        CloseMinigame();
        gameManager.ResolveCurrentIncident();
    }
}