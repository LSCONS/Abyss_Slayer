using System.Collections.Generic;
using UnityEngine;

public class UISkillSlotManager : Singleton<UISkillSlotManager>
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject skillSlotPrefab;
    [SerializeField] private Transform skillSlotParent;

    private readonly List<SkillSlotPresenter> presenters = new();
    private readonly Dictionary<SkillSlotKey, UISkillSlot> slotsViews = new();
    private bool init = false;


    public void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (!init)
        {
            CreateSkillSlots();
        }

        BindSlots();
    }

    private void CreateSkillSlots()
    { 
        init = true;

        foreach (var kvp in player.EquippedSkills)
        {
            if (!IsASDKey(kvp.Key))
                continue;

            var skillSlot = Instantiate(skillSlotPrefab, skillSlotParent);
            var slotView = skillSlot.GetComponent<UISkillSlot>();
            slotsViews[kvp.Key] = slotView;
        }
    }

    // 슬롯 바인딩
    private void BindSlots()
    {
        // 기존에 있는 그 머냐 presenter를 제거함
        foreach (var presenter in presenters)
        {
            presenter.Dispose();
        }
        presenters.Clear();


        // 이제 플레이어 스킬 다시 돌면서 재연결
        foreach (var kvp in player.EquippedSkills)
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

    private bool IsASDKey(SkillSlotKey key)
    {
        return key == SkillSlotKey.A || key == SkillSlotKey.S || key == SkillSlotKey.D;
    }

    private void OnDestroy()
    {
        foreach (var presenter in presenters)
            presenter.Dispose();
        presenters.Clear();
    }

}
