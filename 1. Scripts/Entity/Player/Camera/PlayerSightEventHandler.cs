using System.Collections;
using UnityEngine;

public class PlayerSightEventHandler : MonoBehaviour
{
    [SerializeField] private Light playerSight;
    [SerializeField] private Color originColor;
    [SerializeField] private Color failSessionColor;

    private readonly float originRange = 8f;
    private readonly float failSessionRange = 8f;

    private readonly float originIntensity = 1f;
    private readonly float failSessionIntensity = .5f;

    private Coroutine failSessionCoroutine;
    private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(.1f);

    private void Start()
    {
        GameManager.Instance.OnFailSession += FailSession;
        GameManager.Instance.OnNextDay += InitPlayerSight;
        InitPlayerSight();
    }

    public void InitPlayerSight()
    {
        playerSight.color = originColor;
        playerSight.range = originRange;
        playerSight.intensity = originIntensity;

        StopFailSession();
    }

    [ContextMenu("Fail Session")]
    private void FailSession()
    {
        playerSight.color = failSessionColor;
        playerSight.range = failSessionRange;

        StartFailSession();
    }

    private void StopFailSession()
    {
        if (failSessionCoroutine != null)
            StopCoroutine(failSessionCoroutine);
    }

    private void StartFailSession()
    {
        StopFailSession();
        failSessionCoroutine = StartCoroutine(FailSessionCoroutine());
    }

    private IEnumerator FailSessionCoroutine()
    {
        while (true)
        {
            playerSight.intensity = originIntensity;
            yield return waitForSeconds;
            playerSight.intensity = failSessionIntensity;
            yield return waitForSeconds;
        }
    }
}
