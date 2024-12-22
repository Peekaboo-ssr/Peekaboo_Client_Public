public class EntityBaseState : IState
{
    protected EntityStateMachine entityStateMachine;

    public EntityBaseState(EntityStateMachine stateMachine)
    {
        entityStateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }
    public virtual void FixedUpdate()
    {

    }

    public virtual void Update()
    {

    }
    protected void StartAnimation(int animatorHash)
    {
        entityStateMachine.Entity.Animator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        entityStateMachine.Entity.Animator.SetBool(animatorHash, false);
    }

    protected void StartAnimTrigger(int animatorHash)
    {
        entityStateMachine.Entity.Animator.SetTrigger(animatorHash);
    }
}