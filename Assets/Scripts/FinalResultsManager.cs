using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalResultsManager : MonoBehaviour
{
    [Header("Referencias de Texto")]
    public TextMeshProUGUI scene1TimeText;
    public TextMeshProUGUI scene2TimeText;
    public TextMeshProUGUI scene3TimeText;
    public TextMeshProUGUI totalTimeText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gradeText;

    [Header("Botón para volver a escena 1")]
    public Button reiniciarButton; // Asigna este botón desde el inspector

    void Start()
    {
        if (GameTimerManager.Instance == null)
        {
            Debug.LogError("No se encontró el GameTimerManager en la escena final.");
            return;
        }

        float t1 = GameTimerManager.Instance.GetSceneTime(2); // Nivel 1
        float t2 = GameTimerManager.Instance.GetSceneTime(3); // Nivel 2
        float t3 = GameTimerManager.Instance.GetSceneTime(4); // Nivel 3

        float total = t1 + t2 + t3;

        scene1TimeText.text = "Escena 1: " + FormatTime(t1);
        scene2TimeText.text = "Escena 2: " + FormatTime(t2);
        scene3TimeText.text = "Escena 3: " + FormatTime(t3);

        totalTimeText.text = "Tiempo total: " + FormatTime(total);

        int score;
        string grade;

        if (total <= 240f) // 4 min o menos
        {
            score = 1000;
            grade = "Excelente";
        }
        else if (total <= 360f) // Entre 5 y 6 min
        {
            score = 600;
            grade = "Bueno";
        }
        else if (total <= 480f) // Entre 7 y 8 min
        {
            score = 400;
            grade = "Regular";
        }
        else // 9 min o más
        {
            score = 100;
            grade = "Lento";
        }

        scoreText.text = "Puntuación: " + score;
        gradeText.text = "Calificación: " + grade;

        // Asigna el evento del botón para reiniciar a escena 1
        if (reiniciarButton != null)
            reiniciarButton.onClick.AddListener(() => IrAEscenaUno());
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:00}:{secs:00}";
    }

    // Método para ir directamente a la escena 1
    public void IrAEscenaUno()
    {
        SceneManager.LoadScene(1); // Cambia el número si tu escena 1 no es el índice 1
    }
}
