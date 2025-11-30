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

    private int lastX = 0;
    private int lastY = -1;

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
            if (x != 0)
            {
                animator.SetInteger("X", 1);
                animator.SetInteger("Y", 0);
                lastX = (int)Mathf.Sign(x);
                lastY = 0;
            }
            else if (y != 0)
            {
                animator.SetInteger("X", 0);
                animator.SetInteger("Y", (int)Mathf.Sign(y));
                lastX = 0;
                lastY = (int)Mathf.Sign(y);
            }
            else
            {
                animator.SetInteger("X", 0);
                animator.SetInteger("Y", 0);
            }
        }

        if (x != 0)
        {
            float targetScaleX = Mathf.Abs(transform.localScale.x);

            if (x > 0)
            {
                targetScaleX = -targetScaleX;
            }
            else
            {
                targetScaleX = Mathf.Abs(transform.localScale.x);
            }

            transform.localScale = new Vector3(
                targetScaleX,
                transform.localScale.y,
                transform.localScale.z
            );
        }

        if (Input.GetMouseButtonDown(0) && animator != null)
        {
            if (lastX != 0)
            {
                animator.SetInteger("X", 1);
                animator.SetInteger("Y", 0);
            }
            else if (lastY != 0)
            {
                animator.SetInteger("X", 0);
                animator.SetInteger("Y", lastY);
            }

            if (lastX != 0 || lastY != 0)
            {
                animator.SetTrigger("Attack");
            }
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