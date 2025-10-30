using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public VideoPlayer videoPlayer;       // Asigna el VideoPlayer en el Inspector
    public Button transitionButton;       // Asigna el botón en el Inspector
    public AudioSource audioSource;       // Asigna el AudioSource con el sonido
    public AudioClip transitionSound;     // Asigna el clip de sonido en el Inspector

    private bool sceneLoading = false;    // Para evitar doble carga

    void Start()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;

        if (transitionButton != null)
            transitionButton.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        PlaySoundAndLoadScene();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        PlaySoundAndLoadScene();
    }

    void PlaySoundAndLoadScene()
    {
        if (sceneLoading) return; // Evita múltiples cargas
        sceneLoading = true;

        if (audioSource != null && transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        // Espera el tiempo del sonido antes de cambiar de escena
        float delay = transitionSound != null ? transitionSound.length : 0f;
        Invoke(nameof(LoadScene1), delay);
    }

    void LoadScene1()
    {
        SceneManager.LoadScene(1);
    }
}