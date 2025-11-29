using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
// System.Action은 FadeManager에서 사용하므로 여기서는 필요 없습니다.

public class PlayerMove : MonoBehaviour
{
    // --- 기존 이동/애니메이션 관련 변수 ---
    public float speed = 5.0f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    public Animator animator;

    // --- 체력 및 부활 관련 변수 추가 ---
    public int maxHealth = 100;
    private int currentHealth;

    // 인스펙터에서 지정할 체크포인트 Transform (EmptyObject)
    public Transform currentCheckpoint;

    // 중복 사망 방지
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 체력 초기화
        currentHealth = maxHealth;
        isDead = false;

        // 씬 로드 후 체크포인트 위치로 플레이어 이동 (부활 지점 설정)
        if (currentCheckpoint != null)
        {
            rb.position = currentCheckpoint.position;
        }
    }

    void Update()
    {
        if (isDead)
        {
            moveDirection = Vector2.zero;
            return;
        }
        // ... (기존 이동 입력 코드 유지) ...
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(x, y).normalized;

        animator.SetInteger("X", (int)x);
        animator.SetInteger("Y", (int)y);

        if (x != 0)
        {
            transform.localScale = new Vector3(-x, 1, 1);
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

    // ==========================================================
    // --- 체력 및 부활 로직 ---
    // ==========================================================

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log("Checkpoint Updated to: " + newCheckpoint.name);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " Died. Initiating Respawn...");

        // 씬 리로드는 페이드 아웃이 끝난 후 실행되도록 FadeManager에 요청합니다.
        if (FadeManager.Instance != null)
        {
            // FadeOut에 람다식(무명 함수)으로 씬 로드 명령을 전달합니다.
            FadeManager.Instance.FadeOut(() =>
            {
                // 페이드 아웃이 끝난 후 이 코드가 실행됩니다.
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            });
        }
        else
        {
            // Fader가 없을 경우 그냥 바로 씬 리로드 (비상 상황)
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}