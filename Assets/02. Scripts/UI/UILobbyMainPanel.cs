using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UILobbyMainPanel : UIPermanent
{
    [field: SerializeField] public TextMeshProUGUI TextRoomName { get; private set; }
    [field: SerializeField] public PlayerRoomData ThisPlayerData { get; private set; }
    [field: SerializeField] public List<PlayerRoomData> OtherPlayerData { get; private set; } = new();
    public Dictionary<PlayerRoomData, NetworkData> DictRoomDataToNetworkData { get; private set; } = new();
    public Dictionary<PlayerRef, PlayerRoomData> DictRefToRoomData { get; private set; } = new();
    public string ReadyOnText { get; private set; } = "[준비 완료]";
    public string ReadyOffText { get; private set; } = "[준비 중...]";
    public string ServerText { get; private set; } = "[방장]";

    private void Awake()
    {
#if AllMethodDebug
        Debug.Log("Awake");
#endif
        ManagerHub.Instance.UIConnectManager.UILobbyMainPanel = this;
    }


    /// <summary>
    /// 방의 이름을 바꿔주는 메서드
    /// </summary>
    public void ChangeRoomText()
    {
#if AllMethodDebug
        Debug.Log("ChangeRoomText");
#endif
        TextRoomName.text = ManagerHub.Instance.ServerManager.RoomName;
        SetActiveFalseData();
    }


    public void AddDictRefToRoomData(PlayerRef playerRef)
    {
#if AllMethodDebug
        Debug.Log("AddDictRefToRoomData");
#endif
        if (playerRef == ManagerHub.Instance.ServerManager.ThisPlayerRef)
        {
            DictRefToRoomData[playerRef] = ThisPlayerData;
            return;
        }

        foreach (PlayerRoomData item in OtherPlayerData)
        {
            if (DictRoomDataToNetworkData[item] == null)
            {
                DictRefToRoomData[playerRef] = item;
                break;
            }
        }
    }


    /// <summary>
    /// 서버에 연결되어있는 데이터를 순회하며 SpriteUI를 업데이트 해주는 메서드
    /// </summary>
    public void UIUpdateSprite()
    {
#if AllMethodDebug
        Debug.Log("UIUpdateSprite");
#endif
        //서버에 연결되어 있는 데이터 순회
        foreach (var item in ManagerHub.Instance.ServerManager.DictRefToNetData)
        {
            SetChangeClassSubSprite(item.Key);   //CharacterClass의 Sprite를 바꿔주는 메서드
        }
    }


    /// <summary>
    /// 갱신된 정보를 기준으로 Sprite를 바꿔주고 활성화 해주는 메서드
    /// </summary>
    /// <param name="item"></param>
    public void UpdateNewData(PlayerRef playerRef)
    {
#if AllMethodDebug
        Debug.Log("UpdateNewData");
#endif
        AddDictRefToRoomData(playerRef);
        PlayerRoomData roomData = DictRefToRoomData[playerRef];
        DictRoomDataToNetworkData[roomData] = ManagerHub.Instance.ServerManager.DictRefToNetData[playerRef];
        roomData.TextPlayerName.gameObject.SetActive(true);
        roomData.PlayerSpriteChange.gameObject.SetActive(true);
        roomData.TextPlayerState.gameObject.SetActive(true);
        roomData.TextPlayerName.text = ManagerHub.Instance.ServerManager.DictRefToNetData[playerRef].GetName();

        NetworkData data = DictRoomDataToNetworkData[roomData];
        roomData.PlayerSpriteChange.Init(data.Class, data.HairStyleKey, data.SkinKey, data.FaceKey);
    }


    public void UpdateNameData(PlayerRef playerRef, string name)
    {
#if AllMethodDebug
        Debug.Log("UpdateNameData");
#endif
        PlayerRoomData roomData = DictRefToRoomData[playerRef];
        roomData.TextPlayerName.text = name;
    }


    /// <summary>
    /// 팀원 Sprite를 바꿔주는 메서드
    /// </summary>
    /// <param name="item"></param>
    private void SetChangeClassSubSprite(PlayerRef playerRef)
    {
#if AllMethodDebug
        Debug.Log("SetChangeClassSubSprite");
#endif
        PlayerRoomData roomData = DictRefToRoomData[playerRef];
        NetworkData data = ManagerHub.Instance.ServerManager.DictRefToNetData[playerRef];
        roomData.PlayerSpriteChange.Init(data.Class, data.HairStyleKey, data.SkinKey, data.FaceKey);
    }


    /// <summary>
    /// Data 안에 null값이나 서버에서 사라진 데이터들을 비활성화 및 초기 클래스로 바꿔주는 메서드
    /// </summary>
    private void SetActiveFalseData()
    {
#if AllMethodDebug
        Debug.Log("SetActiveFalseData");
#endif
        foreach (PlayerRoomData item in OtherPlayerData)
        {
            item.TextPlayerName.gameObject.SetActive(false);
            item.PlayerSpriteChange.gameObject.SetActive(false);
            item.TextPlayerState.gameObject.SetActive(false);
            item.PlayerSpriteChange.Init(CharacterClass.Rogue, 1, 1, 1);
            DictRoomDataToNetworkData[item] = null;
        }
    }


    /// <summary>
    /// 들어온 플레이어 정보를 기준으로 데이터를 삭제하는 메서드
    /// </summary>
    /// <param name="playerRef">삭제할 플레이어 정보</param>
    public void SetActiveFalseRef(PlayerRef playerRef)
    {
#if AllMethodDebug
        Debug.Log("SetActiveFalseRef");
#endif
        PlayerRoomData data = DictRefToRoomData[playerRef];
        if (data == null) return;
        data.TextPlayerName?.gameObject.SetActive(false);
        data.PlayerSpriteChange?.gameObject.SetActive(false);
        data.TextPlayerState?.gameObject.SetActive(false);
        data.PlayerSpriteChange?.Init(CharacterClass.Rogue, 1, 1, 1);
        SetReadyText(playerRef, false);
        DictRoomDataToNetworkData[data] = null;
    }


    /// <summary>
    /// 매개변수로 받은 플레이어의 준비 상태를 받고 그에 따라 텍스트를 변환해주는 메서드
    /// </summary>
    /// <param name="playerRef">변경할 플레이어 정보</param>
    /// <param name="isReady">준비 상태</param>
    public void SetReadyText(PlayerRef playerRef, bool isReady)
    {
#if AllMethodDebug
        Debug.Log("SetReadyText");
#endif
        var roomData = DictRefToRoomData[playerRef];
        TextMeshProUGUI textPlayerState = roomData.TextPlayerState;

        textPlayerState.text = isReady ? ReadyOnText : ReadyOffText;
        textPlayerState.color = isReady ? Color.red : Color.white;

        if (roomData.ChiefIcon != null) roomData.ChiefIcon.SetActive(false);
    }


    public void SetServerText(PlayerRef playerRef)
    {
#if AllMethodDebug
        Debug.Log("SetServerText");
#endif
        var roomData = DictRefToRoomData[playerRef];

        TextMeshProUGUI textPlayerState = roomData.TextPlayerState;
        textPlayerState.text = ServerText;
        textPlayerState.color = Color.red;

        if(roomData.ChiefIcon!=null) roomData.ChiefIcon.SetActive(true);
    }
}

[Serializable]
public class PlayerRoomData
{
    [field: SerializeField] public TextMeshProUGUI TextPlayerState { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextPlayerName { get; private set; }
    [field: SerializeField] public SpriteImageChange PlayerSpriteChange { get; private set; }
    [field: SerializeField] public GameObject ChiefIcon { get; private set; }
}


