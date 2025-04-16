using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UIBuffSlotManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject buffSlotPrefab;
    [SerializeField] private Transform buffSlotParent;

    private Dictionary<SkillData, BuffSlotPresenter> activePresenters = new();

    private GameObject slotInstance;
    private BuffSlotPresenter presenter;    

    private void Awake()
    {
        player = GameObject.Find("Player")?.GetComponent<Player>();
    }

    private void Start()
    {
        ObservePlayerBuff();
    }
    private void ObservePlayerBuff()
    {
        Debug.Log("[BuffManager] ObservePlayerBuff 구독 시작");

        player.CurDuration
            .Do(x => Debug.Log($"[BuffManager] CurDuration 변화: {x}"))
            .Subscribe(duration =>
            {
                if (duration > 0)
                {
                    ShowBuffSlot();
                }
                else
                {
                    HideBuffSlot();
                }
            })
            .AddTo(this);
    }

    private void ShowBuffSlot()
    {
        if (slotInstance == null)
        {
            Debug.Log("CreateBuffSlot (처음 생성)");

            slotInstance = Instantiate(buffSlotPrefab, buffSlotParent);
            var view = slotInstance.GetComponent<UIBuffSlot>();
            presenter = new BuffSlotPresenter(player, view);
        }
        else
        {
            Debug.Log("Reusing existing slot");
            slotInstance.SetActive(true);
        }
    }

    private void HideBuffSlot()
    {
        if (slotInstance != null)
        {
            Debug.Log("Hiding existing slot");
            slotInstance.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        presenter?.Dispose();
    }
}
