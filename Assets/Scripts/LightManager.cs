using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class FullVisionSkill : MonoBehaviour
{
    public Light2D visionLight;

    [Header("시야 설정")]
    public float originalRadius = 5f;
    public float fullRadius = 50f;

    [Header("스킬 효과 설정")]
    public float fadeDuration = 0.5f;
    public float fullVisionHoldTime = 1f;

    private bool isSkillActive = false;

    private void Start()
    {
        if (visionLight != null)
        {
            visionLight.pointLightOuterRadius = originalRadius;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isSkillActive)
        {
            ActivateSkill();
        }
    }

    public void ActivateSkill()
    {
        StopAllCoroutines();
        StartCoroutine(FullVisionCoroutine());
    }

    IEnumerator FullVisionCoroutine()
    {
        isSkillActive = true;

        yield return StartCoroutine(ChangeRadius(visionLight.pointLightOuterRadius, fullRadius, fadeDuration));

        yield return new WaitForSeconds(fullVisionHoldTime);

        yield return StartCoroutine(ChangeRadius(visionLight.pointLightOuterRadius, originalRadius, fadeDuration));

        isSkillActive = false;
    }

    IEnumerator ChangeRadius(float startRadius, float endRadius, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            visionLight.pointLightOuterRadius = Mathf.Lerp(startRadius, endRadius, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        visionLight.pointLightOuterRadius = endRadius;
    }
}