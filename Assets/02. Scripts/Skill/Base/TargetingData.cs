using UnityEngine;

/// <summary>
/// 스킬이 어떤 대상에게, 어떤 방식으로 적용되는지 정의하는 ScriptableObject.
/// 예: 단일 대상, 범위, 아군/적군, 자기 자신 포함 여부 등.
/// </summary>
[CreateAssetMenu(menuName = "Skill/TargetingData")]
public class TargetingData : ScriptableObject
{
    [Header("타게팅 조건")]
    public bool requiresTarget;     // 대상 지정이 필요한지 (false면 자동 발동)

    [Header("적용 범위")]
    public bool affectEnemies;      // 적에게 효과
    public bool affectPlayers;      // 아군에게 효과
    public bool includeSelf;        // 자기 자신 포함 여부

    public bool isArea;             // 범위 효과 여부
    public float range;             // 시전 거리 (공격 스킬)
    public float areaRadius;        // 범위 반경 (힐 or 버프)
}
