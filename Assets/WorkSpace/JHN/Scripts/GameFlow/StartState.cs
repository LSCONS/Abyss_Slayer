using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StartState : BaseGameState
{
    public override UIType StateUIType => UIType.GamePlay;
    public override ESceneName SceneName => ESceneName.BattleScene;

    public override Task OnEnter()
    {
        return Task.CompletedTask;
    }

    public override Task OnExit()
    {
        return Task.CompletedTask;
    }

    public override Task OnRunnerEnter()
    {
        return Task.CompletedTask;
    }
}
