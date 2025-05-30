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
        base.Init();

        await ServerManager.Instance.WaitForAllPlayerLoadingAsync();
        foreach(PlayerRef playerRef in ServerManager.Instance.DictRefToPlayer.Keys)
        {
            if ((ServerManager.Instance.ThisPlayerRef != playerRef) && !(DictRefToSlot.ContainsKey(playerRef)))
            {
                DictRefToSlot[playerRef] = Instantiate(DataManager.Instance.PlayerStatusPrefab, transform);
                DictRefToSlot[playerRef].SlotPlayerRef = playerRef;
                DictRefToSlot[playerRef].ChagneInRestText();
                DictRefToSlot[playerRef].ConnectUIHpBar();
                DictRefToSlot[playerRef].SetIcon(ServerManager.Instance.DictRefToNetData[playerRef].Class);
            }
        }
        gameObject.SetActive(true);
        ServerManager.Instance.UITeamStatus = this;
        UIManager.Instance.ResetAllRectTransform();
    }


    /// <summary>
    /// 플레이어의 준비 상태를 바꿔주는 메서드
    /// </summary>
    /// <param name="playerRef">바꿔줄 플레이어</param>
    /// <param name="isReady">바꿔줄 준비 상태</param>
    public void ChangeIsReadyPlayerText(PlayerRef playerRef, bool isReady)
    {
        DictRefToSlot[playerRef].ChagnePlayerReadyText(isReady);
    }


    /// <summary>
    /// RestScene으로 갔을 때 팀원의 정보 Text를 맞춰서 변환해주는 메서드
    /// </summary>
    public void ChagneInRestSceneText()
    {
        foreach (PlayerRef playerRef in ServerManager.Instance.DictRefToPlayer.Keys)
        {
            if ((ServerManager.Instance.ThisPlayerRef != playerRef) && DictRefToSlot.ContainsKey(playerRef))
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
        foreach (PlayerRef playerRef in ServerManager.Instance.DictRefToPlayer.Keys)
        {
            if ((ServerManager.Instance.ThisPlayerRef != playerRef) && DictRefToSlot.ContainsKey(playerRef))
            {
                DictRefToSlot[playerRef].ChangeBattleSceneText();
            }
        }
    }
}
