using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button playButton;         // Botón "Jugar"
    public Button exitButton;         // Botón "Salir"
    public AudioSource audioSource;   // AudioSource para reproducir sonidos
    public AudioClip playSound;       // Sonido al presionar "Jugar"
    public AudioClip exitSound;       // Sonido al presionar "Salir"

    private int targetSceneIndex = -1;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(() => PlayAndLoadScene(2, playSound));

        if (exitButton != null)
            exitButton.onClick.AddListener(() => PlayAndLoadScene(0, exitSound));
    }

    void PlayAndLoadScene(int sceneIndex, AudioClip sound)
    {
        targetSceneIndex = sceneIndex;

        if (audioSource != null && sound != null)
            audioSource.PlayOneShot(sound);

        float delay = sound != null ? sound.length : 0f;
        Invoke(nameof(LoadTargetScene), delay);
    }

    void LoadTargetScene()
    {
        if (targetSceneIndex >= 0)
            SceneManager.LoadScene(targetSceneIndex);
    }
}