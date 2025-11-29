using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove player = other.GetComponent<PlayerMove>();

            if (player != null)
            {
                player.currentCheckpoint = transform;

                Debug.Log("체크포인트 저장됨: " + transform.position);
            }
        }
    }
}