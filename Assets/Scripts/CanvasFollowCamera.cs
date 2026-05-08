using UnityEngine;

public class CanvasFollowCamera : MonoBehaviour
{
    public Camera arCamera;

    // How far in front of the camera the UI appears
    public float distanceFromCamera = 0.8f;

    // How low the UI sits (negative = below center)
    public float verticalOffset = -0.3f;

    void LateUpdate()
    {
        if (arCamera == null) return;

        // Position: in front of camera + slightly down
        Vector3 forward = arCamera.transform.forward;
        Vector3 pos = arCamera.transform.position
                    + forward * distanceFromCamera
                    + Vector3.up * verticalOffset;

        transform.position = pos;

        // Always face the camera
        transform.rotation = Quaternion.LookRotation(forward);
    }
}