using System.Collections;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : Entity
{
    public bool IsTest;
    #region 필수 멤버
    public GhostStateMachine StateMachine { get; protected set; }
    public uint GhostId;
    public NavMeshAgent Agent { get; private set; }
    public GhostStatHandler StatHandler { get; private set; }
    public GhostNetworkHandler NetworkHandler { get; private set; }
    public GhostEventHandler EventHandler { get; private set; }
    [field: SerializeField] public GhostBaseSO GhostSO { get; private set; }
    [field: SerializeField] public EGhostType GhostType { get; private set; }
    public bool IsDefeatable { get; protected set; } // 떼어내기 가능한지
    #endregion

    #region State에 쓰이는 변수
    public Vector3 TargetPos { get; set; }
    public Player Target { get; set; }
    public Collider TargetCollider { get; set; }
    public Door FindDoor { get; set; }
    public bool IsAttackReached { get; set; }
    public bool IsPlayerInRange { get; set; } 
    public bool IsDoneAttackAnim { get; set; }
    public bool IsOpeningDoor { get; set; } 
    public bool IsReturnMove { get; set; }
    public bool IsFirstAttack { get; set; }
    #endregion

    private void Start()
    {
        if (IsTest) Init(GhostId, false);
        if (NetworkManager.Instance.IsHost)
        {
            GameManager.Instance.Player.EventHandler.OnDieEvent += HandlePlayerDie;
            foreach (var remotePlayer in RemoteManager.Instance.PlayerDictionary.Values)
            {
                remotePlayer.EventHandler.OnDieEvent += HandlePlayerDie;
            }
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance.IsHost)
        {
            GameManager.Instance.Player.EventHandler.OnDieEvent -= HandlePlayerDie;
            foreach (var remotePlayer in RemoteManager.Instance.PlayerDictionary.Values)
            {
                remotePlayer.EventHandler.OnDieEvent -= HandlePlayerDie;
            }
        }
    }

    /// <summary>
    /// 귀신 생성시 초기화
    /// </summary>
    /// <param name="speed"> 이동속도 </param>
    public virtual void Init(uint ghostId, bool isFail)
    {
        GhostId = ghostId;

        // 네트워크 핸들러 초기화
        NetworkHandler = GetComponent<GhostNetworkHandler>();
        NetworkHandler.Init(this);

        StatHandler = new GhostStatHandler();
        StatHandler.Init(GhostSO);

        if (NetworkManager.Instance.IsHost || IsTest)
        {
            IsFirstAttack = true;

            Agent = GetComponent<NavMeshAgent>();
            if (Agent != null)
            {
                Agent.enabled = true;
                Agent.speed = GhostSO.Speed;
            }

            NetworkHandler.SendMoveRequest();

            EventHandler = GetComponent<GhostEventHandler>();
            EventHandler.Init(this);
        }   
    }

    private void HandlePlayerDie(Player player)
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy) return;
        if (Target == null) return;
        if (Target.gameObject != player.gameObject) return;
        if (IsTargetDieFromMe()) return;

        Animator.speed = 1;
        if (NetworkManager.Instance.IsHost) Agent.isStopped = false;
        OnPlayerNotDetected();
        if (GhostType != EGhostType.E)
            StateMachine.ChangeState(StateMachine.PatrolState);
    }

    private bool IsTargetDieFromMe()
    {
        if(StateMachine.CurrentState == StateMachine.IdleState)
            return true;
        else
            return false;
    }

    private void Update()
    {
        if (StateMachine != null) StateMachine.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Instance.IsHost) return;

        #region Attack
        int playerLayer = LayerMask.NameToLayer("Player");

        if (other.gameObject.layer == playerLayer && !IsAttackReached 
            && other.gameObject.GetComponent<Player>() == Target)
        {
            IsAttackReached = true;
        }
        #endregion

        #region Open Door
        int doorLayer = LayerMask.NameToLayer("Door");

        if (StateMachine.CurrentState != StateMachine.PatrolState 
            && StateMachine.CurrentState != StateMachine.MoveState) return;
        if (other.gameObject.layer == doorLayer)
        {          
            Door door = other.GetComponent<Door>();
            if (door != null && door.currentDoorState == DoorState.DoorMiddle)
            {
                if (!IsTest)
                {
                    door.ToggleRequest(this);
                }                   
                else
                {
                    door.currentDoorState = DoorState.DoorRight;
                    door.GetComponent<NavMeshObstacle>().carving = true;
                }    
                
                StateMachine.ChangeState(StateMachine.OpenDoorState);
            }
        }
        #endregion
    }

    /// <summary>
    /// 공격 애니메이션 끝날 지점에서 Attack처리
    /// </summary>
    public void DoneAttackAnim()
    {
        IsDoneAttackAnim = true;
    }

    public void OnPlayerNotDetected()
    {
        IsPlayerInRange = false;
        Target = null;
        TargetCollider = null;
    }

    #region Sound

    public void PlaySfxFootStepLeft()
    {
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.GhostFootStepLeft, transform.position);
    }

    public void PlaySfxFootStepRight()
    {
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.GhostFootStepRight, transform.position);
    }

    public void PlayAppearSound(EGhostType ghostType)
    {
        switch (ghostType)
        {
            case EGhostType.A:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.GhostAppearA, transform.position);
                break;
            case EGhostType.B:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.GhostAppearB, transform.position);
                break;
            case EGhostType.C:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.GhostAppearC, transform.position);
                break;
            case EGhostType.D:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.GhostAppearD, transform.position);
                break;
            case EGhostType.E:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.GhostAppearE, transform.position);
                break;
        }

    }
    #endregion

    #region See Ghost
    protected GamePacket CreateSeePacket(bool isOn)
    {
        GamePacket packet = new GamePacket();
        packet.GhostSpecialStateRequest = new C2S_GhostSpecialStateRequest();

        packet.GhostSpecialStateRequest.GhostId = GhostId;
        packet.GhostSpecialStateRequest.SpecialState = GhostSpecialState.See;
        packet.GhostSpecialStateRequest.IsOn = isOn;

        return packet;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (StatHandler == null || StatHandler.CurStat == null) return;

        // 공격 범위를 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, StatHandler.CurStat.AttackRange);
    }
    protected virtual void OnDrawGizmos()
    {
        if (StatHandler == null || StatHandler.CurStat == null) return;

        Vector3 ghostPosition = transform.position + Vector3.up * 0.5f;
        Vector3 ghostForward = transform.forward;

        float sightRange = StatHandler.CurStat.Sight;

        // 시야 반경 그리기
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ghostPosition, sightRange);

        // 소리감지 범위 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, StatHandler.CurStat.Hearing);

        // 시야각의 부채꼴
        Gizmos.color = Color.magenta;
        float halfFoV = HORIZONTAL_FOV / 2.0f;

        Vector3 leftBoundary = Quaternion.Euler(0, -halfFoV, 0) * ghostForward * sightRange;
        Vector3 rightBoundary = Quaternion.Euler(0, halfFoV, 0) * ghostForward * sightRange;
        Gizmos.DrawLine(ghostPosition, ghostPosition + leftBoundary);
        Gizmos.DrawLine(ghostPosition, ghostPosition + rightBoundary);

        // 앞 방향 레이
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ghostPosition, ghostPosition + ghostForward * sightRange);

        // 디버깅 텍스트
        Gizmos.color = Color.white;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(ghostPosition + Vector3.up * 2, $"Sight Range: {sightRange:F2}, Sight Angle: {HORIZONTAL_FOV:F2}");
#endif
        if (Agent == null || !Agent.hasPath) return;

        // 기즈모 색 설정
        Gizmos.color = Color.red;

        // 목표 지점에 기즈모를 구 형태로 그림
        Gizmos.DrawSphere(Agent.destination, 0.5f);

        // 현재 위치와 목표 지점을 연결하는 선 그리기
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, Agent.destination);
    }
    #endregion
}
