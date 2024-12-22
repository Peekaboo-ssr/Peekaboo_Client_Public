using Cysharp.Threading.Tasks;
using UnityEngine;
using AkiDevCat.AVL.Components;

public class GhostE : Ghost
{
    public Light eyeLight;                  // 몬스터의 눈에서 나오는 빛
    public VolumetricLight volumetricLight;
    private bool _isBlinking;               // 깜박임 루틴 실행 여부

    public override void Init(uint ghostId, bool isFail)
    {
        base.Init(ghostId, isFail);
        if (NetworkManager.Instance.IsHost || IsTest)
        {
            if (!isFail)
            {
                StateMachine = new GhostEStateMachine(this);
                IsDefeatable = false;
                StartBlinkRoutine();
            }
            else
            {
                StateMachine = new GhostEStateMachine(this);
                StateMachine.ChangeState(StateMachine.FailSeeionState);
            }
        }
    }

    /// <summary>
    /// 눈 깜박임 루틴 시작
    /// </summary>
    public void StartBlinkRoutine()
    {
        if (!_isBlinking)
        {
            _isBlinking = true;
            BlinkRoutine().Forget();
        }
    }

    /// <summary>
    /// 눈 깜박임 루틴
    /// </summary>
    private async UniTaskVoid BlinkRoutine()
    {
        while (_isBlinking)
        {
            if (Target == null)
            {
                await ChangeEyeState(false); // 눈 감기
                await ChangeEyeState(true);  // 눈 뜨기
            }
            else
            {
                await UniTask.Yield();
            }
        }
    }

    /// <summary>
    /// 눈 상태를 변경하고 랜덤 시간 대기
    /// </summary>
    /// <param name="isOpen">눈을 뜨면 true, 감으면 false</param>
    private async UniTask ChangeEyeState(bool isOpen)
    {
        SetEyesOpen(isOpen);
        float randomDuration = Random.Range(5f, 10f); // 5~10초 사이 랜덤 시간
        await UniTask.WaitForSeconds(randomDuration);
    }

    /// <summary>
    /// 눈 상태 설정
    /// </summary>
    /// <param name="open">눈을 뜨면 true, 감으면 false</param>
    private void SetEyesOpen(bool isOpen)
    {
        if(eyeLight != null && volumetricLight != null)
        {
            eyeLight.enabled = isOpen;
            volumetricLight.enabled = isOpen;
            SendEyePacket(isOpen);
        }

        if (Agent == null || StateMachine == null) return;
        if (isOpen)
        {
            Agent.isStopped = false;
            StateMachine.ChangeState(StateMachine.PatrolState);
        }
        else
        {
            Agent.isStopped = true;
            StateMachine.ChangeState((StateMachine as GhostEStateMachine).InactiveState);
        }
    }

    private void SendEyePacket(bool isOn)
    {
        GamePacket packet = new GamePacket();
        packet.GhostSpecialStateRequest = new C2S_GhostSpecialStateRequest();

        packet.GhostSpecialStateRequest.GhostId = GhostId;
        packet.GhostSpecialStateRequest.SpecialState = GhostSpecialState.EyeLight;
        packet.GhostSpecialStateRequest.IsOn = isOn;

        GameServerSocketManager.Instance.Send(packet);
    }

    public void OpenEyes()
    {
        eyeLight.enabled = true;
        volumetricLight.enabled = true;
    }

    public void CloseEyes()
    {
        eyeLight.enabled = false;
        volumetricLight.enabled = false;
    }

    private void OnDestroy()
    {
        _isBlinking = false;
    }
}
