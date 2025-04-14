using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class LoadingState : BaseGameState
{
    private string targetScene;
    private List<string> preLoadKey;
    public LoadingState(string targetScene, List<string> preLoadKey)
    {
        this.targetScene = targetScene;
        this.preLoadKey = preLoadKey;
    }
    public override async Task OnEnter()
    {
        Debug.Log("LoadingState OnEnter");
        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        Debug.Log("LoadingState OnExit");
        await Task.CompletedTask;
    }
    
}
