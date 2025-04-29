using Fusion;
using System.Collections;
using UnityEngine;

/// <summary>
/// RemoteZone 스킬을 코루틴을 이용해 연속적으로 사용하는 기능
/// </summary>
[CreateAssetMenu(fileName = "NewRepeatRangeSKill", menuName = "Skill/RangeAttack/RepeatRange")]
public class RepeatRangeSkill : RemoteZoneRangeSkill
{
    [field: Header("스킬을 반복할 횟수")]
    [field: SerializeField] public int SkillRepeatCount { get; private set; } = 5;
    [field: Header("스킬을 사용할 때마다 움직일 오브젝트 위치 값")]
    [field: SerializeField] public Vector2 MovePosition { get; private set; } = Vector2.zero;
    [field: Header("스킬을 반복할 딜레이 시간")]
    [field: SerializeField] public float SkillRepeatDelayTime { get; private set; } = 0.5f;

    public override void UseSkill()
    {
        Vector2 temp = MovePosition;
        player.StartHoldSkillCoroutine(Repeat(), () => MovePosition = temp);
    }

    public IEnumerator Repeat()
    {
        WaitForSeconds wait = new WaitForSeconds(SkillRepeatDelayTime);
        Vector2 temp = MovePosition;
        // 풀에서 ZoneAOE 꺼내기
        for (int i = 0; i < 5; i++)
        {
            MovePosition = temp * i;
            PoolManager.Instance.Get<ZoneAOE>().Init(this);
            yield return wait;
        }
        player.StopHoldSkillActionCoroutine();
    }
    public override void SkillUpgrade()
    {

    }
}
