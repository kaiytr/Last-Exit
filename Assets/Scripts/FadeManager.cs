using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    // 어디서든 접근할 수 있도록 인스턴스 설정
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

            // 처음 씬이 로드될 때 (MainScene 시작 시) Fade In
            FadeIn();
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 만들어진 것은 파괴
            Destroy(gameObject);
        }
    }

    // 씬 시작 시 검은색에서 밝아지는 효과
    public void FadeIn()
    {
        StartCoroutine(Fade(1f, 0f)); // 투명도 1(검은색) -> 0(투명)
    }

    // 씬 전환 전 화면이 검은색으로 변하는 효과
    public void FadeOut()
    {
        StartCoroutine(Fade(0f, 1f)); // 투명도 0(투명) -> 1(검은색)
    }

    // 실제 페이드 애니메이션 구현 코루틴
    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color color = fadeImage.color;

        // 시작 투명도 설정
        color.a = startAlpha;
        fadeImage.color = color;

        // 페이드 애니메이션 실행
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);

            color.a = newAlpha;
            fadeImage.color = color;

            yield return null;
        }

        // 최종 투명도 보장
        color.a = endAlpha;
        fadeImage.color = color;
    }
}