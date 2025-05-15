using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UIBuffSlotManager : Singleton<UIBuffSlotManager>
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject buffSlotPrefab;
    [SerializeField] private Transform buffSlotParent;


    private Dictionary<BuffType, GameObject> slotInstances = new();
    private Dictionary<BuffType, BuffSlotPresenter> activePresenters = new();

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init()
    {
        ClearSlot();    
        player = PlayerManager.Instance.Player;

        // Player의 모든 스킬 중 BuffSkill만 감지
        foreach (var skill in player.EquippedSkills.Values)
        {
            if (skill is BuffSkill buffSkill)
            {
                RegisterBuff(buffSkill.Type, buffSkill);
            }
        }
    }

    /// <summary>
    /// 새로운 버프가 추가되었을 때 외부에서 호출할 것
    /// </summary>
    public void RegisterBuff(BuffType buffType, BuffSkill buffSkill)
    {
        if (activePresenters.ContainsKey(buffType))
            return;

        // 구독 걸기
        buffSkill.CurBuffDuration
            .Subscribe(duration =>
            {
                if (duration > 0)
                {
                    if (!activePresenters.ContainsKey(buffType))
                    {
                        ShowBuffSlot(buffType, buffSkill);
                    }
                }
                else
                {
                    HideBuffSlot(buffType, buffSkill);
                }
            })
            .AddTo(this);
    }
    private void ShowBuffSlot(BuffType buffType, BuffSkill buffSkill)
    {
        var slotInstance = Instantiate(buffSlotPrefab, buffSlotParent);
        var view = slotInstance.GetComponent<UIBuffSlot>();
        var presenter = new BuffSlotPresenter(buffSkill, view);

        slotInstances[buffType] = slotInstance;
        activePresenters[buffType] = presenter;
    }

    private void HideBuffSlot(BuffType buffType, BuffSkill buffSkill)
    {
        if (activePresenters.TryGetValue(buffType, out var presenter))
        {
            presenter.Dispose();
            activePresenters.Remove(buffType);
        }

        if (slotInstances.TryGetValue(buffType, out var slotInstance))
        {
            slotInstance.SetActive(false); //  풀링
            slotInstances.Remove(buffType);
        }
    }

    // 슬롯 초기화 해줌 기존에 남아있는 프레젠터랑 슬롯을 정리해줘야됨
    private void ClearSlot()
    {
        foreach (var presenter in activePresenters.Values)
            presenter.Dispose();
        activePresenters.Clear();

        foreach (var slot in slotInstances.Values)
            Destroy(slot);
        slotInstances.Clear();
    }
        
        

    private void OnDestroy()
    {
        foreach (var presenter in activePresenters.Values)
        {
            presenter.Dispose();
        }
    }
}
