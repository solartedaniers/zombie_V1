using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject[] hearts;     // Lista de corazones
    public GameObject gameOverUI;   // Canvas de Game Over
    private int currentHealth;

    void Start()
    {
        currentHealth = hearts.Length;
        gameOverUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 🔥 Llamar cuando el Enemy colisiona con el Player
    public void TakeHit()
    {
        if (currentHealth <= 0) return;

        currentHealth--;
        hearts[currentHealth].SetActive(false); // Apaga un corazón

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Time.timeScale = 0f; // Detiene el juego
        gameOverUI.SetActive(true);

        // 🔓 Muestra y libera el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 🔁 Reiniciar la escena actual
    public void RestartGame()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // 🧭 Ir al menú principal
    public void GoToMenu(string menuSceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
