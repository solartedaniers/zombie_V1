using UnityEngine;
using TMPro;

public class ZombieManager : MonoBehaviour
{
    public static ZombieManager Instance;

    public int totalZombies = 11;
    public TextMeshProUGUI zombieCounterText;
    public GameObject victoryCanvas;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
        if (victoryCanvas != null)
            victoryCanvas.SetActive(false);
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
        Time.timeScale = 0f;
        victoryCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
