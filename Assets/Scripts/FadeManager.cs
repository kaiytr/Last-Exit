using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System; // Action 델리게이트를 사용하기 위해 필요

public class FadeManager : MonoBehaviour
{
    // 정적 인스턴스: 어디서든 FadeManager에 접근할 수 있게 합니다.
    public static FadeManager Instance;

    [Header("UI Component")]
    // Inspector에서 씬 전체를 덮는 Image 컴포넌트를 할당해주세요.
    public Image fadeImage;

    [Header("Fade Settings")]
    public float fadeDuration = 1.0f; // 페이드가 걸리는 시간

    void Awake()
    {
        // 싱글톤 패턴 및 씬 유지 설정
        if (Instance == null)
        {
            Instance = this;
            // 이 스크립트가 붙은 오브젝트(FadeCanvas)를 파괴하지 않고 유지
            DontDestroyOnLoad(gameObject);

            // 씬 시작 시 바로 Fade In (화면 밝히기)
            FadeIn();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 씬 시작 시 검은색에서 밝아지는 효과
    public void FadeIn(Action onComplete = null)
    {
        StartCoroutine(Fade(1f, 0f, onComplete)); // 투명도 1(검은색) -> 0(투명)
    }

    // 씬 전환/리스폰 전 화면이 검은색으로 변하는 효과
    public void FadeOut(Action onComplete = null)
    {
        StartCoroutine(Fade(0f, 1f, onComplete)); // 투명도 0(투명) -> 1(검은색)
    }

    // 실제 페이드 애니메이션 구현 코루틴 (Action onComplete 추가됨)
    IEnumerator Fade(float startAlpha, float endAlpha, Action onComplete)
    {
        float timer = 0f;
        Color color = fadeImage.color;
        color.a = startAlpha;
        fadeImage.color = color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Lerp 함수를 사용하여 부드럽게 투명도를 변경
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);

            color.a = newAlpha;
            fadeImage.color = color;

            yield return null;
        }

        // 최종 투명도 보장
        color.a = endAlpha;
        fadeImage.color = color;

        // 페이드가 끝난 후, 전달받은 콜백 함수(Action)를 실행합니다.
        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}