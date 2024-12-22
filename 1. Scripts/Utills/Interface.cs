public interface IState
{
    public void Enter();
    public void Exit();
    public void FixedUpdate();
    public void Update();
}

public interface IAnimationEvent
{
    public void OnJumpStart();
    public void OnJumpEnd();
    public void OnHitStart();
    public void OnAttackStart();
    public void OnAnimationEnd();
    public void OnDie();
}