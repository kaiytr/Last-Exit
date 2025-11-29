using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System; // Action을 사용하기 위해 필요

public class FadeManager : MonoBehaviour
{
    // 어디서든 접근할 수 있도록 인스턴스 설정
    public static FadeManager Instance;

    [Header("UI Component")]
    // Inspector에서 씬 전체를 덮는 Image 컴포넌트를 할당해주세요. (검은색, 투명도 1로 설정 권장)
    public Image fadeImage;

    [Header("Fade Settings")]
    public float fadeDuration = 1.0f; // 페이드가 걸리는 시간

    void Awake()
    {
        // 싱글톤 패턴 및 씬 유지 설정
        if (Instance == null)
        {
            Instance = this;
            // 이 스크립트가 붙은 오브젝트를 파괴하지 않고 유지
            DontDestroyOnLoad(gameObject);

            // 처음 씬이 로드될 때 화면이 밝아지도록 (Fade In)
            FadeIn();
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 만들어진 것은 파괴
            Destroy(gameObject);
        }
    }

    // 씬 시작 시 검은색에서 밝아지는 효과 (투명도 1 -> 0)
    public void FadeIn()
    {
        StartCoroutine(Fade(1f, 0f, null));
    }

    // 씬 전환 전 화면이 검은색으로 변하는 효과 (투명도 0 -> 1)
    // 페이드가 완료된 후 실행할 함수(콜백)를 인자로 받습니다.
    public void FadeOut(Action onFadeComplete = null)
    {
        StartCoroutine(Fade(0f, 1f, onFadeComplete));
    }

    // 실제 페이드 애니메이션 구현 코루틴
    IEnumerator Fade(float startAlpha, float endAlpha, Action onFadeComplete)
    {
        float timer = 0f;
        Color color = fadeImage.color;

        // 시작 투명도 설정 및 이미지 활성화
        color.a = startAlpha;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(true);

        // 페이드 애니메이션 실행
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Lerp를 사용하여 부드러운 투명도 변화 적용
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);

            color.a = newAlpha;
            fadeImage.color = color;

            yield return null;
        }

        // 최종 투명도 보장
        color.a = endAlpha;
        fadeImage.color = color;

        // 페이드 인(endAlpha=0)이 완료되면 이미지 비활성화
        if (endAlpha == 0f)
        {
            fadeImage.gameObject.SetActive(false);
        }

        // 페이드 완료 후 외부에서 전달받은 콜백 함수 실행 (예: 씬 로드)
        onFadeComplete?.Invoke();
    }
}