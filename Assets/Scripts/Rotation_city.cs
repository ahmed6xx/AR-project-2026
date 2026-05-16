using UnityEngine;

public class Rotation_city : MonoBehaviour
{
    public float speed = 2.0f;
    
    [Header("Audio Settings")]
    public AudioClip musicClip; // Drag your .wav file here in the Inspector
    [Range(0f, 1f)]
    public float musicVolume = 1.0f;
    
    private AudioSource audioSource;

    void Start()
    {
        // Add AudioSource component if not already attached
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure the AudioSource
        audioSource.clip = musicClip;
        audioSource.loop = true; // Makes music loop continuously
        audioSource.volume = musicVolume;
        audioSource.playOnAwake = false; // We'll control when it plays
        
        // Start playing the music
        if (musicClip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No music clip assigned to Rotation_city!");
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}