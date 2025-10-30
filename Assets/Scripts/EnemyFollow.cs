using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; // Para la barra de vida
using TMPro; // Para el texto TMP

public class EnemyFollow : MonoBehaviour
{
    [Header("Configuración del enemigo")]
    public float detectionRange = 15f;
    public float speed = 3.5f;
    public int hitsToDie = 2;
    [SerializeField] public float attackRange = 2f;
    [SerializeField] private float attackRangeEnter = 1.8f; // histéresis de entrada
    [SerializeField] private float attackRangeExit = 2.2f;  // histéresis de salida
    [SerializeField] private float locomotionSmooth = 10f;  // suavizado del parámetro Speed

    [Header("UI de Vida")]
    public Image healthBar; // Imagen circular con fill radial
    public TextMeshProUGUI healthText; // Texto TMP que muestra el porcentaje

    private Transform player;
    private NavMeshAgent agent;
    private int currentHits = 0;
    private float lastPlayerHitTime = -999f;
    public float hitCooldownSeconds = 0.75f;
    private Animator animator;
    private float animatorSpeedParam = 0f;
    private bool isAttacking = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
            agent.updateRotation = true;
            agent.updatePosition = true;
            agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, attackRangeEnter);
        }

        animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false; // locomoción en sitio, movimiento por NavMeshAgent
        }
        UpdateHealthUI();
    }

    void Update()
    {
        if (player == null || agent == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
        }

        // Lógica de ataque con histéresis para evitar parpadeos de estados
        if (!isAttacking && distance <= attackRangeEnter)
            isAttacking = true;
        else if (isAttacking && distance >= attackRangeExit)
            isAttacking = false;

        if (agent != null)
            agent.isStopped = isAttacking;

        // Actualizar parámetros del Animator con suavizado
        if (animator != null && agent != null)
        {
            float targetSpeed = isAttacking ? 0f : agent.velocity.magnitude;
            animatorSpeedParam = Mathf.Lerp(animatorSpeedParam, targetSpeed, locomotionSmooth * Time.deltaTime);
            animator.SetFloat("Speed", animatorSpeedParam);
            animator.SetBool("IsAttacking", isAttacking);
        }
    }

    public void TakeHit()
{
    currentHits++;
    Debug.Log("Zombi recibió un disparo. Hits: " + currentHits);
    UpdateHealthUI();

    if (currentHits >= hitsToDie)
    {
        Debug.Log("Zombi destruido");
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        Destroy(gameObject, 1.2f); // permitir que se vea la animación de muerte
    }
}

    // Daño al jugador cuando entra en el trigger del enemigo (recomendado con CharacterController)
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time - lastPlayerHitTime < hitCooldownSeconds) return;
        lastPlayerHitTime = Time.time;

        var life = other.GetComponentInParent<LifeManager>();
        if (life != null)
        {
            // Llama a la lógica de quitar vida del jugador
            life.TakeHit();
        }
        else
        {
            // Alternativa: si usas GameOverManager con corazones
            var gm = FindObjectOfType<GameOverManager>();
            if (gm != null)
            {
                gm.TakeHit();
            }
        }
    }

    // Fallback si decides usar colliders no-trigger y Rigidbody en enemigo
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        if (Time.time - lastPlayerHitTime < hitCooldownSeconds) return;
        lastPlayerHitTime = Time.time;

        var life = collision.collider.GetComponentInParent<LifeManager>();
        if (life != null)
        {
            life.TakeHit();
        }
        else
        {
            var gm = FindObjectOfType<GameOverManager>();
            if (gm != null)
            {
                gm.TakeHit();
            }
        }
    }

    void UpdateHealthUI()
    {
        float healthPercent = 1f - ((float)currentHits / hitsToDie);

        if (healthBar != null)
            healthBar.fillAmount = healthPercent;

        if (healthText != null)
            healthText.text = Mathf.RoundToInt(healthPercent * 100f) + "%";
    }
}