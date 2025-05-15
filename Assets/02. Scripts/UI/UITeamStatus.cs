using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UITeamStatus : UIPermanent
{
    public UITeamStatusSlot PlayerStatusPrefab { get; private set; }
    public Dictionary<PlayerRef, UITeamStatusSlot> DictRefToSlot { get; private set; } = new();
    public override async void Init()
    {
        base.Init();
        if(PlayerStatusPrefab == null)
        {
            var data = Addressables.LoadAssetAsync<GameObject>("TeamStatusSlot");
            await data.Task;
            PlayerStatusPrefab = data.Result.GetComponent<UITeamStatusSlot>();
        }

        await ServerManager.Instance.WaitForAllPlayerLoadingAsync();
        foreach(PlayerRef playerRef in ServerManager.Instance.DictRefToPlayer.Keys)
        {
            if ((ServerManager.Instance.ThisPlayerRef != playerRef) && !(DictRefToSlot.ContainsKey(playerRef)))
            {
                DictRefToSlot[playerRef] = Instantiate(PlayerStatusPrefab, transform);
                DictRefToSlot[playerRef].playerRef = playerRef;
                DictRefToSlot[playerRef].Init();
            }
        }
        gameObject.SetActive(true);

        ServerManager.Instance.UITeamStatus = this;
        RectTransform rect = transform.parent.GetComponent<RectTransform>();
        if(rect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void ChangeIsReadyPlayerText(PlayerRef playerRef, bool isReady)
    {
        DictRefToSlot[playerRef].ChagnePlayerReadyText(isReady);
    }
}
