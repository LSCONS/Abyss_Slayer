using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerAnimationData PlayerAnimationData {  get; private set; }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        PlayerAnimationData = Resources.Load<PlayerAnimationData>("Player/PlayerAnimationData/PlayerAnimationData");
        PlayerAnimationData.Init();
    }
}
