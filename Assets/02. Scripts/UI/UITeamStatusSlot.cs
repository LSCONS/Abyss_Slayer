using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITeamStatusSlot : MonoBehaviour
{
    [field: SerializeField] public Image ImgPlayerIcon { get;private set; }
    [field: SerializeField] public TextMeshProUGUI TextPlayerName { get; private set; }
    [field: SerializeField] public Image ImgPalyerHP { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextReadyPlayer { get; private set; }
    [field: SerializeField] public UIHealthBar UIHealthBar { get; private set; }
    [field:SerializeField] public Image classIcon { get; private set; }
    public string OnReadyText { get; private set; } = "[준비 완료]";
    public string OffReadyText { get; private set; } = "[준비 중...]";
    public string InGameText { get; private set; } = "[게임 중...]";
    public string ServerReadyText { get; private set; } = "[방장]";
    public PlayerRef playerRef { get; set; }

    public void ConnectUIHpBar()
    {
        UIHealthBar.ConnectOtherPlayerObject(playerRef);
    }

    public void ChagneInRestText()
    {
        TextPlayerName.text = ServerManager.Instance.DictRefToNetData[playerRef].GetName();
        if (ServerManager.Instance.DictRefToNetData[playerRef].IsServer)
        {
            TextReadyPlayer.text = ServerReadyText;
            TextReadyPlayer.color = Color.red;
        }
        else
        {
            TextReadyPlayer.text = OffReadyText;
            TextReadyPlayer.color = Color.white;
        }
    }

    public void ChagnePlayerReadyText(bool isReady)
    {
        TextReadyPlayer.text = isReady ? OnReadyText : OffReadyText;
        TextReadyPlayer.color = isReady ? Color.red : Color.white;
    }

    public void ChangeInGameText()
    {
        TextReadyPlayer.text = InGameText;
        TextReadyPlayer.color = Color.yellow;
    }

    public void SetIcon(CharacterClass cls)
    {
        if (DataManager.Instance.DictClassToImage.TryGetValue(cls, out var icon))
        {
            classIcon.sprite = icon;
        }
    }
}
