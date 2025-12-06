using UnityEngine;
using System.Collections;

public class MushroomController : EnemyController
{
    // [Inspector에서 설정할 변수]
    [Header("Mushroom Settings")]
    public int MushroomMaxHealth = 30;
    public float attackCooldown = 3.0f;
    public float moveSpeed = 2.5f; // ⚡️ 이동 속도 추가 (Inspector에서 설정)
    public float detectionRange = 7.0f; // ⚡️ 탐지 범위 추가
    public float meleeAttackRange = 1.0f; // ⚡️ 근접 공격 범위 추가

    private float nextAttackTime = 0f;
    private Animator animator;

    private const int ATTACK_DAMAGE = 10;
    private const int ATTACK_INCREASE_AMOUNT = 3;

    private const string PARAM_IS_RUNNING = "IsRunning";
    private const string PARAM_ATTACK = "Attack";
    private const string PARAM_DIE = "Die"; // 사망 파라미터

    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        maxHealth = MushroomMaxHealth;
        // base.Start()는 EnemyController의 기본 초기화를 담당합니다.
        base.Start();
        animator = GetComponent<Animator>();
        nextAttackTime = 0f;

        if (rb != null)
        {
            // Rigidbody2D Kinematic 사용 시 회전 방지
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 Mushroom 오브젝트에 없습니다!");
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
            if (rb != null) rb.linearVelocity = Vector2.zero; // Kinematic 이동 중단
        }
        else
        {
            if (rb != null) rb.linearVelocity = moveDirection * moveSpeed; // Kinematic 이동 실행
        }
    }

    private void HandleEngage(float distance)
    {
        // ⚡️ Animator 상태 이름 확인 (Animator에서 설정된 정확한 이름으로 수정하세요)
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Mushromm_Attack");

        // 1. 공격 로직 (공격 사거리 내 + 쿨타임 충족 + 현재 공격 중이 아님)
        if (distance <= meleeAttackRange && Time.time >= nextAttackTime && !isAttacking)
        {
            animator.SetTrigger(PARAM_ATTACK);
            nextAttackTime = Time.time + attackCooldown;

            // 공격을 시작했으므로 이동을 멈춥니다.
            HandleIdle();
            return;
        }

        // 2. 이동 로직 (공격 중이 아니거나 공격 사거리 밖일 때)
        if (!isAttacking && distance > meleeAttackRange)
        {
            Vector2 targetDirection = (playerTarget.position - transform.position).normalized;
            moveDirection = targetDirection;

            // ⚡️ 좌우 반전 함수 호출
            FlipSprite(targetDirection.x);

            animator.SetBool(PARAM_IS_RUNNING, true);
            isMoving = true;
        }
        else
        {
            // 쿨타임 중이거나 공격 중이거나 사거리 내 대기
            HandleIdle();
        }
    }

    // ⚡️ 좌우 반전 함수 분리 (성공했던 로직 그대로 적용)
    private void FlipSprite(float directionX)
    {
        float targetScaleX = Mathf.Abs(transform.localScale.x);

        // 왼쪽으로 이동해야 할 때 (플레이어가 왼쪽에 있음)
        if (directionX < -0.01f)
        {
            // 왼쪽을 바라보도록 반전 (스케일 X를 음수로 설정)
            targetScaleX = -Mathf.Abs(transform.localScale.x);
        }
        // 오른쪽으로 이동해야 할 때 (플레이어가 오른쪽에 있음)
        else if (directionX > 0.01f)
        {
            // 오른쪽을 바라보도록 반전 해제 (스케일 X를 양수로 설정)
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

        if (animator != null)
        {
            animator.SetTrigger(PARAM_DIE);
        }
    }
}