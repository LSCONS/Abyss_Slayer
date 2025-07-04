using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UIBossBuffSlotManager : Singleton<UIBossBuffSlotManager>
{
    [SerializeField] private GameObject buffSlotPrefab;
    [SerializeField] private Transform buffSlotParent;

    private Dictionary<EBuffType, GameObject> debuffSlot = new();

    /// <summary>
    /// 보스 디버프 UI 슬롯 생성
    /// </summary>
    public void CreateSlot(int buffTypeInt, string debuffDataName, string debuffDataDescription, float debuffDataDuration)
    {
        GameObject slotOb;

        // 이미 전에 있었는지 확인하고 있으면 그거 재사용해야됨
        if(debuffSlot.TryGetValue((EBuffType)buffTypeInt, out slotOb))
        {
            slotOb.SetActive(true);
        }
        else
        {
            slotOb = Instantiate(buffSlotPrefab, buffSlotParent);
            debuffSlot[(EBuffType)buffTypeInt] = slotOb;
            // 새로 생기면 젤 앞으로 보내주고 싶음
            slotOb.transform.SetAsLastSibling();
        }
        var slot = slotOb.GetComponent<UIBuffSlot>();

        // 아이콘 설정 찾을 수 없다면 return
        if (!(ManagerHub.Instance.DataManager.DictBuffToSprite.TryGetValue((EBuffType)buffTypeInt, out Sprite icon))) return;
        slot.SetIcon(icon);

        // 슬롯 업데이트 해줌
        StartCoroutine(UpdateSlot(slot, (EBuffType)buffTypeInt, debuffDataDuration));
        slot.SetBuffInfo(debuffDataName, debuffDataDescription);
    }

    /// <summary>
    /// 디버프 시간 기준으로 UI 업데이트 → 시간이 끝나면 자동 제거
    /// </summary>
    private IEnumerator UpdateSlot(UIBuffSlot slot, EBuffType type,float debuffDataDuration)
    {
        float duration = debuffDataDuration;
        float startTime = Time.time;

        while (true)
        {
            float elapsed = Time.time - startTime;
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

    // 슬롯 클리어해주는 메서드
    public void ClearAllSlots()
    {
        foreach(var kvp in debuffSlot)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        debuffSlot.Clear();
    }
}
