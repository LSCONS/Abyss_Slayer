using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITeamStatusSlot : MonoBehaviour
{
    [field: SerializeField] public Image ImgPlayerIcon { get;private set; }
    [field: SerializeField] public Image ImgPalyerHP { get; private set; }
    [field: SerializeField] public Image classIcon { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextPlayerName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextReadyPlayer { get; private set; }
    [field: SerializeField] public UIHealthBar UIHealthBar { get; private set; }
    public string OnReadyText { get; private set; } = "[준비 완료]";
    public string OffReadyText { get; private set; } = "[준비 중...]";
    public string InGameText { get; private set; } = "[게임 중...]";
    public string ServerReadyText { get; private set; } = "[방장]";
    public PlayerRef SlotPlayerRef { get; set; }


    /// <summary>
    /// 특정 플레이어와 HpBar를 연결해주는 메서드
    /// </summary>
    public void ConnectUIHpBar()
    {
#if AllMethodDebug
        Debug.Log("ConnectUIHpBar");
#endif
        UIHealthBar.ConnectOtherPlayerObject(SlotPlayerRef);
    }


    /// <summary>
    /// RestScene에서 사용할 Text로 바꿔주는 메서드
    /// </summary>
    public void ChagneInRestText()
    {
#if AllMethodDebug
        Debug.Log("ChagneInRestText");
#endif
        TextPlayerName.text = ManagerHub.Instance.ServerManager.DictRefToNetData[SlotPlayerRef].GetName();
        if (ManagerHub.Instance.ServerManager.DictRefToNetData[SlotPlayerRef].IsServer)
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


    /// <summary>
    /// 플레이어의 준비 상태에 따라 Text를 바꿔주는 메서드
    /// </summary>
    /// <param name="isReady">바꿔 줄 준비 상태</param>
    public void ChagnePlayerReadyText(bool isReady)
    {
#if AllMethodDebug
        Debug.Log("ChagnePlayerReadyText");
#endif
        TextReadyPlayer.text = isReady ? OnReadyText : OffReadyText;
        TextReadyPlayer.color = isReady ? Color.red : Color.white;
    }


    /// <summary>
    /// BattleScene에서 사용할 Text로 바꿔줄 메서드
    /// </summary>
    public void ChangeBattleSceneText()
    {
#if AllMethodDebug
        Debug.Log("ChangeBattleSceneText");
#endif
        TextReadyPlayer.text = InGameText;
        TextReadyPlayer.color = Color.yellow;
    }

    public void SetIcon(CharacterClass cls)
    {
#if AllMethodDebug
        Debug.Log("SetIcon");
#endif
        if (ManagerHub.Instance.DataManager.DictClassToCharacterData.TryGetValue(cls, out CharacterClassData data))
        {
            classIcon.sprite = data.IconClass;
        }
    }
}
