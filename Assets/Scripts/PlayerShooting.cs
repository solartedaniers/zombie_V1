using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bulletPrefab;       // Prefab de la bala
    public Transform firePoint;           // Punto de salida del disparo
    public AudioSource shootSound;        // Sonido del disparo
    public float fireRate = 0.5f;         // Tiempo entre disparos
    public float bulletSpeed = 30f;       // Velocidad de la bala

    [Header("Impacto")]
    public GameObject defaultHitEffect;   // Efecto por defecto si el prefab no lo trae

    [Header("Munición")]
    public int maxAmmo = 30;              // Máximo de balas por recarga
    private int currentAmmo;              // Balas actuales
    public float reloadTime = 1f;         // Tiempo de recarga (1 segundo)
    private bool isReloading = false;     // Estado de recarga
    public TMP_Text ammoText;             // Texto TMP en el Canvas para mostrar la munición

    private float nextFireTime = 0f;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        // Si está recargando, no puede disparar
        if (isReloading) return;

        // Si presiona R y no está recargando, recargar
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        // Si intenta disparar sin balas
        if (currentAmmo <= 0)
        {
            // Mostrar mensaje para recargar
            if (ammoText != null)
            {
                ammoText.text = "Presiona R para recargar";
            }
            return;
        }

        // Disparo normal
        if (Input.GetButtonDown("Fire1") && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Restar una bala
        currentAmmo--;
        UpdateAmmoUI();

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
            bullet.AddComponent<BulletMover>().Initialize(bulletSpeed);
        }

        if (shootSound != null)
        {
            shootSound.Play();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (ammoText != null)
        {
            ammoText.text = "Recargando...";
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "Balas: " + currentAmmo + " / " + maxAmmo;
        }
    }

    // Clase interna para controlar el movimiento continuo de la bala (sin límite de distancia)
    private class BulletMover : MonoBehaviour
    {
        private float speed;

        public void Initialize(float bulletSpeed)
        {
            speed = bulletSpeed;
        }

        void Update()
        {
            // Mover la bala hacia adelante indefinidamente
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}
