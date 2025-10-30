using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [Header("Objetos que representan las vidas (en orden)")]
    public List<GameObject> lifeObjects; // Ej: corazones, luces, etc.

    [Header("Canvas que se activa cuando no quedan vidas")]
    public GameObject gameOverCanvas;

    [Header("Detección propia (desactivar para evitar doble daño)")]
    public bool enableSelfCollisionDetection = false;

    void Start()
    {
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }

    // ✅ Detecta colisiones entre Player y Enemy (opcional)
    private void OnCollisionEnter(Collision collision)
    {
        if (!enableSelfCollisionDetection) return;
        // Si este objeto tiene el tag "Player" y choca con un "Enemy"
        if (CompareTag("Player") && collision.gameObject.CompareTag("Enemy"))
        {
            RemoveLife();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enableSelfCollisionDetection) return;
        // También soporta triggers, por si usas colliders tipo trigger
        if (CompareTag("Player") && other.CompareTag("Enemy"))
        {
            RemoveLife();
        }
    }

    public void TakeHit()
    {
        RemoveLife();
    }

    void RemoveLife()
    {
        if (lifeObjects.Count > 0)
        {
            // Toma el último objeto de la lista
            GameObject lastLife = lifeObjects[lifeObjects.Count - 1];
            
            // Lo desactiva
            if (lastLife != null)
                lastLife.SetActive(false);

            // Lo elimina de la lista
            lifeObjects.RemoveAt(lifeObjects.Count - 1);

            Debug.Log($"❤️ Se perdió una vida. Vidas restantes: {lifeObjects.Count}");
        }

        // Si ya no quedan vidas, muestra el canvas
        if (lifeObjects.Count == 0 && gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            Debug.Log("💀 Game Over — todas las vidas perdidas");
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
