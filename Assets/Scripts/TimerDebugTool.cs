#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class TimerDebugTool : MonoBehaviour
{
    [ContextMenu("Simular tiempos y mostrar resultados")]
    public void SimularTiempos()
    {
        if (GameTimerManager.Instance == null)
        {
            Debug.LogError("GameTimerManager no está presente.");
            return;
        }

        GameTimerManager.Instance.StartSceneTimer(0);
        GameTimerManager.Instance.StopSceneTimer(0);

        GameTimerManager.Instance.StartSceneTimer(1);
        GameTimerManager.Instance.StopSceneTimer(1);

        GameTimerManager.Instance.StartSceneTimer(2);
        GameTimerManager.Instance.StopSceneTimer(2);

        Debug.Log("Tiempo total: " + GameTimerManager.Instance.GetTotalTime());
        Debug.Log("Puntaje: " + GameTimerManager.Instance.GetScore());
    }
}