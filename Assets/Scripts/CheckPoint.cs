using UnityEngine;

public class Checkpoint : MonoBehaviour // ?? 클래스 정의 필요
{
    // OnTriggerEnter는 MonoBehaviour를 상속받는 클래스 내부에 정의되어야 합니다.
    // Collider를 인자로 받는 함수입니다.
    private void OnTriggerEnter(Collider other)
    {
        // Collider가 2D인 경우 OnTriggerEnter2D를 사용해야 합니다.
        // 현재는 Collider (3D) 기준으로 작성되었습니다.

        // 플레이어 태그를 확인합니다. (플레이어 객체의 Tag가 "Player"여야 합니다.)
        if (other.CompareTag("Player"))
        {
            // 플레이어 객체에서 PlayerMove 스크립트를 가져옵니다.
            PlayerMove player = other.GetComponent<PlayerMove>();

            if (player != null)
            {
                // PlayerMove 스크립트의 SetCheckpoint 함수를 호출하여 현재 위치를 저장합니다.
                player.SetCheckpoint(this.transform);

                // (선택 사항) 한 번 통과한 체크포인트는 비활성화하여 중복 저장 방지
                // gameObject.SetActive(false); 
            }
        }
    }

    // Collider가 2D인 경우 (Rigidbody2D를 사용하므로 이 함수를 추가해주는 것이 안전합니다.)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove player = other.GetComponent<PlayerMove>();

            if (player != null)
            {
                player.SetCheckpoint(this.transform);
                // gameObject.SetActive(false); 
            }
        }
    }
}