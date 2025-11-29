using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System; // FadeManager의 Action 콜백을 위해 필요

public class PlayerMove : MonoBehaviour
{
    // --- 기존 이동/애니메이션 관련 변수 ---
    public float speed = 5.0f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    public Animator animator; // (주석 처리된 채 유지)

    // --- 체력 및 부활 관련 변수 ---
    [SerializeField] int maxHealth = 100;
    private int currentHealth;

    // 인스펙터에서 지정할 체크포인트 Transform (EmptyObject)
    public Transform currentCheckpoint;

    // 중복 사망 방지
    private bool isDead = false;

    // --- 테스트용 변수 ---
    [Header("Respawn Test Settings")]
    public float damageInterval = 1f; // 1초마다 데미지
    public int damageAmount = 20; // 20씩 데미지

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 체력 초기화
        currentHealth = maxHealth;
        isDead = false;

        // 씬 로드 후 체크포인트 위치로 플레이어 이동 (부활 지점 설정)
        if (currentCheckpoint != null)
        {
            transform.position = currentCheckpoint.position;
        }

        // ?? 테스트 루틴 시작: 1초마다 데미지를 주기 시작합니다.
        // StartCoroutine(DamageTestRoutine());
    }

    void Update()
    { 

        if (isDead)
        {
            moveDirection = Vector2.zero;
            return;
        }

        // --- 이동 입력 처리 ---
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(x, y).normalized;

        animator.SetInteger("X", (int)x);
        animator.SetInteger("Y", (int)y);

        if (x != 0)
        {
            // ?? 크기 커짐 버그 방지 로직 적용: Y, Z 스케일을 현재 값으로 유지
            transform.localScale = new Vector3(
                -x * Mathf.Abs(transform.localScale.x), // X축만 반전
                transform.localScale.y, // Y축 스케일 유지
                transform.localScale.z  // Z축 스케일 유지
            );
        }

        if (Input.GetMouseButtonDown(0))
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

    // ==========================================================
    // --- 체력 및 부활 로직 ---
    // ==========================================================

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

        // Fader가 있고 체크포인트가 설정되어 있을 경우 인-씬 리스폰 시작
        if (FadeManager.Instance != null && currentCheckpoint != null)
        {
            // 1. Fade Out 명령과 함께 리스폰 로직을 콜백으로 전달
            FadeManager.Instance.FadeOut(() =>
            {
                // **2. Fade Out 완료 후 (검은 화면 상태에서) 실행될 리스폰 로직**

                // a. 플레이어의 위치를 체크포인트로 순간 이동
                transform.position = currentCheckpoint.position;

                // b. 상태 초기화
                currentHealth = maxHealth;
                isDead = false;

                // c. 물리 속도 초기화 (부활 후 미끄러짐 방지)
                rb.linearVelocity = Vector2.zero;

                // d. Fade In 명령 (화면을 밝힘)
                FadeManager.Instance.FadeIn();

                // ?? 테스트 루틴 재시작 (Die 함수 내부에서 isDead가 false로 리셋된 후, 루프를 재시작하거나 새 코루틴을 시작해야 함)
                // 현재 DamageTestRoutine은 무한 루프이므로, Die 후 상태가 리셋되면 자동으로 데미지를 다시 주기 시작합니다.
            });
        }
        else
        {
            // 비상 상황: Fader나 체크포인트가 없으면 경고 후 그냥 씬 리로드
            Debug.LogError("리스폰 환경 오류: FadeManager나 Checkpoint가 설정되지 않아 현재 씬을 리로드합니다.");
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }

    // ==========================================================
    // --- ?? 테스트용: 1초마다 데미지를 주는 코루틴 ---
    // ==========================================================
    /*IEnumerator DamageTestRoutine()
    {
        // 게임이 시작되고 끝날 때까지 반복
        while (true)
        {
            // 설정된 시간(damageInterval)만큼 대기합니다.
            yield return new WaitForSeconds(damageInterval);

            // 플레이어가 살아있는 상태일 때만 데미지 적용
            if (!isDead)
            {
                TakeDamage(damageAmount);
            }
        }
    }*/
}