using UnityEngine;
using UnityEngine.UI;

public class FirePoint : MonoBehaviour
{
    [HideInInspector] public bool isExtinguished = false;

    private Image fireImage;

    void Awake() => fireImage = GetComponent<Image>();

    public void Extinguish()
    {
        if (isExtinguished) return;
        isExtinguished = true;
        // Visual feedback: hide fire or tint it grey
        fireImage.color = new Color(0.4f, 0.4f, 0.4f, 0.5f);
        // Optionally: play a particle/sound here
    }

   public void Reset()
{
    isExtinguished = false;
    if (fireImage != null)
        fireImage.color = Color.white;
}
}