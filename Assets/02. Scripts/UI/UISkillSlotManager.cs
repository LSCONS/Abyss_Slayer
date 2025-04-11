using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UISkillSlotManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject skillSlotPrefab;
    [SerializeField] private Transform skillSlotParent;

    private readonly List<SkillSlotPresenter> presenters = new();

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    private void Start()
    {
        CreateSkillSlots();
    }

    private void CreateSkillSlots()
    {
        foreach (var kvp in player.equippedSkills)
        {
            if (!IsASDKey(kvp.Key))
                continue;

            var skillSlot = Instantiate(skillSlotPrefab, skillSlotParent);
            var slotView = skillSlot.GetComponent<UISkillSlot>();
            var presenter = new SkillSlotPresenter(kvp.Value, slotView);
            presenters.Add(presenter);
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
