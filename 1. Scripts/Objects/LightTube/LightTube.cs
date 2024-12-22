using AkiDevCat.AVL.Components;
using System.Collections;
using UnityEngine;

public class LightTube : MonoBehaviour
{
    enum ELightTubeType
    {
        On,
        OFF
    }

    [Header("# Light")]
    [SerializeField] private Light tubeLight;
    [SerializeField] private VolumetricLight volumetricLight;

    [Header("# Light Property")]
    [SerializeField] private ELightTubeType type;
    [SerializeField] private float twinkleDuration;
    [SerializeField] private float minTwinkleCount;
    [SerializeField] private float maxTwinkleCount;

    private Coroutine twinkleCoroutine;
    private WaitForSeconds twinkleWaitForSeconds; // twinkle 시작 주기
    private readonly WaitForSeconds twinkleShortInterval = new WaitForSeconds(.2f);
    private readonly WaitForSeconds twinkleLongInterval = new WaitForSeconds(.4f);

    private void Start()
    {
        twinkleWaitForSeconds= new WaitForSeconds(twinkleDuration);
        SetLightTube();
    }

    private void OnDestroy()
    {
        StopTwinkle();
    }

    private void SetLightTube()
    {
        switch(type)
        {
            case ELightTubeType.On:
                Twinkle();
                break;
            case ELightTubeType.OFF:
                LightOff();
                break;
        }
    }

    [ContextMenu("Twinkle")]
    private void Twinkle()
    { 
        LightOn();
        StartTwinkle();
    }

    private void LightOn()
    {
        tubeLight.enabled = true;
        volumetricLight.enabled = true;
    }

    private void LightOff()
    {
        tubeLight.enabled = false;
        volumetricLight.enabled = false;
    }

    private void StartTwinkle()
    {
        StopTwinkle();
        twinkleCoroutine = StartCoroutine(TwinkleCoroutine());
    }

    private void StopTwinkle()
    {
        if(twinkleCoroutine != null)
            StopCoroutine(twinkleCoroutine);
    }

    private IEnumerator TwinkleCoroutine()
    {
       while(true)
        {
            float twinkleCount = Random.Range(minTwinkleCount, maxTwinkleCount);
            for (int i = 0; i < twinkleCount; i++) 
            {
                LightOff();
                yield return Random.Range(0f, 1f) > 0.5f ? twinkleShortInterval : twinkleLongInterval;
                LightOn();
                yield return Random.Range(0f, 1f) > 0.5f ? twinkleShortInterval : twinkleLongInterval;
            }
            yield return twinkleWaitForSeconds;
        }
    }
}
