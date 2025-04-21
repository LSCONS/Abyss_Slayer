using UniRx;
using UnityEngine;

public class Skill : ScriptableObject
{
    // 모든 스킬에 공통으로 적용되는 변수
    [HideInInspector] public Player player;
    [field: SerializeField] public string SkillName { get; private set; } = "스킬 이름";
    [field: SerializeField] public string SkillDesription { get; private set; } = "스킬 설명";
    [field: SerializeField] public Sprite SkillIcon {get; private set;} // 스킬 아이콘

    [field: SerializeField] public bool CanUse { get; set; } = true; // 스킬 사용 가능 여부
    [field: SerializeField] public bool CanMove { get; private set; } = true; // 스킬 사용 중 움직임 가능 여부

    [field: SerializeField] public ReactiveProperty<float> MaxCoolTime { get; private set; } // 기본 쿨타임
        = new ReactiveProperty<float>(10f);
    [field: SerializeField] public ReactiveProperty<float> CurCoolTime { get; private set; } // 현재 쿨타임
        = new ReactiveProperty<float>(0f);
    [field: SerializeField] public ApplyState ApplyState { get; set; } // 연결해서 작동시킬 State 설정
    
    // 플레이어 초기화
    public void Init(Player player)
    {
        this.player = player;
    }

    // 스킬 사용 추상 메서드
    public virtual void UseSkill()
    {

    }

    // 플레이어 방향 반환
    public float PlayerFrontXNomalized()
    {
        float x = player.SpriteRenderer.flipX ? -1f : 1f;
        return x;
    }

    // 플레이어 위치 반환
    public Vector3 PlayerPosition()
    {
        Vector3 playerPosition = player.transform.position;
        return playerPosition;
    }

    // 스킬 쿨타임 줄이기
    public void Cooldown(float cooldown)
    {
        if (!CanUse)
        {
            CurCoolTime.Value = Mathf.Max(CurCoolTime.Value - cooldown, 0);
            if (CurCoolTime.Value == 0) CanUse = true;
        }
    }
}
