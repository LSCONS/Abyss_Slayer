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
            skillSlot.GetComponent<UISkillSlot>().Bind(kvp.Value);
        }
    }

    private bool IsASDKey(SkillSlotKey key)
    {
        return key == SkillSlotKey.A || key == SkillSlotKey.S || key == SkillSlotKey.D;
    }
}
