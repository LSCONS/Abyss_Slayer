using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UIBuffSlotManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject buffSlotPrefab;
    [SerializeField] private Transform buffSlotParent;

    private readonly List<BuffSlotPresenter> presenters = new();

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
        player.CurDuration
            .Pairwise() // 이전 값, 현재 값 쌍을 받아서
            .Where(pair => pair.Previous <= 0 && pair.Current > 0) // 0 → 양수로 변한 순간만 감지
            .Subscribe(_ => CreateBuffSlot())
            .AddTo(this);
    }

    private void CreateBuffSlot()
    {
        Debug.Log("CreateBuffSlot");

        var slot = Instantiate(buffSlotPrefab, buffSlotParent);
        var view = slot.GetComponent<UIBuffSlot>();

        // 버프는 SkillData 없이 Player 기준으로 만들 수 있으므로 Presenter도 다르게 구성해야 함
        var presenter = new BuffSlotPresenter(player, view);
        presenters.Add(presenter);
    }
    private void OnDestroy()
    {
        foreach (var presenter in presenters)
            presenter.Dispose();
        presenters.Clear();
    }
}
