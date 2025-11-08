using UnityEngine;

public class GameTimerManager : MonoBehaviour
{
    public static GameTimerManager Instance;

    private float[] sceneTimes = new float[3]; // Almacena el tiempo de las 3 escenas
    private float startTime;
    private bool timerRunning = false;
    private int currentSceneIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Se mantiene entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartSceneTimer(int sceneIndex)
    {
        currentSceneIndex = sceneIndex;
        startTime = Time.time;
        timerRunning = true;
    }

    public void StopSceneTimer(int sceneIndex)
    {
        if (!timerRunning) return;
        timerRunning = false;
        float elapsed = Time.time - startTime;
        sceneTimes[sceneIndex] = elapsed;
    }

    public float GetSceneTime(int sceneIndex)
    {
        return sceneTimes[sceneIndex];
    }

    // 🔹 Nuevo método: obtener tiempo en vivo (aunque el timer esté corriendo)
    public float GetCurrentSceneElapsedTime()
    {
        if (!timerRunning) return sceneTimes[currentSceneIndex];
        return Time.time - startTime;
    }

    public float GetTotalTime()
    {
        float total = 0f;
        foreach (float t in sceneTimes)
            total += t;
        return total;
    }

    public int GetScore()
    {
        float totalTime = GetTotalTime();
        if (totalTime <= 60f) return 1000;
        if (totalTime <= 120f) return 800;
        if (totalTime <= 180f) return 600;
        return 400;
    }
}
