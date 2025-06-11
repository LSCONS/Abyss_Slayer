using Fusion;
using Photon.Realtime;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

/// <summary>
/// 방에 접속한 플레이어의 데이터를 저장해두는 오브젝트.
/// </summary>
[RequireComponent(typeof(NetworkObject))]
public class NetworkData : NetworkBehaviour
{
    [Networked, Capacity(24)] public NetworkArray<byte> NameBuffer { get; } //플레이어 이름
    [Networked] public PlayerRef PlayerDataRef { get; set; }    //플레이어 정보
    [Networked] public bool IsServer { get; set; } = false; //플레이어의 서버 권한 여부
    [Networked] public bool IsReady { get; set; } = false; //플레이어 레디 여부
    [Networked] public int IntPlayerClass { get; set; } = (int)CharacterClass.Rogue; //플레이어 직업
    [Networked] public int HairStyleKey { get; set; } = 1;
    public int HairColorKey => HairColorConfig.HairColorIndexByClass[Class];
    [Networked] public int FaceKey { get; set; } = 1;
    [Networked] public int SkinKey { get; set; } = 1;
    public (int, int) HairKey => (HairStyleKey, HairColorKey);
    public int PointStatus { get; set; } = 1;
    public int PointSkill { get; set; } = 1;
    public CharacterClass Class => (CharacterClass)IntPlayerClass;

    public override void Spawned()
    {
#if AllMethodDebug
        Debug.Log("Spawned");
#endif
        base.Spawned();

        //위치를 DontDestroyOnLoad로 고정
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);


        if (Runner.LocalPlayer == PlayerDataRef)
        {
            ManagerHub.Instance.ServerManager.ThisPlayerRef = PlayerDataRef;
        }
        ManagerHub.Instance.ServerManager.DictRefToNetData[PlayerDataRef] = this;
        try
        {
            ManagerHub.Instance.UIConnectManager.UILobbyMainPanel?.UpdateNewData(PlayerDataRef);
        }
        catch { }

        if (IsServer) ManagerHub.Instance.UIConnectManager.UILobbyMainPanel?.SetServerText(PlayerDataRef);

        //서버에 본인 데이터와 이름을 등록
        if (Runner.LocalPlayer == PlayerDataRef)
        {
            Rpc_RequestSetName(ManagerHub.Instance.ServerManager.PlayerNameBytes);
        }

        try
        {
            ManagerHub.Instance.UIConnectManager.UILobbyMainPanel?.UIUpdateSprite();
        }
        catch { }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
#if AllMethodDebug
        Debug.Log("Despawned");
#endif
        try
        {
            ManagerHub.Instance.UIConnectManager.UILobbyMainPanel.SetActiveFalseRef(PlayerDataRef);
        }
        catch { }
        try
        {
            PlayerEixtRoomText();
        }catch { }
        base.Despawned(runner, hasState);
    }


    /// <summary>
    /// 플레이어의 접속 텍스트를 모두에게 보내는 메서드
    /// </summary>
    private void PlayerEnterRoomText()
    {
#if AllMethodDebug
        Debug.Log("PlayerEnterRoomText");
#endif
        string enterText = $"\"{GetName()}\"님이 접속하셨습니다.\n";
        ManagerHub.Instance.UIConnectManager.UIChatController?.SendChatMessage(enterText.StringToBytes());
    }


    /// <summary>
    /// 플레이어의 퇴장 텍스트를 모두에게 보내는 메서드
    /// </summary>
    private void PlayerEixtRoomText()
    {
#if AllMethodDebug
        Debug.Log("PlayerEixtRoomText");
#endif
        string exitText = $"\"{GetName()}\"님이 접속을 종료했습니다.\n";
        ManagerHub.Instance.UIConnectManager.UIChatController?.SendChatMessage(exitText.StringToBytes());
    }


    /// <summary>
    /// 해당 데이터애 들어있는 이름을 string으로 반환해주는 메서드
    /// </summary>
    /// <returns>이름 string으로 반환</returns>
    public string GetName()
    {
#if AllMethodDebug
        Debug.Log("GetName");
#endif
        var bytes = new System.Collections.Generic.List<byte>();
        foreach (var b in NameBuffer)
        {
            if (b == 0) break;
            bytes.Add(b);
        }
        return Encoding.UTF8.GetString(bytes.ToArray());
    }


    /// <summary>
    /// 서버에게 이름 데이터 저장을 요청
    /// </summary>
    /// <param name="nameBytes"></param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RequestSetName(byte[] nameBytes)
    {
#if AllMethodDebug
        Debug.Log("Rpc_RequestSetName");
#endif
        // 2) NameBuffer 에 한 번에 채워 넣기
        for (int i = 0; i < NameBuffer.Length; i++)
            NameBuffer.Set(i, i < nameBytes.Length ? nameBytes[i] : (byte)0);

        ManagerHub.Instance.UIConnectManager.UILobbyMainPanel?.UpdateNameData(PlayerDataRef, nameBytes.BytesToString());
        try
        {
            PlayerEnterRoomText();
        }
        catch { }
    }


    /// <summary>
    /// 입력받은 Class를 기준으로 플레이어 Sprite를 변경
    /// </summary>
    /// <param name="characterClass">변경할 직업 Class</param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ChangeClass(CharacterClass characterClass)
    {
#if AllMethodDebug
        Debug.Log("Rpc_ChangeClass");
#endif
        IntPlayerClass = (int)characterClass;
        ManagerHub.Instance.UIConnectManager.UILobbyMainPanel.UIUpdateSprite();
    }


    /// <summary>
    /// 모든 플레이어들에게 채팅창에 메시지를 보내는 메서드
    /// </summary>
    /// <param name="bytes"></param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_EnterToChatting(byte[] bytes)
    {
#if AllMethodDebug
        Debug.Log("Rpc_EnterToChatting");
#endif
        ManagerHub.Instance.UIConnectManager.UIChatController.SendChatMessage(bytes);
    }


    /// <summary>
    /// 해당 Ref의 포함 여부를 확인 후, 포함되어있다면 삭제 / 없다면 추가를 진행.
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_ClickReadyBtn()
    {
#if AllMethodDebug
        Debug.Log("Rpc_ClickReadyBtn");
#endif
        IsReady = !IsReady;
        Rpc_SetReadyText(IsReady);
        ManagerHub.Instance.UIConnectManager.UILobbySelectPanel.CheckAllPlayerIsReady();
    }


    /// <summary>
    /// MainPanel에 있는 ReadyText를 활성화 혹은 비활성화 시키는 메서드
    /// </summary>
    /// <param name="isActive"></param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetReadyText(bool isActive)
    {
#if AllMethodDebug
        Debug.Log("Rpc_SetReadyText");
#endif
        ManagerHub.Instance.UIConnectManager.UILobbyMainPanel.SetReadyText(PlayerDataRef, isActive);
    }


    /// <summary>
    /// 씬을 이동할 때 사용할 메서드
    /// </summary>
    /// <param name="enumScene"></param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_MoveScene(ESceneName enumScene)
    {
#if AllMethodDebug
        Debug.Log("Rpc_MoveScene");
#endif
        foreach (var item in ManagerHub.Instance.ServerManager.DictRefToPlayer.Values)
        {
            item.ResetPlayerStatus();
        }
        ManagerHub.Instance.GameFlowManager.RpcServerSceneLoad(enumScene);
    }


    /// <summary>
    /// 플레이어들의 입력을 활성화 시킬 메서드
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ConnectInput()
    {
#if AllMethodDebug
        Debug.Log("Rpc_ConnectInput");
#endif
        ManagerHub.Instance.ServerManager.PlayerInput.InputEvent();
    }


    /// <summary>
    /// 플레이어들의 입력을 비활성화 시킬 메서드
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_DisconnectInput()
    {
#if AllMethodDebug
        Debug.Log("Rpc_DisconnectInput");
#endif
        ManagerHub.Instance.ServerManager.PlayerInput.OutPutEvent();
    }


    /// <summary>
    /// 모든 플레이어 활성화
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_PlayerActiveTrue()
    {
#if AllMethodDebug
        Debug.Log("Rpc_PlayerActiveTrue");
#endif
        StartCoroutine(ActivePlayer());
    }

    private IEnumerator ActivePlayer()
    {
#if AllMethodDebug
        Debug.Log("ActivePlayer");
#endif
        while (ManagerHub.Instance.ServerManager.DictRefToPlayer.Values.Count != Runner.SessionInfo.PlayerCount)
        {
            yield return new WaitForSeconds(0.1f);
        }

        foreach (Player player in ManagerHub.Instance.ServerManager.DictRefToPlayer.Values)
        {
            SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetActiveScene());
            player.PlayerData.PlayerDataInit(player);
        }

        foreach (NetworkData data in ManagerHub.Instance.ServerManager.DictRefToNetData.Values)
        {
            SceneManager.MoveGameObjectToScene(data.gameObject, SceneManager.GetActiveScene());
        }
        SceneManager.MoveGameObjectToScene(RunnerManager.Instance.gameObject, SceneManager.GetActiveScene());
    }


    /// <summary>
    /// 클라이언트들이 래디 버튼을 누를 경우 실행할 메서드
    /// </summary>
    /// <param name="isReady"></param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_PlayerIsReady(bool isReady)
    {
#if AllMethodDebug
        Debug.Log("Rpc_PlayerIsReady");
#endif
        if (Runner.LocalPlayer == PlayerDataRef) return;

        if (Runner.IsServer)
        {
            IsReady = isReady;
            bool isAllReday = ManagerHub.Instance.ServerManager.CheckAllPlayerIsReadyInServer();
            ManagerHub.Instance.ServerManager.IsAllReadyAction?.Invoke(isAllReday);
        }
        ManagerHub.Instance.UIConnectManager.UITeamStatus?.ChangeIsReadyPlayerText(PlayerDataRef, isReady);
    }


    /// <summary>
    /// 플레이어의 준비 상태를 공유하는 메서드
    /// </summary>
    /// <param name="isReady"></param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetReady(bool isReady)
    {
#if AllMethodDebug
        Debug.Log("Rpc_SetReady");
#endif
        IsReady = isReady;
    }


    /// <summary>
    /// 플레이어의 포지션을 특정 위치로 고정되게 공유하는 메서드
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ResetPlayerPosition()
    {
#if AllMethodDebug
        Debug.Log("Rpc_ResetPlayerPosition");
#endif
        Player player = ManagerHub.Instance.ServerManager.DictRefToPlayer[PlayerDataRef];
        if((Vector2)player.transform.position != player.PlayerPosition)
        {
            player.transform.position = (Vector3)player.PlayerPosition;
        }
    }


    /// <summary>
    /// 플레이어가 선택한 커스텀 번호를 공유할 메서드
    /// </summary>
    /// <param name="hairStyle">공유할 머리</param>
    /// <param name="skin">공유할 피부</param>
    /// <param name="face">공유할 머리</param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_InitPlayerCustom(int hairStyle, int skin, int face)
    {
#if AllMethodDebug
        Debug.Log("Rpc_InitPlayerCustom");
#endif
        HairStyleKey = hairStyle;
        SkinKey = skin;
        FaceKey = face;
        ManagerHub.Instance.UIConnectManager.UILobbyMainPanel.UIUpdateSprite();
    }


    /// <summary>
    /// VirtualCamera의 움직임을 공유할 메서드
    /// </summary>
    /// <param name="size">카메라 줌 사이즈</param>
    /// <param name="priority"></param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_VirtualCamera(float size, int priority)
    {
#if AllMethodDebug
        Debug.Log("Rpc_VirtualCamera");
#endif
        ManagerHub.Instance.ServerManager.Boss.BossController.VirtualCamera.m_Lens.OrthographicSize = size;
        ManagerHub.Instance.ServerManager.Boss.BossController.VirtualCamera.Priority = priority;
    }


    /// <summary>
    /// LobbyState에서 선택한 레벨의 정보를 공유할 메서드
    /// </summary>
    /// <param name="level"></param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_LobbySelectLevelUpdateUI(int level)
    {
#if AllMethodDebug
        Debug.Log("Rpc_LobbySelectLevelUpdateUI");
#endif
        ManagerHub.Instance.UIConnectManager.UILobbySelectPanel.UpdateUI(level);
    }


    /// <summary>
    /// 플레이어의 무적 상태를 공유할 메서드
    /// </summary>
    /// <param name="isInvincibility">무적 유무</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetInvincibilityAllPlayer(bool isInvincibility)
    {
#if AllMethodDebug
        Debug.Log("Rpc_SetInvincibilityAllPlayer");
#endif
        foreach (Player player in ManagerHub.Instance.ServerManager.DictRefToPlayer.Values)
        {
            player.Invincibility = isInvincibility;
        }
    }


    /// <summary>
    /// 게임에 적용시킨 레벨을 공유하는 메서드
    /// </summary>
    /// <param name="gameLevel">공유할 게임 레벨</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetGameLevel(int gameLevel)
    {
#if AllMethodDebug
        Debug.Log("Rpc_SetGameLevel");
#endif
        ManagerHub.Instance.GameValueManager.SetEGameLevel(gameLevel);
    }

     
    /// <summary>
    /// RestState에서 준비/시작의 버튼을 리셋해주는 메서드
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ResetRestButton()
    {
#if AllMethodDebug
        Debug.Log("Rpc_ResetRestButton");
#endif
        ManagerHub.Instance.UIConnectManager.UIReadyBossStage.ResetClientButton();
    }


    /// <summary>
    /// BattleState를 기준으로 TeamText를 변경해주는 메서드
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetBattleTeamText()
    {
#if AllMethodDebug
        Debug.Log("Rpc_SetInGameTeamText");
#endif
        ManagerHub.Instance.UIConnectManager.UITeamStatus?.ChagneInBattleScenePlayerText();
    }


    /// <summary>
    /// RestState의 기준으로 TeamText를 변경해주는 메서드
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetInRestTeamText()
    {
#if AllMethodDebug
        Debug.Log("Rpc_SetInRestTeamText");
#endif
        ManagerHub.Instance.UIConnectManager.UITeamStatus?.ChagneInRestSceneText();
    }


    /// <summary>
    /// 스킬 업그레이드 적용 공유
    /// </summary>
    /// <param name="playerRef">스킬 업그레이드를 적용하고 싶은 플레이어</param>
    /// <param name="slotKeyInt">적용시킬 스킬 슬롯</param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ApplySkillUpgrade(PlayerRef playerRef, int slotKeyInt)
    {
        Player player = ManagerHub.Instance.ServerManager.DictRefToPlayer[playerRef];
        Skill skill = player.DictSlotKeyToSkill[(SkillSlotKey)slotKeyInt];
        skill.SkillUpgrade();
    }


    /// <summary>
    /// 여는 팝업 창 공유
    /// </summary>
    /// <param name="popupName">열고 싶은 팝업 창 이름</param>
    /// <param name="descriptionText">바꾸고 싶은 설명 Text</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_OpenPopup(byte[] popupName, byte[] descriptionText)
    {
        ManagerHub.Instance.UIManager.OpenPopup(popupName.BytesToString(), descriptionText.BytesToString());
    }


    /// <summary>
    /// 닫는 팝업 창 공유
    /// </summary>
    /// <param name="popupName">닫을 팝업 창 이름</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ClosePopup(byte[] popupName)
    {
        ManagerHub.Instance.UIManager.ClosePopup(popupName.BytesToString());
    }

    /// <summary>
    /// 보스 처치 후 늘어나는 스킬 포인트 공유
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_AddSkillPoint()
    {
        ManagerHub.Instance.ServerManager.ThisPlayer.AddSkillPoint(ManagerHub.Instance.GameValueManager.AddSkillPointValue);
        ManagerHub.Instance.ServerManager.ThisPlayer.AddStatusPoint(ManagerHub.Instance.GameValueManager.AddStatusPointValue);
    }


    /// <summary>
    /// 마지막 보스 처치 후 폭죽 이벤트를 공유
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetActiveTrueFireWorks()
    {
        ManagerHub.Instance.ServerManager.fireworks = GameObject.Instantiate(ManagerHub.Instance.DataManager.FireworksPrefab); // 마지막 보스 잡으면 불꽃놀이 실행
        ManagerHub.Instance.ServerManager.fireworks.StartFireworks();
    }


    /// <summary>
    /// 보스에게 걸린 버프에 대한 정보를 공유
    /// </summary>
    /// <param name="buffTypeInt"></param>
    /// <param name="debuffDataName"></param>
    /// <param name="debuffDataDescription"></param>
    /// <param name="debuffDataDuration"></param>
    /// <param name="debuffDataStartTime"></param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_CreateBossBuffSlot(int buffTypeInt, byte[] debuffDataName, byte[] debuffDataDescription, float debuffDataDuration)
    {
        UIBossBuffSlotManager.Instance.CreateSlot(buffTypeInt, debuffDataName.BytesToString(), debuffDataDescription.BytesToString(), debuffDataDuration);
    }


    /// <summary>
    /// 멀티로 클리어한 클리어 타임을 공유하는 메서드
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_MultiClearTime(byte[] clearTime, int playerCount)
    {
        string temp = $"[{ManagerHub.Instance.ServerManager.PlayerName}] 클리어 인원: {playerCount}명\n{clearTime.BytesToString()}";
        PlayerPrefs.SetString(PlayerPrefabData.MultiClearTime, temp);
        ManagerHub.Instance.UIConnectManager.UIStartTitle?.MultiClearTimeUpdate();
    }
}
