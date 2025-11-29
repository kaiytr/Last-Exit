using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyController : MonoBehaviour
{
    [Header("Basic Settings")]
    [HideInInspector] private float moveSpeed;

    public float detectionRange = 10f;

    protected Transform playerTarget;
    private SpriteRenderer spriteRenderer;
    protected bool isMoving = false;

    private Light2D playerLight2D;

    [Header("Health Settings (Base)")]
    [HideInInspector]
    public int maxHealth = 10;
    protected int currentHealth;
    protected bool isDead = false;

    private const int CONTACT_DAMAGE = 5;

    protected void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
            playerLight2D = playerObj.GetComponent<Light2D>();
        }

        if (playerLight2D != null)
        {
            detectionRange = playerLight2D.pointLightOuterRadius;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSpeedByTag();
    }

    void Update() { }

    void FixedUpdate()
    {
        if (playerTarget != null && isMoving && !isDead)
        {
            FollowPlayer();
            LookAtPlayer();
        }
    }

    void SetSpeedByTag()
    {
        switch (gameObject.tag)
        {
            case "Wingbat":
                moveSpeed = 3.0f;
                break;
            case "Goblin":
                moveSpeed = 1.5f;
                break;
            case "Mushroom":
                moveSpeed = 2.5f;
                break;
            default:
                moveSpeed = 2.0f;
                break;
        }
    }

    void FollowPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
    }

    void LookAtPlayer()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (playerTarget.position.x < transform.position.x);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            var playerHealth = collision.gameObject.GetComponent<PlayerMove>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(CONTACT_DAMAGE);
            }
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        isMoving = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log($"{gameObject.name} died.");
    }
}