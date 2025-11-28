using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Basic Settings")]
    private float moveSpeed; // 태그에 따라 자동으로 설정될 속도
    public float detectionRange = 10f; // 플레이어를 감지할 거리

    private Transform playerTarget;
    private SpriteRenderer spriteRenderer; // 적의 방향 전환을 위해 필요

    void Start()
    {
        // 1. 플레이어 찾기 (Player 태그가 붙은 오브젝트를 찾음)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

        // 2. SpriteRenderer 컴포넌트 가져오기 (이미지 좌우 반전을 위해)
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 3. 자신의 태그에 따라 이동 속도 설정 (여기서 4종류 구분)
        SetSpeedByTag();
    }

    void Update()
    {
        if (playerTarget == null) return;

        // 플레이어와 적 사이의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        // 감지 범위 안에 들어왔을 때만 추적
        if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
            LookAtPlayer();
        }
    }

    // 태그별 속도 설정 로직 (핵심 부분)
    void SetSpeedByTag()
    {
        switch (gameObject.tag)
        {
            case "Wingbat":
                moveSpeed = 3.0f; // 날아다니므로 가장 빠름
                break;
            case "Goblin":
                moveSpeed = 2.0f; // 민첩함
                break;
            case "Mushroom":
                moveSpeed = 1.5f; // 보통 속도
                break;
            case "Skeleton":
                moveSpeed = 1.0f; // 아주 느림
                break;
            default:
                moveSpeed = 2.0f; // 태그가 없을 경우 기본 속도
                Debug.Log($"태그가 설정되지 않았거나 알 수 없는 태그입니다: {gameObject.tag}");
                break;
        }
    }

    // 플레이어 추적 로직
    void FollowPlayer()
    {
        // 현재 위치에서 플레이어 위치로 moveSpeed 속도로 이동
        transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
    }

    // 플레이어 바라보기 (좌우 반전)
    void LookAtPlayer()
    {
        if (spriteRenderer != null)
        {
            // 플레이어가 오른쪽에 있으면 flipX = false (원본), 왼쪽에 있으면 true (반전)
            // (스프라이트가 기본적으로 오른쪽을 보고 있다고 가정)
            if (playerTarget.position.x > transform.position.x)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}