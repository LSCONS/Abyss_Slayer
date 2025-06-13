using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class RunnerManager : Singleton<RunnerManager>
{
    private NetworkRunner mainRunner = null;
    public NetworkRunner MainRunner
    {
        get 
        {
            if(mainRunner == null) 
            {
                mainRunner = gameObject.GetComponent<NetworkRunner>() 
                    ?? gameObject.AddComponent<NetworkRunner>();
            }
            return mainRunner;
        }
    }
    public bool IsThisRunner
        => ManagerHub.Instance.ServerManager.ThisPlayerRef == MainRunner.LocalPlayer;

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
