using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float speed = 2.0f;

    void Update()
    {
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}