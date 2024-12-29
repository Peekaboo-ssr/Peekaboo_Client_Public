using System.Collections;
using UnityEngine;

public class DiedEffect : MonoBehaviour
{
    [SerializeField] private float duration;

    private Coroutine dieEffectCoroutine;
    private WaitForSeconds waitForSecond;
    private ParticleSystem particleSystem;

    private void Awake()
    {
        waitForSecond = new WaitForSeconds(duration);
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartDieEffect();
    }

    private void OnDisable()
    {
        StopDieEffect();
    }

    private void StopDieEffect()
    {
        if (dieEffectCoroutine != null)
            StopCoroutine(dieEffectCoroutine);
    }

    private void StartDieEffect()
    {
        StopDieEffect();
        dieEffectCoroutine = StartCoroutine(DieEffect());
    }

    private IEnumerator DieEffect()
    {
        particleSystem.Play();
        yield return waitForSecond;
        particleSystem.Stop();
    }
}
