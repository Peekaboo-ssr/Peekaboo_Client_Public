using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.Events;

public class GhostEventHandler : MonoBehaviour
{
    private Ghost _ghost;

    public UnityAction<Player> OnSeeEvent;
    public UnityAction<Player> OnNotSeeEvent;

    public void Init(Ghost ghost)
    {
        _ghost = ghost;
        InitSpeechDetection();
    }

    #region 소리 감지
    private void InitSpeechDetection()
    {
        foreach (var participantEntry in VivoxManager.Instance.participants)
        {
            var participant = participantEntry.Value.Item1;
            var player = participantEntry.Value.Item2;

            SubscribeToSpeechDetection(participant, player);
        }
    }

    private void SubscribeToSpeechDetection(VivoxParticipant participant, Player player)
    {
        participant.ParticipantSpeechDetected += () =>
        {
            if (participant.AudioEnergy >= 0.3f)
            {
                OnSpeechDetected(player);
            }
        };
    }

    /// <summary>
    /// 소리감지
    /// </summary>
    /// <param name="participant"></param>
    private void OnSpeechDetected(Player player)
    {
        if (_ghost.Target != null) return;
        if (player == null) return;

        float audibleDistance = VivoxManager.Instance.channel3DSetting.GetChannel3DProperties().AudibleDistance;
        float combinedDetection = _ghost.StatHandler.CurStat.Hearing + audibleDistance;

        if (player == null || this == null) return;
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer <= combinedDetection)
        {
            if (player == null) return;
            _ghost.TargetPos = player.transform.position;
            _ghost.IsPlayerInRange = true;
            return;
        }

        _ghost.IsPlayerInRange = false;
    }
    #endregion

    #region 처다보기 및 사운드 재생
    public void SeeGhost(Player player)
    {
        OnSeeEvent?.Invoke(player);
    }

    public void NotSeeGhost(Player player)
    {
        OnNotSeeEvent?.Invoke(player);
    }
    #endregion
}
