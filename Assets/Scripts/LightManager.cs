using UnityEngine;

public class LightManager : MonoBehaviour
{
   public UnityEngine.Rendering.Universal.Light2D light2D;
    void Start()
    {
        light2D.pointLightOuterRadius = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
