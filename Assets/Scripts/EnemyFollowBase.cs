using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class EnemyFollowBase : MonoBehaviour
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

    protected Transform player;
    protected NavMeshAgent agent;
    protected int currentHits = 0;
    protected Animator animator;
    private float animatorSpeedParam = 0f;
    protected bool isAttacking = false;
    protected bool isDead = false;

    protected virtual void Start()
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

    // UPDATE: Virtual para override en hijos
    public virtual void TakeBulletHit(Collider hitCollider)
    {
        TakeHit();
    }

    public void TakeHit()
    {
        if (isDead) return;

        currentHits++;
        UpdateHealthUI();

        if (currentHits >= hitsToDie)
            Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;
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
        if (isDead) return;
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
