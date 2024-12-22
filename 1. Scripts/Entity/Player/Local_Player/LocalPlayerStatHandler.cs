using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    public int Heart;
    public float MoveSpeed;
    public float Stamina;
    public float LookSensivity;
}

public class LocalPlayerStatHandler
{
    public PlayerStat BaseStat { get; private set; }
    public PlayerStat CurrentStat { get; private set; }
    public event Action<float> OnStaminaChangeEvent;

    private bool isDie;
    private PlayerBaseSO data;

    public void Init(PlayerBaseSO playerSO)
    {
        isDie = false;

        BaseStat = new PlayerStat();
        CurrentStat = new PlayerStat();
        data = playerSO;

        InitStat(BaseStat);
        InitStat(CurrentStat);

        GameManager.Instance.Player.NetworkHandler.LifeUpdateRequest();
    }

    private void InitStat(PlayerStat stat)
    {
        stat.MoveSpeed = data.MoveSpeed;
        stat.Stamina = data.Stamina;
        stat.LookSensivity = data.LookSensivity;
    }

    private void CallChangeStaminaEvent(float percentage)
    {
        OnStaminaChangeEvent?.Invoke(percentage);
    }

    public int GetCurHeart()
    {
        return CurrentStat.Heart;
    }

    public int GetMaxHeart()
    {
        return BaseStat.Heart;
    }

    public bool IsDie()
    {
        return isDie;
    }

    public void Walk()
    {
        CurrentStat.MoveSpeed = BaseStat.MoveSpeed;
    }

    public void Run()
    {
        if (!CanRun())
            return;
        CurrentStat.MoveSpeed = data.RunSpeed;
    }

    public void Stop()
    {
        CurrentStat.MoveSpeed = 0;
    }

    public void UseStamina()
    {
        CurrentStat.Stamina = Mathf.Max(CurrentStat.Stamina -= data.StaminaDrain, 0);
        CallChangeStaminaEvent(GetStaminaPercentage());
    }

    public void StaminaRecovery()
    {
        CurrentStat.Stamina = Mathf.Min(CurrentStat.Stamina += data.StaminaRecovery, BaseStat.Stamina);
        CallChangeStaminaEvent(GetStaminaPercentage());
    }

    public bool IsStaminaFull()
    {
        if(CurrentStat.Stamina >= BaseStat.Stamina)
            return true;
        else 
            return false;
    }

    public bool CanRun()
    {
        if (CurrentStat.Stamina >= data.StaminaDrain)
            return true;
        else
            return false;
    }

    public float GetStaminaPercentage()
    {
        return CurrentStat.Stamina / data.Stamina;
    }

    public void TakeDamageRequest(uint ghostId)
    {
        GameManager.Instance.Player.NetworkHandler.PlayerAttackedRequest(ghostId);
    }

    // PlayerLifeResponse와 연동
    public void TakeDamage(uint life, bool isAttacked)
    {
         CurrentStat.Heart = (int)life;

        if (CurrentStat.Heart <= 0)
        {
            isDie = true;
            GameManager.Instance.Player.EventHandler.CallDieEvent(GameManager.Instance.Player);
        }

        if (!isAttacked)
        {
            BaseStat.Heart = (int)life; // 초기화
            UIManager.Instance.UI_HUD.UI_PlayerHealth.InitUI(CurrentStat.Heart);
            return;
        }

        UIManager.Instance.UI_HUD.UI_PlayerHealth.UpdatePlayerHealthUI(CurrentStat.Heart);
        UIManager.Instance.UI_HUD.UI_PlayerHit.OnHit();

        GameManager.Instance.Player.AnimationHandler.StartAnimTrigger(GameManager.Instance.Player.AnimationData.HitParameterHash, GameManager.Instance.Player);
        GameManager.Instance.Player.EventHandler.CallHitEvent();
    }

    public void SetLookSensivity(float value)
    {
        CurrentStat.LookSensivity = value;
    }
}