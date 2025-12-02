using UnityEngine;
using System.Collections;

public class WingBatController : EnemyController
{
    [Header("WingBat Settings")]
    public int WingBatMaxHealth = 25;
    public float attackCooldown = 3.0f;

    private float nextAttackTime = 0f;
    private Animator animator;

    private const int WINGBAT_ATTACK_DAMAGE = 8;
    private const int ATTACK_INCREASE_AMOUNT = 2;

    private const string PARAM_IS_FLYING = "IsFlying"; // WingBat�� IsRunning ��� IsFlying ���
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
            Debug.LogError("Animator ������Ʈ�� WingBat ������Ʈ�� �����ϴ�!");
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

    void FixedUpdate()
    {
        if (isDead || !isMoving)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = moveDirection * moveSpeed;
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
            // �̵� ���� ��� �� ����
            Vector2 targetDirection = (playerTarget.position - transform.position).normalized;
            moveDirection = targetDirection;

            // �÷��̾� �ٶ󺸱� (�¿� ����)
            float targetScaleX = Mathf.Abs(transform.localScale.x);
            if (targetDirection.x < 0)
            {
                targetScaleX = Mathf.Abs(transform.localScale.x);
            }
            else if (targetDirection.x > 0)
            {
                targetScaleX = -Mathf.Abs(transform.localScale.x);
            }
            transform.localScale = new Vector3(targetScaleX, transform.localScale.y, transform.localScale.z);

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
        Debug.Log("WingBat died 1");
        if (isDead) return;
        base.Die();

        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null)
        {
            player.IncreaseAttackPower(ATTACK_INCREASE_AMOUNT);
        }

        animator.SetTrigger(PARAM_DIE);
        Debug.Log("WingBat died 2");  
    }

    public void DestroyObjectEvent()
    {
        Destroy(gameObject);
    }
}