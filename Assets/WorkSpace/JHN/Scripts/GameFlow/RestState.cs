using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class RestState : BaseGameState
{
    private int stageIndex;
    public RestState(int stageIndex)
    {
        this.stageIndex = stageIndex;   
    }
    public override async Task OnEnter()
    {
        Debug.Log("RestState OnEnter");
        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        Debug.Log("RestState OnExit");
        await Task.CompletedTask;
    }
}
