using System.Diagnostics;

public class EntityStateMachine
{
    public Entity Entity;
    public CharacterState CharacterState;
    public IState CurrentState;
    public IState BeforeState;

    public void ChangeState(IState state)
    {
        if (Entity as GhostA != null)
        {
            UnityEngine.Debug.Log($"Ghost{(Entity as GhostA).GhostId}의 현재 State: {state.GetType().Name}");
        }
        BeforeState = CurrentState;
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState?.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }

    public bool IsEntityState(IState state)
    {
        return CurrentState == state;
    }

    public void UpdateCharacterState(CharacterState characterState)
    {
        CharacterState = characterState;
    }
}