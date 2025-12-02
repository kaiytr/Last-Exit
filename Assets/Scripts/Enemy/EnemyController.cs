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

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;

        // 사망 시 충돌 비활성화
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    public void DestroyObjectEvent()
    {
        Destroy(gameObject);
    }
}