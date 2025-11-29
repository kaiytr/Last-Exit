using UnityEngine;

public class WingbatController : EnemyController
{
    [Header("Health Settings")]
    public int WingbatMaxHealth = 20;

    [Header("Wingbat Attack Settings")]
    public float attackCooldown = 5.0f;
    private float nextAttackTime = 0f;

    private Animator animator;

    private const float SPEED_REDUCTION = 3.0f;

    private const string PARAM_IS_RUNNING = "IsRunning";
    private const string PARAM_ATTACK = "Attack";
    private const string PARAM_DIE = "Die";

    new void Start()
    {
        maxHealth = WingbatMaxHealth;
        base.Start();
        animator = GetComponent<Animator>();
        nextAttackTime = 0f;

        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 Wingbat 오브젝트에 없습니다!");
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
                var playerMove = hit.GetComponent<PlayerMove>();

                if (playerMove != null)
                {
                    playerMove.speed -= SPEED_REDUCTION;
                    if (playerMove.speed < 0) playerMove.speed = 0;

                    Debug.Log($"박쥐 공격! 플레이어 속도 {SPEED_REDUCTION} 감소. 현재 속도: {playerMove.speed}");
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