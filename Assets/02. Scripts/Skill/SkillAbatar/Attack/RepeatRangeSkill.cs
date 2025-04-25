using Fusion;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "NewRepeatRangeSKill", menuName = "Skill/RangeAttack/RepeatRange")]

public class RepeatRangeSkill : RangeAttackSkill
{

    [field: Header("콜라이더 크기")]
    [field: SerializeField] public Vector2 ColliderSize { get; private set; } = new Vector2(1.8f, 14);

    [field: Header("오브젝트 크기")]
    [field: SerializeField] public Vector2 SpawnSize { get; private set; } = new Vector2(1, 0.4f);

    [field: Header("콜라이더 위치 값 조정")]
    [field: SerializeField] public Vector2 ColliderOffset { get; private set; } = new Vector2(0, 7);

    [field: Header("오브젝트 위치 값 조정(플레이어 기준)")]
    [field: SerializeField] public Vector2 SpawnOffset { get; private set; } = new Vector2(3, -1.1f);

    [field: Header("생성 딜레이 시간")]
    [field: SerializeField] public float TickRate { get; private set; } = 0.5f;

    [field: Header("콜라이더 유지 시간")]
    [field: SerializeField] public float Duration { get; private set; } = 0.5f;

    [field: Header("Effect 이름")]
    [field: SerializeField] public string EffectName { get; private set; } = "이펙트 이름";
    [field: Header("스킬을 반복할 횟수")]
    [field: SerializeField] public int UseSkillCount { get; private set; } = 5;
    [field: Header("스킬을 사용할 때마다 움직일 오브젝트 위치 값")]
    [field: SerializeField] public Vector2 MovePosition { get; private set; } = Vector2.zero;

    public override void UseSkill()
    {
        Vector2 temp = MovePosition;
        base.UseSkill();
        player.StartHoldSkillCoroutine(Repeat(), () => MovePosition = temp);
    }

    public IEnumerator Repeat()
    {
        WaitForSeconds wait = new WaitForSeconds(TickRate);
        Vector2 temp = MovePosition;
        // 풀에서 ZoneAOE 꺼내기
        for (int i = 0; i < 5; i++)
        {
            MovePosition = temp * i;
            PoolManager.Instance.Get<ZoneAOE>().Init(this);
            yield return wait;
        }
        player.StopHoldSkillCoroutine();
    }
}
