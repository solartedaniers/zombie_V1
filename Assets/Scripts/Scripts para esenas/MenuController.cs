using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button playButton;                // Botón "Jugar"
    public Button exitButton;                // Botón "Salir"
    public Button instruccionesButton;       // Botón "Instrucciones"
    public Button salirInstruccionesButton;  // Botón "Salir" o "X" en el canvas de instrucciones

    public AudioSource audioSource;          // AudioSource para sonidos
    public AudioClip playSound;              // Sonido al presionar "Jugar"
    public AudioClip exitSound;              // Sonido al presionar "Salir"

    public GameObject canvasPrincipal;       // Canvas principal (menú)
    public GameObject canvasInstrucciones;   // Canvas de instrucciones

    private int targetSceneIndex = -1;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(() => PlayAndLoadScene(2, playSound));

        if (exitButton != null)
            exitButton.onClick.AddListener(() => PlayAndLoadScene(0, exitSound));

        if (instruccionesButton != null)
            instruccionesButton.onClick.AddListener(MostrarInstrucciones);

        if (salirInstruccionesButton != null)
            salirInstruccionesButton.onClick.AddListener(OcultarInstrucciones);
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

    // Muestra el canvas de instrucciones y oculta el menú principal
    void MostrarInstrucciones()
    {
        if (canvasPrincipal != null)
            canvasPrincipal.SetActive(false);

        if (canvasInstrucciones != null)
            canvasInstrucciones.SetActive(true);
    }

    // Oculta el canvas de instrucciones y restaura el menú principal
    void OcultarInstrucciones()
    {
        if (canvasInstrucciones != null)
            canvasInstrucciones.SetActive(false);

        if (canvasPrincipal != null)
            canvasPrincipal.SetActive(true);
    }
}
