using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClassSlotController : MonoBehaviour
{
    [SerializeField] private GameObject classSlotPrefab;
    [SerializeField] private Transform slotParent;

    // 리스트에 슬롯 넣고
    private List<ClassSlot> slots = new();
    public IReadOnlyList<ClassSlot> Slots => slots;

    // 클래스 개수 가져와서 그 개수만큼 슬롯 생성하기
    public void CreateClassSlots()
    {
        if (classSlotPrefab == null) return;

        int slotIndex = (int)CharacterClass.Count;

        for (int i = 0; i < slotIndex; i++)
        {
            CharacterClass cc = (CharacterClass)i;

            if (cc == CharacterClass.Healer)    // 힐러 미구현
                continue;

            var go = Instantiate(classSlotPrefab, slotParent);
            var slot = go.GetComponent<ClassSlot>();
            slot.SetData(cc);
            slot.SetIcon(cc);
            slots.Add(slot);    //리스트에 저장
        }
    }
}
