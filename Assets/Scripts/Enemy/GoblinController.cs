using UnityEngine;

public class GoblinController : EnemyController
{
    [Header("Health Settings")]
    public int GoblinMaxHealth = 30;

    [Header("Attack Cooldown")]
    public float attackCooldown = 3.0f;
    private float nextAttackTime = 0f;

    private Animator animator;

    private const int ATTACK_DAMAGE = 15;

    private const string PARAM_IS_RUNNING = "IsRunning";
    private const string PARAM_ATTACK = "Attack";
    private const string PARAM_DIE = "Die";

    new void Start()
    {
        maxHealth = GoblinMaxHealth;
        base.Start();
        animator = GetComponent<Animator>();
        nextAttackTime = 0f;

        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 Goblin 오브젝트에 없습니다!");
        }
    }

    new void Update()
    {
        if (playerTarget == null || isDead)
        {
            HandleIdle();
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTarget.position);

        if (distance <= detectionRange)
        {
            HandleEngage();
        }
        else
        {
            HandleIdle();
        }
    }

    private void HandleEngage()
    {
        animator.SetBool(PARAM_IS_RUNNING, true);
        isMoving = true;

        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger(PARAM_ATTACK);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void HandleIdle()
    {
        animator.SetBool(PARAM_IS_RUNNING, false);
        isMoving = false;
    }

    public void AttackDamageEvent()
    {
        if (isDead) return;

        float hitRadius = 1.0f;
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, hitRadius);

        foreach (var hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                var playerHealth = hit.GetComponent<PlayerMove>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(ATTACK_DAMAGE);
                    return;
                }
            }
        }
    }

    protected override void Die()
    {
        if (isDead) return;
        base.Die();

        animator.SetTrigger(PARAM_DIE);
    }

    public void DestroyObjectEvent()
    {
        Destroy(gameObject);
    }
}