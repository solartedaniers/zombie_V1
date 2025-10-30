using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // 🔁 Reinicia la escena actual
    public void RestartScene()
    {
        Time.timeScale = 1f; // Asegura que el tiempo se reanude
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // 📜 Carga una escena por su índice (en el Build Settings)
    public void LoadSceneByIndex(int sceneIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }


}
