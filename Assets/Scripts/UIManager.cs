using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject notificationPanel;
    public TMP_Text notificationText;

    [Header("Buttons")]
    public Button btn_Repair;
    public Button btn_Isolate;
    public Button btn_Extinguish;

    void Start()
    {
        HideAll();

        btn_Repair.onClick.AddListener(OnRepairClicked);
        btn_Isolate.onClick.AddListener(OnIsolateClicked);
        btn_Extinguish.onClick.AddListener(OnExtinguishClicked);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) ShowIncident1_PowerFailure();
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ShowIncident2_WaterLeak();
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ShowIncident3_TrafficFire();
        if (Keyboard.current.digit0Key.wasPressedThisFrame) HideAll();
    }

    // --- Called by GameManager when an incident triggers ---

    public void ShowIncident1_PowerFailure()
    {
        notificationPanel.SetActive(true);
        notificationText.text = "Power Failure at Office Tower!";

        btn_Repair.gameObject.SetActive(true);
        btn_Isolate.gameObject.SetActive(false);
        btn_Extinguish.gameObject.SetActive(false);
    }

    public void ShowIncident2_WaterLeak()
    {
        notificationPanel.SetActive(true);
        notificationText.text = "Water Leak at Residential House!";

        btn_Repair.gameObject.SetActive(true);
        btn_Isolate.gameObject.SetActive(true);
        btn_Extinguish.gameObject.SetActive(false);
    }

    public void ShowIncident3_TrafficFire()
    {
        notificationPanel.SetActive(true);
        notificationText.text = "Traffic Jam & Fire on Road!";

        btn_Repair.gameObject.SetActive(false);
        btn_Isolate.gameObject.SetActive(false);
        btn_Extinguish.gameObject.SetActive(true);
    }

    public void HideAll()
    {
        notificationPanel.SetActive(false);
        btn_Repair.gameObject.SetActive(false);
        btn_Isolate.gameObject.SetActive(false);
        btn_Extinguish.gameObject.SetActive(false);
    }

    // --- Button click handlers ---

    void OnRepairClicked()
    {
        Debug.Log("Repair clicked");
    }

    void OnIsolateClicked()
    {
        Debug.Log("Isolate clicked");
    }

    void OnExtinguishClicked()
    {
        Debug.Log("Extinguish clicked");
    }
}