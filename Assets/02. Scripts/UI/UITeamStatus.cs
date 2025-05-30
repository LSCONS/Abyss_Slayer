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
                DictRefToSlot[playerRef].playerRef = playerRef;
                DictRefToSlot[playerRef].ChagneInRestText();
                DictRefToSlot[playerRef].ConnectUIHpBar();
                DictRefToSlot[playerRef].SetIcon(ServerManager.Instance.DictRefToNetData[playerRef].Class);
            }
        }
        gameObject.SetActive(true);
        ServerManager.Instance.UITeamStatus = this;
        UIManager.Instance.ResetAllRectTransform();
    }

    public void ChangeIsReadyPlayerText(PlayerRef playerRef, bool isReady)
    {
        DictRefToSlot[playerRef].ChagnePlayerReadyText(isReady);
    }


    public void ChagneInRestText()
    {
        foreach (PlayerRef playerRef in ServerManager.Instance.DictRefToPlayer.Keys)
        {
            if ((ServerManager.Instance.ThisPlayerRef != playerRef) && DictRefToSlot.ContainsKey(playerRef))
            {
                DictRefToSlot[playerRef].ChagneInRestText();
            }
        }
    }


    public void ChagneInGamePlayerText()
    {
        foreach (PlayerRef playerRef in ServerManager.Instance.DictRefToPlayer.Keys)
        {
            if ((ServerManager.Instance.ThisPlayerRef != playerRef) && DictRefToSlot.ContainsKey(playerRef))
            {
                DictRefToSlot[playerRef].ChangeInGameText();
            }
        }
    }
}
