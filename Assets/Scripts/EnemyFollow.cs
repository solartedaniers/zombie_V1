using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class EnemyFollow : MonoBehaviour
{
    [Header("Configuración del enemigo")]
    public float detectionRange = 15f;
    public float speed = 3.5f;
    public int hitsToDie = 2;
    [SerializeField] public float attackRange = 2f;
    [SerializeField] private float attackRangeEnter = 1.8f;
    [SerializeField] private float attackRangeExit = 2.2f;
    [SerializeField] private float locomotionSmooth = 10f;

    [Header("UI de Vida")]
    public Image healthBar;
    public TextMeshProUGUI healthText;

    private Transform player;
    private NavMeshAgent agent;
    private int currentHits = 0;
    private Animator animator;
    private float animatorSpeedParam = 0f;
    private bool isAttacking = false;
    private bool isDead = false;   // ✅ evita múltiples muertes

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
            animator.applyRootMotion = false;

        UpdateHealthUI();
    }

    void Update()
    {
        if (player == null || agent == null || isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
            agent.SetDestination(player.position);
        else
            agent.ResetPath();

        if (!isAttacking && distance <= attackRangeEnter)
            isAttacking = true;
        else if (isAttacking && distance >= attackRangeExit)
            isAttacking = false;

        agent.isStopped = isAttacking;

        if (animator != null)
        {
            float targetSpeed = isAttacking ? 0f : agent.velocity.magnitude;
            animatorSpeedParam = Mathf.Lerp(animatorSpeedParam, targetSpeed, locomotionSmooth * Time.deltaTime);
            animator.SetFloat("Speed", animatorSpeedParam);
            animator.SetBool("IsAttacking", isAttacking);
        }
    }

    public void TakeHit()
    {
        if (isDead) return; // ✅ evita daño después de muerto

        currentHits++;
        UpdateHealthUI();

        if (currentHits >= hitsToDie)
            Die();
    }

    void Die()
    {
        if (isDead) return; // ✅ evita que ejecute dos veces
        isDead = true;

        if (agent != null)
            agent.isStopped = true;

        if (animator != null)
            animator.SetTrigger("Die");

        ZombieManager.Instance.ZombieKilled();
        Destroy(gameObject, 1.2f);
    }

    private void OnTriggerEnter(Collider other) => TryDamage(other);
    private void OnTriggerStay(Collider other) => TryDamage(other);
    private void OnCollisionEnter(Collision collision) => TryDamage(collision.collider);
    private void OnCollisionStay(Collision collision) => TryDamage(collision.collider);

    void TryDamage(Collider col)
    {
        if (isDead) return; // ✅ zombi muerto ya no daña jugador

        if (!col.CompareTag("Player")) return;

        var life = col.GetComponentInParent<LifeManager>();
        if (life != null)
        {
            life.TakeHit();
        }
        else
        {
            var gm = FindObjectOfType<GameOverManager>();
            if (gm != null) gm.TakeHit();
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
