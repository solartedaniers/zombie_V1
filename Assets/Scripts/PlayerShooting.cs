using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bulletPrefab;       // Prefab de la bala
    public Transform firePoint;           // Punto de salida del disparo
    public AudioSource shootSound;        // Sonido del disparo
    public float fireRate = 0.5f;         // Tiempo entre disparos
    public float bulletSpeed = 20f;       // Velocidad de la bala
    public float bulletRange = 25f;       // Distancia máxima de la bala

    [Header("Impacto")]
    public GameObject defaultHitEffect;   // Efecto por defecto si el prefab no lo trae

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
        // 1 - Raycast desde la cámara (centro de la pantalla)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }

        // 2 - Dirección final del disparo desde el firePoint hacia la mira/crosshair
        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        // 3 - Instancia la bala y que apunte hacia el target
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
        bullet.SetActive(true);

        Collider col = bullet.GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider sc = bullet.AddComponent<SphereCollider>();
            sc.isTrigger = false;
            sc.radius = 0.05f;
            col = sc;
        }

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

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.velocity = shootDirection * bulletSpeed;
        }
        else
        {
            bullet.AddComponent<BulletMover>().Initialize(bulletSpeed, bulletRange);
        }

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
