using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [Header("DoorData")]
    public uint doorId; // 차후 remoteManager에 등록할 때 필요한 id
    public float duration;
    public DoorState currentDoorState = DoorState.DoorMiddle;
    public EDoorType DoorType = EDoorType.Default;
    public bool isMove = false;
    public bool isRight = false;

    [Header("회전 Type")]
    public Ease ease;

    [Header("Sound Type")]
    public bool isMainDoor;

    [Header("DEV Test")]
    public bool IsToggle;
    public DoorState targetDoorState = DoorState.DoorMiddle;
    private Quaternion targetRotation;

    private void Start()
    {
        GameManager.Instance.OnNextDay += Init;
    }

    private bool isRot // 문이 회전 되어 있는 지
    {
        get => transform.parent.rotation.y % 180 != 0;
    }
    private void Init()
    {
        RemoteToggle(DoorState.DoorMiddle);
    }

    #region DEV
    [ContextMenu("DoorToggleNotification")]
    public void ToggleNotiTest()
    {
        // 문이 열리는 각도 구하기
        RemoteToggle(targetDoorState);
    }
    [ContextMenu("DoorToggleRequest")]
    public void ToggleReqTest(Entity entity)
    {
        Vector3 dir = (entity.transform.position - transform.parent.position).normalized;
        GetTargetState(dir);
    }
    private float GetOpenTargetAngle(Vector3 dir)
    {
        float targetAngle = 0;
        switch (targetDoorState)
        {
            case DoorState.DoorLeft:
                targetAngle = 0;
                break;

            case DoorState.DoorRight:
                targetAngle = 0;
                break;

            case DoorState.DoorMiddle:
                if (isRot)
                {
                    if (targetDoorState == DoorState.DoorLeft) targetAngle = -90;
                    else targetAngle = 90;
                    break;
                }
                else
                {
                    if (targetDoorState == DoorState.DoorRight) targetAngle = 90;
                    else targetAngle = -90;
                    break;
                }
        }
        return targetAngle;
    }
    #endregion

    #region SERVER
    public void ToggleRequest(Entity entity)
    {
        if (isMove) return;
        Vector3 dir = (entity.transform.position - transform.parent.position).normalized;
        DoorState doorState = GetTargetState(dir);

        DoorToggleRequest(doorId, doorState);
    }
    private DoorState GetTargetState(Vector3 dir) 
    {
        DoorState targetState = new DoorState();
        switch (currentDoorState)
        {
            case DoorState.DoorLeft:
                targetState = DoorState.DoorMiddle;
                break;

            case DoorState.DoorRight:
                targetState = DoorState.DoorMiddle;
                break;

            case DoorState.DoorMiddle:
                if (isRot)
                {
                    if (dir.z > 0) targetState = DoorState.DoorLeft;
                    else targetState = DoorState.DoorRight;
                    break;
                }
                else
                {
                    if (dir.x > 0) targetState = DoorState.DoorRight;
                    else targetState = DoorState.DoorLeft;
                    break;
                }
        }

        if (!isRight)
        {
            if (targetState == DoorState.DoorLeft) targetState = DoorState.DoorRight;
            else if(targetState == DoorState.DoorRight) targetState = DoorState.DoorLeft;
        }

        return targetState;
    }
    private void DoorToggleRequest(uint doorId, DoorState doorState)
    {
        if (DoorType == EDoorType.Start && !GameManager.Instance.isStart){
            NetworkManager.Instance.StartStageRequest();
            GameManager.Instance.isStart = true;
        }
        GamePacket packet = new GamePacket();
        packet.DoorToggleRequest = new C2S_DoorToggleRequest();
        packet.DoorToggleRequest.DoorId = doorId;
        packet.DoorToggleRequest.DoorState = doorState;
        GameServerSocketManager.Instance.Send(packet);
    }
    public void RemoteToggle(DoorState doorState)
    {
        float targetAngle = GetRemoteOpenTargetAngle(doorState);
        targetRotation = Quaternion.Euler(0, targetAngle, 0);
        // state 갱신 및 중복 요청 방지
        isMove = true;
        PlayDoorSfx(doorState);

        var obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.carving = (doorState != DoorState.DoorMiddle);
        }

        transform.DOLocalRotateQuaternion(targetRotation, duration).SetEase(ease).OnComplete(() =>
        {
            isMove = false;
            currentDoorState = doorState;
        });
    }
    private float GetRemoteOpenTargetAngle(DoorState doorState)
    {
        float targetAngle = 0;
        switch (doorState)
        {
            case DoorState.DoorLeft:
                targetAngle = -90;
                break;

            case DoorState.DoorRight:
                targetAngle = 90;
                break;

            case DoorState.DoorMiddle:
                targetAngle = 0;
                break;
        }
        return targetAngle;
    }
    #endregion
    #region Sound
    private void PlayDoorSfx(DoorState doorState)
    {
        if (isMainDoor)
        {
            SoundManager.Instance.PlayInGameSfx(EInGameSfx.MainDoor, transform.position);
            return;
        }

        switch (doorState)
        {
            case DoorState.DoorLeft:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.DoorOpen, transform.position);
                break;
            case DoorState.DoorRight:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.DoorOpen, transform.position);
                break;
            case DoorState.DoorMiddle:
                SoundManager.Instance.PlayInGameSfx(EInGameSfx.DoorClose, transform.position);
                break;
        }
    }
    #endregion
}