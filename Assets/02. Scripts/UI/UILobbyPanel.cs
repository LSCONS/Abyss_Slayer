using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILobbyPanel : UIPermanent
{
    [field: SerializeField] public TextMeshProUGUI TextRoomName { get; private set; }
    [field: SerializeField] public PlayerRoomData ThisPlayerData { get; private set; }
    [field: SerializeField] public List<PlayerRoomData> OtherPlayerData { get; private set; } = new();
}

[Serializable]
public class PlayerRoomData
{
    [field: SerializeField] public TextMeshProUGUI PlayerName { get; private set; }
    [field: SerializeField] public SpriteImageChange PlayerSpriteChange { get; private set; }
}


