using System.Collections;
using UnityEngine;

public partial class Player : Entity
{ 
    public PlayerEventHandler EventHandler { get; private set; }
    public PlayerItemHoldHandler ItemHoldHandler { get; private set; }
    public PlayerAnimationHandler AnimationHandler { get; private set; }
    public PlayerGhostDetectionHandler GhostDetectionHandler { get; private set; }
    [field : SerializeField] public PlayerBaseSO PlayerSO { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public Transform CameraTransform { get; private set; }
    public string UserID {  get; private set; }
    public string UserNickName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        EventHandler = GetComponent<PlayerEventHandler>();
        ItemHoldHandler = GetComponent<PlayerItemHoldHandler>();
        AnimationHandler = GetComponent<PlayerAnimationHandler>();
        GhostDetectionHandler = GetComponent<PlayerGhostDetectionHandler>();
        Rigidbody = GetComponentInChildren<Rigidbody>();
    }
    
    protected virtual void Start()
    {
        EventHandler.OnDieEvent += PlayerDie;
        GhostDetectionHandler.Init(this);
    }

    public void SetUserID(string userId)
    {
        UserID = userId;
    }

    public void SetUserNickName(string nickName)
    {
        UserNickName = nickName;
    }

    protected virtual void PlayerDie(Player player)
    {
        GhostDetectionHandler.StopDetection();
    }

    #region Sound

    private Coroutine footstepCoroutine;

    private WaitForSeconds walkSoundInterval = new WaitForSeconds(.4f);
    private WaitForSeconds runLeftSoundInterval = new WaitForSeconds(.2f);
    private WaitForSeconds runRightSoundInterval = new WaitForSeconds(.1f); // .1f 차이 나게 설정
    public void StartFootStepSound(bool isRun)
    {
        StopFootStepSound();
        footstepCoroutine = StartCoroutine(FootStep(isRun));
    }

    public void StopFootStepSound()
    {
        if(footstepCoroutine != null) 
            StopCoroutine(footstepCoroutine);
    }

    private IEnumerator FootStep(bool isRun)
    {
        WaitForSeconds leftSoundInterval;
        WaitForSeconds rightSoundInterval;

        leftSoundInterval = isRun? runLeftSoundInterval : walkSoundInterval;
        rightSoundInterval = isRun? runRightSoundInterval : walkSoundInterval;

        while (true)
        {
            SoundManager.Instance.PlayInGameSfx(EInGameSfx.FootStepLeft, transform.position);
            yield return leftSoundInterval;
            SoundManager.Instance.PlayInGameSfx(EInGameSfx.FootStepRight, transform.position);
            yield return rightSoundInterval;
        }
    }

    public void HitSound(Transform soundPos)
    {
        int randomIdx = Random.Range(0, 4);
        switch (randomIdx)
        {
            case 0:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.PlayerHitA, soundPos.position);
                break;
            case 1:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.PlayerHitB, soundPos.position);
                break;
            case 2:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.PlayerHitC, soundPos.position);
                break;
            case 3:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.PlayerHitD, soundPos.position);
                break;
        }

    }

    #endregion
}
