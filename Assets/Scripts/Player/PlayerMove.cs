using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    public float speed = 10.0f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    public Animator animator;

    [SerializeField] int maxHealth = 100;
    private int currentHealth;

    public Transform currentCheckpoint;
    private bool isDead = false;

    [Header("Respawn Test Settings")]
    public float damageInterval = 1f;
    public int damageAmount = 20;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        isDead = false;

        if (currentCheckpoint != null)
        {
            transform.position = currentCheckpoint.position;
        }
    }

    void Update()
    {
        if (isDead)
        {
            moveDirection = Vector2.zero;
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(x, y).normalized;

        if (animator != null)
        {
            animator.SetInteger("X", (int)x);
            animator.SetInteger("Y", (int)y);
        }

        if (x != 0)
        {
            transform.localScale = new Vector3(
                -x * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }

        if (Input.GetMouseButtonDown(0) && animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = moveDirection * speed;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (currentCheckpoint != null)
        {
            Debug.Log("Player Died. Respawning...");
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        transform.position = currentCheckpoint.position;
        currentHealth = maxHealth;
        speed = 10.0f;
        isDead = false;
        rb.linearVelocity = Vector2.zero;
    }
}