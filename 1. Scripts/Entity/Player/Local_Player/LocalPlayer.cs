using System.Collections;
using UnityEngine;

public partial class LocalPlayer : Player
{
    public PlayerStateMachine StateMachine { get; private set; }
    public LocalPlayerInputHandler InputHandler { get; private set; }
    public LocalPlayerInteractHandler InteractHandler { get; private set; }
    public LocalPlayerStatHandler StatHandler { get; private set; }
    public LocalPlayerNetworkHandler NetworkHandler { get; private set; }
    public LocalPlayerAnimEventHandler AnimEventHandler { get; private set; }

    [Header("Jump CoolDown")]
    [field: SerializeField] public bool CanJump { get; private set; }
    private WaitForSeconds jumpWaitForSeconds;
    private Coroutine jumpCoroutine;

    [Header("Stamina Recovery")]
    private Coroutine staminaRecoveryCoroutine;
    private Coroutine staminaDrainCoroutine;
    private WaitForSeconds staminaWaitForSeconds = new WaitForSeconds(1f);

    protected override void Awake()
    {
        base.Awake();
        InputHandler = GetComponent<LocalPlayerInputHandler>();
        NetworkHandler = GetComponent<LocalPlayerNetworkHandler>();
        InteractHandler = GetComponent<LocalPlayerInteractHandler>();
        AnimEventHandler = GetComponentInChildren<LocalPlayerAnimEventHandler>();
        
        StateMachine = new PlayerStateMachine(this);
        StatHandler = new LocalPlayerStatHandler();
        GameManager.Instance.Player = this;
    }

    protected override void Start()
    {
        base.Start();
        PlayerInit();

        jumpWaitForSeconds = new WaitForSeconds(PlayerSO.JumpCoolDown);

        EventHandler.OnJumpEvent += Jump;

        GameManager.Instance.OnNextDay += NetworkHandler.SendMoveRequest;
        GameServerSocketManager.Instance.OnMainSceneLoad += NetworkHandler.SendMoveRequest;
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    private void Update()
    {
        StateMachine.Update();
    }

    public void PlayerInit()
    {
        StatHandler.Init(PlayerSO);
        StateMachine.ChangeState(StateMachine.IdleState);

        NetworkHandler.StartInteraction();

        // Jump 
        CanJump = true;
    }

    protected override void PlayerDie(Player player)
    {
        base.PlayerDie(player);

        NetworkHandler.StopMoveRequest();
        NetworkHandler.BlockInteraction();

        SoundManager.Instance.StopHeartBeatBgm();

        UIManager.Instance.OpenDieUI();
        gameObject.SetActive(false);
    }

    #region ContextMenu
    [ContextMenu("Die")]
    public void b()
    {
        StatHandler.TakeDamageRequest(1005);
    }
    #endregion

    #region Jump
    private void StartCheckJumpCoolDown()
    {
        if(CanJump)
        {
            StopCheckJumpCoolDown();
            jumpCoroutine = StartCoroutine(CheckJumpCoroutine());
        }
    }

    private void StopCheckJumpCoolDown()
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
        }
    }

    private IEnumerator CheckJumpCoroutine()
    {
        CanJump = false;
        yield return jumpWaitForSeconds;
        CanJump = true;
    }

    private void Jump()
    {
        if (CanJump)
        {
            AnimationHandler.StartAnimTrigger(AnimationData.JumpParameterHash, this);
            Rigidbody.AddForce(Vector3.up * PlayerSO.JumpForce, ForceMode.Impulse);

            StartCheckJumpCoolDown();
        }
    }
    #endregion

    #region Stamina

    // Stamina Recovery
    public void StartRecoveryStamina()
    {
        StopRecoveryStamina();
        staminaRecoveryCoroutine = StartCoroutine(RecoveryStaminaCoroutine());
    }

    public void StopRecoveryStamina()
    {
        if (staminaRecoveryCoroutine != null)
        {
            StopCoroutine(staminaRecoveryCoroutine);
        }
    }

    private IEnumerator RecoveryStaminaCoroutine()
    {
        while(!StatHandler.IsStaminaFull())
        {
            StatHandler.StaminaRecovery();
            yield return staminaWaitForSeconds;
        }
    }

    // Stamina Use
    public void StartDrainStamina()
    {
        StopDrainStamina();
        staminaDrainCoroutine = StartCoroutine(StaminaDrainCoroutine());
    }

    public void StopDrainStamina()
    {
        if (staminaDrainCoroutine != null)
        {
            StopCoroutine(staminaDrainCoroutine);
        }
    }

    private IEnumerator StaminaDrainCoroutine()
    {
        while (true)
        {    
            if (StatHandler.CanRun())
            {
                StateMachine.ChangeState(StateMachine.RunState);
                StopRecoveryStamina();
            } 
            else // 달리는 도중 Stamina 떨어지면
            {
                StateMachine.ChangeState(StateMachine.MoveState); // 걷기 상태로 전환
                StartRecoveryStamina(); // 스태미나 회복
                new WaitUntil(() => StatHandler.CanRun());  // 스태미나가 다시 충분해질 때까지 대기
            }
            yield return staminaWaitForSeconds;

        }
    }
    #endregion
}
