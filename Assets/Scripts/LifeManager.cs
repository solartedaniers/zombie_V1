using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [Header("Objetos que representan las vidas")]
    public List<GameObject> lifeObjects;

    [Header("Canvas de Game Over")]
    public GameObject gameOverCanvas;

    [Header("Tiempo invulnerable despues de recibir daño")]
    public float invulnerabilityTime = 5f;

    private bool canTakeDamage = true;

    void Start()
    {
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }

    public void TakeHit()
    {
        if (!canTakeDamage) return;

        RemoveLife();
        StartCoroutine(InvulnerabilityCooldown());
    }

    IEnumerator InvulnerabilityCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(invulnerabilityTime);
        canTakeDamage = true;
    }

    void RemoveLife()
    {
        if (lifeObjects.Count > 0)
        {
            GameObject lastLife = lifeObjects[lifeObjects.Count - 1];

            if (lastLife != null)
                lastLife.SetActive(false);

            lifeObjects.RemoveAt(lifeObjects.Count - 1);
        }

        if (lifeObjects.Count == 0 && gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
