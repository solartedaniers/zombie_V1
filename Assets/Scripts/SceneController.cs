using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // 🔁 Reinicia la escena actual
    public void RestartScene()
    {
        Time.timeScale = 1f; // Asegura que el tiempo se reanude
        Scene currentScene = SceneManager.GetActiveScene();
        // Guardar tiempo ANTES de recargar la escena
        if (GameTimerManager.Instance != null)
            GameTimerManager.Instance.StopSceneTimer(currentScene.buildIndex);
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // 📜 Carga una escena por su índice (en el Build Settings)
    public void LoadSceneByIndex(int sceneIndex)
    {
        Time.timeScale = 1f;
        // Guarda el tiempo ANTES de cambiar de escena
        if (GameTimerManager.Instance != null)
            GameTimerManager.Instance.StopSceneTimer(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(sceneIndex);
    }
}
