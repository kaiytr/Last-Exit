using UnityEngine;
using System.Collections;

public class BossController : EnemyController
{
    [Header("Boss Settings")]
    public int BossMaxHealth = 300;
    public float attackCooldown = 3.0f;
    public float moveSpeed = 3.5f;
    public float detectionRange = 10.0f;
    public float meleeAttackRange = 2.0f;
    public float selfDestructTime = 60.0f;

    private float nextAttackTime = 0f;
    private Animator animator;

    private const int BOSS_ATTACK_DAMAGE = 25;

    private const string PARAM_IS_RUNNING = "IsRunning";
    private const string PARAM_ATTACK = "Attack";
    private const string PARAM_DIE = "Die";

    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        maxHealth = BossMaxHealth;
        base.Start();
        animator = GetComponent<Animator>();
        nextAttackTime = 0f;

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 Boss 오브젝트에 없습니다!");
        }

        StartCoroutine(SelfDestructTimer());
    }

    protected new void Update()
    {
        if (playerTarget == null || isDead)
        {
            HandleIdle();
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTarget.position);

        if (distance <= detectionRange)
        {
            HandleEngage(distance);
        }
        else
        {
            HandleIdle();
        }
    }

    void FixedUpdate()
    {
        if (isDead || !isMoving)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
        else
        {
            if (rb != null) rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    private void HandleEngage(float distance)
    {
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Attack");

        if (distance <= meleeAttackRange && Time.time >= nextAttackTime && !isAttacking)
        {
            animator.SetTrigger(PARAM_ATTACK);
            nextAttackTime = Time.time + attackCooldown;

            HandleIdle();
            return;
        }

        if (!isAttacking && distance > meleeAttackRange)
        {
            Vector2 targetDirection = (playerTarget.position - transform.position).normalized;
            moveDirection = targetDirection;

            FlipSprite(targetDirection.x);

            animator.SetBool(PARAM_IS_RUNNING, true);
            isMoving = true;
        }
        else
        {
            HandleIdle();
        }
    }

    private void FlipSprite(float directionX)
    {
        float targetScaleX = Mathf.Abs(transform.localScale.x);

        if (directionX < -0.01f)
        {
            targetScaleX = -Mathf.Abs(transform.localScale.x);
        }
        else if (directionX > 0.01f)
        {
            targetScaleX = Mathf.Abs(transform.localScale.x);
        }

        transform.localScale = new Vector3(targetScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void HandleIdle()
    {
        animator.SetBool(PARAM_IS_RUNNING, false);
        isMoving = false;
        moveDirection = Vector2.zero;
    }

    public void MeleeAttackEvent()
    {
        if (isDead) return;

        float hitRadius = 2.0f;
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, hitRadius);

        foreach (var hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                var playerHealth = hit.GetComponent<PlayerMove>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(BOSS_ATTACK_DAMAGE);
                    return;
                }
            }
        }
    }

    IEnumerator SelfDestructTimer()
    {
        yield return new WaitForSeconds(selfDestructTime);

        if (!isDead)
        {
            Debug.Log("Boss: 60초 타이머 만료. 자동 파괴 시작.");
            Die();
        }
    }

    protected override void Die()
    {
        if (isDead) return;

        base.Die();

        if (animator != null)
        {
            animator.SetTrigger(PARAM_DIE);
        }

        StopAllCoroutines();
    }

    public void DestroyOnAnimationEnd()
    {
        Debug.Log("Boss: 사망 애니메이션 완료. 오브젝트 파괴.");
        Destroy(gameObject);
    }
}