using UnityEngine;

public class EnemyBoss : EnemyFollowBase
{
    private float speedIncreaseOnHit = 0.5f; // cuánto sube la velocidad por impacto fuera de los ojos

    protected override void Start()
    {
        hitsToDie = 10;     
        speed = 6f;       
        detectionRange = 30f;
        attackRange = 3f;   

        base.Start();
    }

    protected override void Die()
    {
        base.Die();
        // Cosas extra del jefe aquí
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            // Revisamos si NO le pega a los ojos
            if (!collision.collider.CompareTag("Eyes"))
            {
                speed += speedIncreaseOnHit;
            }

            hitsToDie--;

            if (hitsToDie <= 0)
            {
                Die();
            }
        }
    }
}
