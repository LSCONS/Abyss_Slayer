using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerHub : Singleton<ManagerHub>
{
    [field: SerializeField] public GameValueManager GameValueManager    { get; private set; }
    [field: SerializeField] public SoundManager     SoundManager        { get; private set; }
    public ServerManager        ServerManager       { get; private set; } = new();
    public UIConnectManager     UIConnectManager    { get; private set; } = new();
    public DataManager          DataManager         { get; private set; } = new();
    public AnalyticsManager     AnalyticsManager    { get; private set; } = new();
    public GameFlowManager      GameFlowManager     { get; private set; } = new();
    public UIManager            UIManager           { get; private set; } = new();
    

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        UIManager.Init();
        AnalyticsManager.Init();
        GameFlowManager.Init();
    }


    private void Update()
    {
        UIManager.Update();
        GameFlowManager.Update();
    }
}
