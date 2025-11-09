using UnityEngine;
using TMPro;

public class FinalResultsManager : MonoBehaviour
{
    [Header("Referencias de Texto")]
    public TextMeshProUGUI scene1TimeText;
    public TextMeshProUGUI scene2TimeText;
    public TextMeshProUGUI scene3TimeText;
    public TextMeshProUGUI totalTimeText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gradeText;

    void Start()
    {
        if (GameTimerManager.Instance == null)
        {
            Debug.LogError("No se encontró el GameTimerManager en la escena final.");
            return;
        }

        // Obtener tiempos de cada nivel jugable según los índices reales
        float t1 = GameTimerManager.Instance.GetSceneTime(2); // Nivel 1
        float t2 = GameTimerManager.Instance.GetSceneTime(3); // Nivel 2
        float t3 = GameTimerManager.Instance.GetSceneTime(4); // Nivel 3

        float total = t1 + t2 + t3;

        // Mostrar tiempos individuales en el Canvas
        scene1TimeText.text = "Escena 1: " + FormatTime(t1);
        scene2TimeText.text = "Escena 2: " + FormatTime(t2);
        scene3TimeText.text = "Escena 3: " + FormatTime(t3);

        // Total acumulado
        totalTimeText.text = "Tiempo total: " + FormatTime(total);

        // Puntuación y calificación
        int score = GameTimerManager.Instance.GetScore();
        scoreText.text = "Puntuación: " + score;
        gradeText.text = "Calificación: " + GetGrade(total);
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:00}:{secs:00}";
    }

    private string GetGrade(float totalTime)
    {
        if (totalTime <= 90f) return "Excelente 🌟";
        if (totalTime <= 150f) return "Bueno 👍";
        if (totalTime <= 240f) return "Regular 😐";
        return "Lento ⏳";
    }
}
