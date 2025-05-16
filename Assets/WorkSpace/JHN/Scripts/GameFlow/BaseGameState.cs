using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface IGameState
{
    Task OnEnter();
    Task OnExit();
    Task OnRunnerEnter();
}

public abstract class BaseGameState : IGameState
{
    public abstract UIType StateUIType { get; } // 각 상태에서 쓸 uitype을 설정
    public abstract Task OnEnter();
    public abstract Task OnExit();
    public abstract Task OnRunnerEnter();
    public virtual void OnUpdate()
    {
    }
}
