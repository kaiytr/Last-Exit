using UnityEngine;
using System.Collections;

public class MushroomController : EnemyController
{
    [Header("Health Settings")]
    public int MushroomMaxHealth = 30;

    [Header("Attack Cooldown")]
    public float attackCooldown = 3.0f;
    private float nextAttackTime = 0f;

    private Animator animator;

    private const int ATTACK_DAMAGE = 10;

    private const string PARAM_IS_RUNNING = "IsRunning";
    private const string PARAM_ATTACK = "Attack";
    private const string PARAM_DIE = "Die";

    void Start()
    {
        maxHealth = MushroomMaxHealth;
        base.Start();
        animator = GetComponent<Animator>();
        nextAttackTime = 0f;

        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 Mushroom 오브젝트에 없습니다!");
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
        // 1. 이동 상태 유지
        animator.SetBool(PARAM_IS_RUNNING, true);
        isMoving = true;

        // 2. 공격 조건 체크 및 발동
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger(PARAM_ATTACK);
            nextAttackTime = Time.time + attackCooldown;
            // Run 상태를 끄는 코드를 넣지 않습니다.
        }
    }

    private void HandleIdle()
    {
        // Idle 상태일 때만 Run을 False로 끕니다.
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