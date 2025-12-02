using UnityEngine;
using System.Collections;

public class GoblinController : EnemyController
{
    [Header("Goblin Settings")]
    public int GoblinMaxHealth = 40;
    public float attackCooldown = 2.0f;

    private float nextAttackTime = 0f;
    private Animator animator;

    private const int GOBLIN_ATTACK_DAMAGE = 12;
    private const int ATTACK_INCREASE_AMOUNT = 4; // 처치 시 공격력 +4

    private const string PARAM_IS_RUNNING = "IsRunning";
    private const string PARAM_ATTACK = "Attack";
    private const string PARAM_DIE = "Die";

    // ⚡️ 이동 로직 변수 추가
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Start()
    {
        // ⚡️ Rigidbody 초기화
        rb = GetComponent<Rigidbody2D>();

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

    // ⚡️ 실제 이동 처리
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
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Goblin_Attack");

        if (Time.time >= nextAttackTime && !isAttacking)
        {
            animator.SetTrigger(PARAM_ATTACK);
            nextAttackTime = Time.time + attackCooldown;
        }

        if (!isAttacking)
        {
            // ⚡️ 이동 방향 계산
            Vector2 targetDirection = (playerTarget.position - transform.position).normalized;
            moveDirection = targetDirection;

            // ⚡️ 좌우 반전 로직 (문워크 수정)
            float targetScaleX = Mathf.Abs(transform.localScale.x);

            // 몬스터가 오른쪽으로 이동할 때 (moveDirection.x > 0), 스프라이트 반전
            if (moveDirection.x > 0.01f)
            {
                targetScaleX = -targetScaleX;
            }
            // 몬스터가 왼쪽으로 이동할 때 (moveDirection.x < 0), 스프라이트 정방향
            else if (moveDirection.x < -0.01f)
            {
                targetScaleX = Mathf.Abs(transform.localScale.x);
            }

            transform.localScale = new Vector3(targetScaleX, transform.localScale.y, transform.localScale.z);

            animator.SetBool(PARAM_IS_RUNNING, true);
            isMoving = true;
        }
        else
        {
            animator.SetBool(PARAM_IS_RUNNING, false);
            isMoving = false;
            moveDirection = Vector2.zero; // 공격 중 이동 멈춤
        }
    }

    private void HandleIdle()
    {
        animator.SetBool(PARAM_IS_RUNNING, false);
        isMoving = false;
        moveDirection = Vector2.zero; // 대기 중 이동 멈춤
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
                Debug.Log("Player hit");
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(GOBLIN_ATTACK_DAMAGE);
                    return;
                }
            }
        }
    }

    protected override void Die()
    {
        if (isDead) return;
        base.Die();

        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null)
        {
            player.IncreaseAttackPower(ATTACK_INCREASE_AMOUNT);
        }

        animator.SetTrigger(PARAM_DIE);
        Debug.Log("Goblin Died");
    }

    public void DestroyObjectEvent()
    {
        Destroy(gameObject);
    }
}