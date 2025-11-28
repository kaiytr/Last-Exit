using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTimer : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public float timeToWait = 10f; // Boss 씬으로 전환될 때까지 기다릴 시간 (10초)
    public string bossSceneName = "BossScene"; // 전환할 Boss 씬의 이름

    // 현재 타이머 상태를 보여주기 위한 변수 (선택 사항)
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime; // 시간 증가

        if (timer >= timeToWait)
        {
            // 10초가 지나면 씬 전환 코루틴 시작
            timer = float.MinValue; // 중복 호출 방지
            StartCoroutine(TransitionToBossScene());
        }
    }

    IEnumerator TransitionToBossScene()
    {
        Debug.Log("10초 경과! 씬 전환을 시작합니다.");

        // 1. Fade Out 명령 (화면을 검은색으로 만듦)
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeOut();
            // 페이드 아웃이 완료될 때까지 대기
            yield return new WaitForSeconds(FadeManager.Instance.fadeDuration);
        }
        else
        {
            Debug.LogError("FadeManager 인스턴스를 찾을 수 없습니다! 씬 전환 전 페이드 아웃 없이 바로 전환합니다.");
        }

        // 2. 씬 전환 실행
        SceneManager.LoadScene(bossSceneName);

        // **주의**: FadeManager는 씬이 전환될 때 자동으로 새로운 씬에서 FadeIn을 실행합니다.
        // 따라서 이 코드에서는 별도로 FadeIn을 호출할 필요가 없습니다.
    }
}