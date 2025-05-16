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

    /// <summary>
    /// 상태 즉시 변환
    /// </summary>
    /// <param name="newState">변경할 상태</param>
    /// <returns></returns>
    protected Task ChangeState(IGameState newState)
    {
        GameFlowManager.Instance.ChangeState(newState);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 일정 시간 후 상태 변환
    /// </summary>
    protected async Task ChangeStateWithDelay(IGameState newState, int msDelay)
    {
        await Task.Delay(msDelay);
        GameFlowManager.Instance.ChangeState(newState);
    }

    public virtual void OnUpdate()
    {
    }
}
