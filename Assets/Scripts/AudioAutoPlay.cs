using UnityEngine;

public class AudioAutoPlay : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip sonido; // Asigna el audio desde el Inspector

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && sonido != null)
        {
            audioSource.clip = sonido;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void OnEnable()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}