using UnityEngine;
using System.Collections;

public class WingBatController : EnemyController
{
    public int WingBatMaxHealth = 25;
    public float attackCooldown = 3.0f;
    public float moveSpeed = 4.0f;
    public float detectionRange = 7.0f;

    private float nextAttackTime = 0f;
    private Animator animator;

    private const int WINGBAT_ATTACK_DAMAGE = 8;
    private const int ATTACK_INCREASE_AMOUNT = 2;

    private const string PARAM_IS_FLYING = "IsFlying";
    private const string PARAM_ATTACK = "Attack";
    private const string PARAM_DIE = "Die";

    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        maxHealth = WingBatMaxHealth;
        base.Start();
        animator = GetComponent<Animator>();
        nextAttackTime = 0f;

        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 WingBat 오브젝트에 없습니다!");
        }
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
            HandleEngage();
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

    private void HandleEngage()
    {
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("WingBat_Attack");

        if (Time.time >= nextAttackTime && !isAttacking)
        {
            animator.SetTrigger(PARAM_ATTACK);
            nextAttackTime = Time.time + attackCooldown;
        }

        if (!isAttacking)
        {
            Vector2 targetDirection = (playerTarget.position - transform.position).normalized;
            moveDirection = targetDirection;

            FlipSprite(targetDirection.x);

            animator.SetBool(PARAM_IS_FLYING, true);
            isMoving = true;
        }
        else
        {
            animator.SetBool(PARAM_IS_FLYING, false);
            isMoving = false;
            moveDirection = Vector2.zero;
        }
    }

    // 좌우 반전 로직 (스프라이트가 기본적으로 오른쪽을 바라본다고 가정하고 수정)
    private void FlipSprite(float directionX)
    {
        float targetScaleX = Mathf.Abs(transform.localScale.x);

        // 플레이어가 왼쪽에 있을 때 (왼쪽으로 이동해야 함)
        if (directionX < -0.01f)
        {
            // 왼쪽을 바라보도록 반전 (스케일 X를 음수로 설정)
            targetScaleX = -Mathf.Abs(transform.localScale.x);
        }
        // 플레이어가 오른쪽에 있을 때 (오른쪽으로 이동해야 함)
        else if (directionX > 0.01f)
        {
            // 오른쪽을 바라보도록 반전 해제 (스케일 X를 양수로 설정)
            targetScaleX = Mathf.Abs(transform.localScale.x);
        }

        transform.localScale = new Vector3(targetScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void HandleIdle()
    {
        animator.SetBool(PARAM_IS_FLYING, false);
        isMoving = false;
        moveDirection = Vector2.zero;
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
                    playerHealth.TakeDamage(WINGBAT_ATTACK_DAMAGE);
                    return;
                }
            }
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
    }
}