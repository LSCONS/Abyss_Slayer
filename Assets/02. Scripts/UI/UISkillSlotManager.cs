using System.Collections.Generic;
using UnityEngine;

public class UISkillSlotManager : Singleton<UISkillSlotManager>
{
    [SerializeField] private GameObject skillSlotPrefab;
    [SerializeField] private Transform skillSlotParent;
    private Player player = null;

    private readonly List<SkillSlotPresenter> presenters = new();
    private readonly Dictionary<SkillSlotKey, UISkillSlot> slotsViews = new();
    private bool init = false;


    public async void Init()
    {
#if AllMethodDebug
        Debug.Log("Init");
#endif
        player = await ManagerHub.Instance.ServerManager.WaitForThisPlayerAsync();

        if (!init)
        {
            CreateSkillSlots();
        }

        BindSlots();
    }

    private void CreateSkillSlots()
    {
#if AllMethodDebug
        Debug.Log("CreateSkillSlots");
#endif
        init = true;

        foreach (var kvp in ManagerHub.Instance.ServerManager.ThisPlayer.DictSlotKeyToSkill)
        {
            if (!IsASDKey(kvp.Key))
                continue;

            var skillSlot = Instantiate(skillSlotPrefab, skillSlotParent);
            var slotView = skillSlot.GetComponent<UISkillSlot>();
            slotView.Init();
            slotsViews[kvp.Key] = slotView;
        }
    }

    // 슬롯 바인딩
    private void BindSlots()
    {
#if AllMethodDebug
        Debug.Log("BindSlots");
#endif
        // 기존에 있는 그 머냐 presenter를 제거함
        foreach (var presenter in presenters)
        {
            presenter.Dispose();
        }
        presenters.Clear();


        // 이제 플레이어 스킬 다시 돌면서 재연결
        foreach (var kvp in ManagerHub.Instance.ServerManager.ThisPlayer.DictSlotKeyToSkill)
        {
            if (!IsASDKey(kvp.Key))
                continue;
            if(slotsViews.TryGetValue(kvp.Key, out var slotView))
            {
                var presenter = new SkillSlotPresenter(kvp.Value, slotView);
                presenters.Add(presenter);
            }
        }
    }

    public void SettingKeySlot()
    {
#if AllMethodDebug
        Debug.Log("SettingKeySlot");
#endif
        foreach (var presenter in presenters)
        {
            presenter.SetKeyText();
        }
    }

    private bool IsASDKey(SkillSlotKey key)
    {
#if AllMethodDebug
        Debug.Log("IsASDKey");
#endif
        return key == SkillSlotKey.Z || key == SkillSlotKey.A || key == SkillSlotKey.S || key == SkillSlotKey.D;
    }

    private void OnDestroy()
    {
#if AllMethodDebug
        Debug.Log("OnDestroy");
#endif
        foreach (var presenter in presenters)
            presenter.Dispose();
        presenters.Clear();
    }

}
