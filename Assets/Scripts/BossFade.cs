using UnityEngine;
using System.Collections;

public class BossSceneInitializer : MonoBehaviour
{
    void Start()
    {
        // 씬 로드 후 짧은 딜레이 후 FadeIn 실행 (UI가 완전히 준비되는 것을 보장)
        StartCoroutine(StartFadeInAfterLoad());
    }

    IEnumerator StartFadeInAfterLoad()
    {
        // 1. 아주 짧은 시간(예: 한 프레임 또는 0.1초) 대기하여 모든 오브젝트가 로드되도록 합니다.
        yield return new WaitForSeconds(0.1f);

        // 2. FadeManager 인스턴스를 찾아 FadeIn 호출
        if (FadeManager.Instance != null)
        {
            Debug.Log("Boss 씬 로드 완료! Fade In을 시작합니다.");
            FadeManager.Instance.FadeIn();
        }
        else
        {
            Debug.LogError("FadeManager 인스턴스를 찾을 수 없어 FadeIn을 실행할 수 없습니다.");
        }
    }
}