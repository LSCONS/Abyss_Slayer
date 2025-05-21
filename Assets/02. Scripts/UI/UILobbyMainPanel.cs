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
        ServerManager.Instance.LobbyMainPanel = this;
    }


    /// <summary>
    /// 방의 이름을 바꿔주는 메서드
    /// </summary>
    public void ChangeRoomText()
    {
        TextRoomName.text = ServerManager.Instance.RoomName;
        SetActiveFalseData();
    }


    public void AddDictRefToRoomData(PlayerRef playerRef)
    {
        if (playerRef == ServerManager.Instance.ThisPlayerRef)
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
        //서버에 연결되어 있는 데이터 순회
        foreach (var item in ServerManager.Instance.DictRefToNetData)
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
        AddDictRefToRoomData(playerRef);
        PlayerRoomData roomData = DictRefToRoomData[playerRef];
        DictRoomDataToNetworkData[roomData] = ServerManager.Instance.DictRefToNetData[playerRef];
        roomData.TextPlayerName.gameObject.SetActive(true);
        roomData.PlayerSpriteChange.gameObject.SetActive(true);
        roomData.TextPlayerState.gameObject.SetActive(true);
        roomData.TextPlayerName.text = ServerManager.Instance.DictRefToNetData[playerRef].GetName();

        NetworkData data = DictRoomDataToNetworkData[roomData];
        roomData.PlayerSpriteChange.Init(data.Class, data.HairKey, data.SkinKey, data.FaceKey);
    }


    public void UpdateNameData(PlayerRef playerRef, string name)
    {
        PlayerRoomData roomData = DictRefToRoomData[playerRef];
        roomData.TextPlayerName.text = name;
    }


    /// <summary>
    /// 팀원 Sprite를 바꿔주는 메서드
    /// </summary>
    /// <param name="item"></param>
    private void SetChangeClassSubSprite(PlayerRef playerRef)
    {
        PlayerRoomData roomData = DictRefToRoomData[playerRef];
        NetworkData data = ServerManager.Instance.DictRefToNetData[playerRef];
        roomData.PlayerSpriteChange.Init(data.Class, data.HairKey, data.SkinKey, data.FaceKey);
    }


    /// <summary>
    /// Data 안에 null값이나 서버에서 사라진 데이터들을 비활성화 및 초기 클래스로 바꿔주는 메서드
    /// </summary>
    private void SetActiveFalseData()
    {
        foreach (PlayerRoomData item in OtherPlayerData)
        {
            item.TextPlayerName.gameObject.SetActive(false);
            item.PlayerSpriteChange.gameObject.SetActive(false);
            item.TextPlayerState.gameObject.SetActive(false);
            item.PlayerSpriteChange.Init(CharacterClass.Rogue, (1, 5), 1, 1);
            DictRoomDataToNetworkData[item] = null;
        }
    }


    /// <summary>
    /// 들어온 플레이어 정보를 기준으로 데이터를 삭제하는 메서드
    /// </summary>
    /// <param name="playerRef">삭제할 플레이어 정보</param>
    public void SetActiveFalseRef(PlayerRef playerRef)
    {
        PlayerRoomData data = DictRefToRoomData[playerRef];

        data.TextPlayerName.gameObject.SetActive(false);
        data.PlayerSpriteChange.gameObject.SetActive(false);
        data.TextPlayerState.gameObject.SetActive(false);
        data.PlayerSpriteChange.Init(CharacterClass.Rogue, (1, 5), 1, 1);
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
        var roomData = DictRefToRoomData[playerRef];
        TextMeshProUGUI textPlayerState = roomData.TextPlayerState;

        textPlayerState.text = isReady ? ReadyOnText : ReadyOffText;
        textPlayerState.color = isReady ? Color.red : Color.white;

        if (roomData.ChiefIcon != null) roomData.ChiefIcon.SetActive(false);
    }


    public void SetServerText(PlayerRef playerRef)
    {
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


