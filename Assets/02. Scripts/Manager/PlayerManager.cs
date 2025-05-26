public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerCustomizationInfo PlayerCustomizationInfo { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
