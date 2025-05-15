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
    public string OnReadyText { get; private set; } = "[준비 완료]";
    public string OffReadyText { get; private set; } = "[준비 중...]";
    public string ServerReadyText { get; private set; } = "[방장]";
    public PlayerRef playerRef { get; set; }

    public void Init()
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
}
