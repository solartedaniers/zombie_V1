using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ZombieManager : MonoBehaviour
{
    public static ZombieManager Instance;

    public int totalZombies = 11;
    public TextMeshProUGUI zombieCounterText;
    public GameObject victoryCanvas;
    public TextMeshProUGUI missionTimeText;
    public TextMeshProUGUI timerText;

    private bool missionActive = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
        if (victoryCanvas != null)
            victoryCanvas.SetActive(false);

        // Iniciar cronómetro global
        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.StartSceneTimer(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Update()
    {
        if (missionActive && GameTimerManager.Instance != null)
        {
            float elapsed = GameTimerManager.Instance.GetCurrentSceneElapsedTime();
            int minutes = Mathf.FloorToInt(elapsed / 60);
            int seconds = Mathf.FloorToInt(elapsed % 60);
            if (timerText != null)
                timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void ZombieKilled()
    {
        totalZombies--;
        UpdateUI();

        if (totalZombies <= 0)
        {
            Victory();
        }
    }

    void UpdateUI()
    {
        zombieCounterText.text = "Zombis: " + totalZombies;
    }

    void Victory()
    {
        missionActive = false;

        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.StopSceneTimer(SceneManager.GetActiveScene().buildIndex);
            float time = GameTimerManager.Instance.GetSceneTime(SceneManager.GetActiveScene().buildIndex);

            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            string formattedTime = $"{minutes:00}:{seconds:00}";

            if (missionTimeText != null)
                missionTimeText.text = "Tiempo de misión: " + formattedTime;
        }

        Time.timeScale = 0f;
        victoryCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToScene4()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }
}
