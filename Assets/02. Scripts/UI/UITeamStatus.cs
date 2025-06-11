using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UITeamStatus : UIPermanent
{
    public Dictionary<PlayerRef, UITeamStatusSlot> DictRefToSlot { get; private set; } = new();
    public override async void Init()
    {
#if AllMethodDebug
        Debug.Log("Init");
#endif
        base.Init();

        await ManagerHub.Instance.ServerManager.WaitForAllPlayerLoadingAsync();
        foreach(PlayerRef playerRef in ManagerHub.Instance.ServerManager.DictRefToPlayer.Keys)
        {
            if ((ManagerHub.Instance.ServerManager.ThisPlayerRef != playerRef) && !(DictRefToSlot.ContainsKey(playerRef)))
            {
                DictRefToSlot[playerRef] = Instantiate(ManagerHub.Instance.DataManager.PlayerStatusPrefab, transform);
                DictRefToSlot[playerRef].SlotPlayerRef = playerRef;
                DictRefToSlot[playerRef].ChagneInRestText();
                DictRefToSlot[playerRef].ConnectUIHpBar();
                DictRefToSlot[playerRef].SetIcon(ManagerHub.Instance.ServerManager.DictRefToNetData[playerRef].Class);
            }
        }
        gameObject.SetActive(true);
        ManagerHub.Instance.UIConnectManager.UITeamStatus = this;
        ManagerHub.Instance.UIManager.ResetAllRectTransform();
    }


    /// <summary>
    /// 플레이어의 준비 상태를 바꿔주는 메서드
    /// </summary>
    /// <param name="playerRef">바꿔줄 플레이어</param>
    /// <param name="isReady">바꿔줄 준비 상태</param>
    public void ChangeIsReadyPlayerText(PlayerRef playerRef, bool isReady)
    {
#if AllMethodDebug
        Debug.Log("ChangeIsReadyPlayerText");
#endif
        DictRefToSlot[playerRef].ChagnePlayerReadyText(isReady);
    }


    /// <summary>
    /// RestScene으로 갔을 때 팀원의 정보 Text를 맞춰서 변환해주는 메서드
    /// </summary>
    public void ChagneInRestSceneText()
    {
#if AllMethodDebug
        Debug.Log("ChagneInRestSceneText");
#endif
        foreach (PlayerRef playerRef in ManagerHub.Instance.ServerManager.DictRefToPlayer.Keys)
        {
            if ((ManagerHub.Instance.ServerManager.ThisPlayerRef != playerRef) && DictRefToSlot.ContainsKey(playerRef))
            {
                DictRefToSlot[playerRef].ChagneInRestText();
            }
        }
    }


    /// <summary>
    /// BattleScene으로 갔을 때 팀원의 정보 Text를 맞춰서 변환해주는 메서드
    /// </summary>
    public void ChagneInBattleScenePlayerText()
    {
#if AllMethodDebug
        Debug.Log("ChagneInBattleScenePlayerText");
#endif
        foreach (PlayerRef playerRef in ManagerHub.Instance.ServerManager.DictRefToPlayer.Keys)
        {
            if ((ManagerHub.Instance.ServerManager.ThisPlayerRef != playerRef) && DictRefToSlot.ContainsKey(playerRef))
            {
                DictRefToSlot[playerRef].ChangeBattleSceneText();
            }
        }
    }
}
