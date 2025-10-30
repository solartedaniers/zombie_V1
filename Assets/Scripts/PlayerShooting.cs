using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bulletPrefab;      // Prefab de la bala
    public Transform firePoint;          // Punto de salida del disparo
    public AudioSource shootSound;       // Sonido del disparo
    public float fireRate = 0.5f;        // Tiempo entre disparos
    public float bulletSpeed = 20f;      // Velocidad de la bala
    public float bulletRange = 25f;      // Distancia máxima de la bala

    [Header("Impacto")]
    public GameObject defaultHitEffect;  // Efecto por defecto si el prefab no lo trae

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Crear una nueva bala al disparar
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Activar (por si el prefab está desactivado)
        bullet.SetActive(true);

        // Asegurar que la bala tenga un Collider
        Collider col = bullet.GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider sc = bullet.AddComponent<SphereCollider>();
            sc.isTrigger = false;
            sc.radius = 0.05f;
            col = sc;
        }

        // Asegurar que la bala tenga el script Bullet y configurar sus parámetros
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp == null)
        {
            bulletComp = bullet.AddComponent<Bullet>();
        }
        bulletComp.speed = bulletSpeed;
        if (bulletComp.hitEffect == null && defaultHitEffect != null)
        {
            bulletComp.hitEffect = defaultHitEffect;
        }

        // Si la bala tiene Rigidbody, darle velocidad
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.velocity = firePoint.forward * bulletSpeed;
        }
        else
        {
            // Si no tiene Rigidbody, moverla manualmente
            bullet.AddComponent<BulletMover>().Initialize(bulletSpeed, bulletRange);
        }

        // Reproducir sonido del disparo
        if (shootSound != null)
        {
            shootSound.Play();
        }
    }

    // Clase interna para controlar el movimiento y destrucción de la bala
    private class BulletMover : MonoBehaviour
    {
        private float speed;
        private float range;
        private Vector3 startPosition;

        public void Initialize(float bulletSpeed, float bulletRange)
        {
            speed = bulletSpeed;
            range = bulletRange;
            startPosition = transform.position;
        }

        void Update()
        {
            // Mover la bala hacia adelante
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Verificar distancia recorrida
            if (Vector3.Distance(startPosition, transform.position) >= range)
            {
                Destroy(gameObject);
            }
        }
    }
}
