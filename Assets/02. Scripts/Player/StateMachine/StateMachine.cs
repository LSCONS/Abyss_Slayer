public interface IPlayerState
{
    public void Enter();

    public void Exit();

    public void Update();

    public void FixedUpdate();
}


public abstract class StateMachine
{
    public IPlayerState currentState;

    public virtual void ChangeState(IPlayerState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
}
