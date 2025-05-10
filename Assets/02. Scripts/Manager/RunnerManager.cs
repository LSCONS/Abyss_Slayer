using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerManager : Singleton<RunnerManager>
{
    private NetworkRunner mainRunner = null;
    public NetworkRunner MainRunner
    {
        get 
        {
            if(mainRunner == null) mainRunner = gameObject.AddComponent<NetworkRunner>();
            return mainRunner;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public NetworkRunner GetRunner()
    {
        return MainRunner;
    }
}
