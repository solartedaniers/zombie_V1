using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 6f;
    public GameObject hitEffect;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        Destroy(gameObject, lifeTime);
    }

    private void HandleHit(GameObject other, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (hitEffect != null)
        {
            Quaternion rot = hitNormal != Vector3.zero ? Quaternion.LookRotation(hitNormal) : Quaternion.identity;
            GameObject effect = Instantiate(hitEffect, hitPoint, rot);
            Destroy(effect, 1.5f);
        }

        EnemyFollowBase enemy = other.GetComponentInParent<EnemyFollowBase>();
        if (enemy != null)
        {
            enemy.TakeBulletHit(other.GetComponent<Collider>());
        }

        // ✅ Espera una fracción antes de destruir la bala, para evitar que la explosión se pierda visualmente
        Destroy(gameObject, 0.05f);
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 point = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
        Vector3 normal = collision.contactCount > 0 ? collision.GetContact(0).normal : Vector3.up;
        HandleHit(collision.collider.gameObject, point, normal);
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 point = other.ClosestPoint(transform.position);
        Vector3 toCenter = (transform.position - point);
        Vector3 normal = toCenter.sqrMagnitude > 0.0001f ? toCenter.normalized : Vector3.up;
        HandleHit(other.gameObject, point, normal);
    }
}
