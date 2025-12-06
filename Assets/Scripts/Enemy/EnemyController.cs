using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Base Enemy Stats")]
    public int maxHealth = 30;
    public float detectionRange = 5.0f;
    public float moveSpeed = 3.0f;

    protected int currentHealth;
    protected bool isDead = false;
    protected Transform playerTarget;
    protected bool isMoving = false;

    private const string PARAM_DIE = "Die";

    protected void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    protected void Update()
    {
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            this.Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        // 사망 시 충돌 비활성화
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        PlayerMove player = FindObjectOfType<PlayerMove>();

        if (player != null)
        {
            int increaseAmount = GetAttackIncreaseAmountByTag(gameObject.tag);
            player.IncreaseAttackPower(increaseAmount);
        }

        // Animator 컴포넌트가 없을 수 있으므로 예외 처리
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger(PARAM_DIE);
        }
    }

   
    private int GetAttackIncreaseAmountByTag(string tag)
    {
        switch (tag)
        {
            case "WingBat": // WingBat 태그를 가진 적을 잡았을 때
                return 1;
            case "Mushroom": // Mushroom 태그를 가진 적을 잡았을 때
                return 1;
            case "Goblin": // Goblin 태그를 가진 적을 잡았을 때
                return 1;
            case "Skeleton": // Skeleton 태그를 가진 적을 잡았을 때
                return 2;
            default:
                Debug.LogWarning($"EnemyController: 알 수 없는 태그 '{tag}'입니다. 기본값 0을 적용합니다.");
                return 0;
        }
    }

    public void DestroyObjectEvent()
    {
        Destroy(gameObject);
    }
}