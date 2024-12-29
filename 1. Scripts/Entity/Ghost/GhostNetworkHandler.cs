using System.Collections;
using UnityEngine;

public class GhostNetworkHandler : MonoBehaviour
{
    [SerializeField] private WaitForSeconds moveRQWaitForSeconds = new WaitForSeconds(0.1f);
    private Ghost _ghost;
    private Coroutine _moveRQCoroutine;
    private int _preState;

    private Rigidbody _rb;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    public void Init(Ghost ghost)
    {
        _ghost = ghost;
        _preState = _ghost.AnimationData.IdleParameterHash;
        _rb = GetComponent<Rigidbody>();
    }

    #region Move
    /// <summary>
    /// GhostMoveNotification 이벤트 처리
    /// </summary>
    /// <param name="position">목표 위치</param>
    /// <param name="rotation">목표 회전</param>
    public void HandleMoveNotification(Vector3 position, Quaternion rotation)
    {
        if (NetworkManager.Instance.IsHost) return;

        targetPosition = position;
        targetRotation = rotation;
    }

    private void Update()
    {
        if (!NetworkManager.Instance.IsHost)
        {
            _rb.transform.position = Vector3.Lerp(_rb.position, targetPosition, Time.deltaTime * 10f);
            _ghost.transform.rotation = Quaternion.Slerp(_ghost.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void SendMoveRequest()
    {
        if (_moveRQCoroutine != null)
            StopCoroutine(_moveRQCoroutine);
        _moveRQCoroutine = StartCoroutine(SendMoveRequestCoroutine());
    }

    private IEnumerator SendMoveRequestCoroutine()
    {
        while (true)
        {
            MoveRequest(_ghost.transform.position, _ghost.transform.rotation);
            yield return moveRQWaitForSeconds;
        }
    }

    private void MoveRequest(Vector3 position, Quaternion rotation)
    {
        GamePacket packet = new GamePacket();
        packet.GhostMoveRequest = new C2S_GhostMoveRequest();

        GhostMoveInfo ghostMoveInfo = new GhostMoveInfo();

        Position pos = position.ToPosition();
        Rotation rot = rotation.ToRotation();

        ghostMoveInfo.GhostId = _ghost.GhostId;
        ghostMoveInfo.Position = pos;
        ghostMoveInfo.Rotation = rot;

        packet.GhostMoveRequest.GhostMoveInfos.Add(ghostMoveInfo);

        //Debug.Log($"Sending Ghost Move Request: Position=({position.x}, {position.y}, {position.z}), " +
        //  $"Rotation=({rotation.x}, {rotation.y}, {rotation.z}), State={state}");

        GameServerSocketManager.Instance.Send(packet);
    }
    #endregion

    #region State(Anim)

    /// <summary>
    /// Ghost 애니메이션 처리 - Host, Remote 둘다
    /// </summary>
    /// <param name="animatorHash"></param>
    /// <param name="isStart"> 애니메이션 시작이면 true </param>
    public void StartBoolAnim(int animatorHash)
    {
        _ghost.Animator.SetBool(_preState, false);
        _ghost.Animator.SetBool(animatorHash, true);
        _preState = animatorHash;
    }

    public void StateChangeRequest(CharacterState characterState)
    {
        GamePacket packet = new GamePacket();
        packet.GhostStateChangeRequest = new C2S_GhostStateChangeRequest();

        GhostStateInfo ghostStateInfo = new GhostStateInfo();

        ghostStateInfo.GhostId = _ghost.GhostId;
        ghostStateInfo.CharacterState = characterState;

        packet.GhostStateChangeRequest.GhostStateInfo = ghostStateInfo;
        GameServerSocketManager.Instance.Send(packet);
    }

    public void StateSync(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Idle:
                StartBoolAnim(_ghost.AnimationData.IdleParameterHash);
                break;
            case CharacterState.Move:
                StartBoolAnim(_ghost.AnimationData.MoveParameterHash);
                break;
            case CharacterState.Attack:
                _ghost.Animator.SetTrigger(_ghost.AnimationData.AttackParameterHash);
                break;
        }
    }
    #endregion

    #region SpecialState
    public void SpecialStateSync(GhostSpecialState state, bool isOn)
    {
        Debug.Log("스페셜 Ghost Type : " + _ghost.GhostType);
        switch (state)
        {
            case GhostSpecialState.See:
                if (_ghost.GhostType == EGhostType.A)
                {
                    if (isOn) (_ghost as GhostA).SeeGhost();
                    else (_ghost as GhostA).NotSeeGhost();
                }
                else if(_ghost.GhostType == EGhostType.C)
                {
                    if (isOn) (_ghost as GhostC).SeeGhost();
                    else (_ghost as GhostC).NotSeeGhost();
                }
                break;
            case GhostSpecialState.EyeLight:
                if (_ghost.GhostType != EGhostType.E) return;
                if (NetworkManager.Instance.IsHost) return;
                if(isOn) (_ghost as GhostE).OpenEyes();
                else (_ghost as GhostE).CloseEyes();
                break;
            default: break;
        }
    }
    #endregion
}