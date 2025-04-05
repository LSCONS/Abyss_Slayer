using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public interface IPlayerState
{
    public void Enter();

    public void Exit();

    public void HandleInput();

    public void Update();

    public void FixedUpdate();
}


public abstract class StateMachine : MonoBehaviour
{
    protected IPlayerState currentState;

    public void ChangeState(IPlayerState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void HandleInput()
    {
        currentState?.HandleInput();
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
