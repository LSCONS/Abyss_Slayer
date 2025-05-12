using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UIBossBuffSlotManager : Singleton<UIBossBuffSlotManager>
{
    [SerializeField] private GameObject buffSlotPrefab;
    [SerializeField] private Transform buffSlotParent;

    private Dictionary<DebuffType, GameObject> debuffSlot = new();

    /// <summary>
    /// 보스 디버프 UI 슬롯 생성
    /// </summary>
    public void CreateSlot(DebuffType type, DebuffData data, Sprite icon)
    {

        GameObject slotOb;

        // 이미 전에 있었는지 확인하고 있으면 그거 재사용해야됨
        if(debuffSlot.TryGetValue(type, out slotOb))
        {
            slotOb.SetActive(true);
        }
        else
        {
            slotOb = Instantiate(buffSlotPrefab, buffSlotParent);
            debuffSlot[type] = slotOb;
            // 새로 생기면 젤 앞으로 보내주고 싶음
            slotOb.transform.SetAsLastSibling();
        }
        var slot = slotOb.GetComponent<UIBuffSlot>();

        // 아이콘 설정
        slot.SetIcon(icon);

        // 슬롯 업데이트 해줌
        StartCoroutine(UpdateSlot(slot, type, data));

    }

    /// <summary>
    /// 디버프 시간 기준으로 UI 업데이트 → 시간이 끝나면 자동 제거
    /// </summary>
    private IEnumerator UpdateSlot(UIBuffSlot slot, DebuffType type, DebuffData data)
    {
        float duration = data.Duration;

        while (true)
        {
            float elapsed = Time.time - data.StartTime;
            float remaining = Mathf.Clamp(duration - elapsed, 0, duration);

            float ratio = remaining / duration;
            slot.SetCoolTime(remaining, duration); // 남은 시간 비율 표시
            if (remaining <= 0f)
                break;

            yield return null;
        }

        // 끝나면 setfalse
        if (this.debuffSlot.TryGetValue(type, out var slotGO))
        {
            slotGO.SetActive(false);
        }
    }

}
