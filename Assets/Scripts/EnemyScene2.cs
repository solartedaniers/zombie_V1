using UnityEngine;

public class EnemyScene2 : EnemyFollowBase
{
    public float speedIncrease = 1.2f;

    protected override void Start()
    {
        hitsToDie = 4;
        speed = 8f;
        detectionRange = 15f;
        base.Start();
    }

    public override void TakeBulletHit(Collider hitCollider)
    {
        if (agent != null)
            agent.speed *= speedIncrease;

        TakeHit();
    }

    // 👇 Nuevo método: detección de colisión con el jugador
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colisión con: " + collision.gameObject.name); // Verifica si detecta al jugador

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("💥 El enemigo ha golpeado al jugador");

            // Verifica si el jugador tiene LifeManager
            LifeManager life = collision.gameObject.GetComponent<LifeManager>();
            if (life != null)
            {
                life.TakeHit();
                Debug.Log("🩸 Daño aplicado al jugador (LifeManager)");
            }
            else
            {
                // Si usas GameOverManager en lugar de LifeManager
                GameOverManager gameOver = collision.gameObject.GetComponent<GameOverManager>();
                if (gameOver != null)
                {
                    gameOver.TakeHit();
                    Debug.Log("🩸 Daño aplicado al jugador (GameOverManager)");
                }
                else
                {
                    Debug.LogWarning("⚠️ No se encontró ningún script de vida en el jugador");
                }
            }
        }
    }

    
}
