using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

// 범위 공격에 쓰일 장판 스크립트
public class ZoneAOE : BasePoolable
{
    private Coroutine tickCoroutine; // 틱뎀 줄 코루틴
    private Coroutine ReturnToPoolCoroutine; // 풀로 돌려보내는 코루틴
    private HashSet<IHasHealth> targetsInRange = new();

    [SerializeField] private GameObject effectPrefab; // 이펙트 프리팹

    public SpriteRenderer SpriteRenderer {  get; private set; }
    public Animator     Animator        { get; private set; }

    public Player       Player          { get; private set; }
    public Skill        Skill           { get; private set; }
    public Vector2      SpawnOffset     { get; private set; }
    public Vector2      SpawnSize       { get; private set; }
    public Vector2      ColliderSize    { get; private set; }
    public Vector2      ColliderOffset  { get; private set; }
    public float        TickRate        { get; private set; }
    public float        Duration        { get; private set; }
    public float        Damage          { get; private set; }
    public LayerMask    TargetLayer     { get; private set; }
    public string       EffectName      { get; private set; }
    public Vector2      MovePosition    { get; private set; }


    public override void Init()
    {

    }

    public void StartSkill()
    {
        gameObject.SetActive(true);

        // 위치 세팅
        float playerFlipX = Player.IsFlipX ? -1 : 1;
        SpawnOffset += MovePosition;
        Vector3 spawnPosition = Player.transform.position + new Vector3(SpawnOffset.x * playerFlipX, SpawnOffset.y, 0);

        transform.position = spawnPosition;
        transform.localScale = (Vector3)SpawnSize;

        // 애니메이터 세팅
        SetActiveAnimator();

        // meleedamagecheck 세팅
        var meleeCheck = GetComponent<MeleeDamageCheck>();
        System.Type fxType = null;
        meleeCheck.Init(Player, Skill, ColliderSize, ColliderOffset, Damage, fxType, Duration);
        meleeCheck.SetRepeatMode(true, TickRate);
        // duration 후 풀에 자동 반환

        if (Skill != null && Skill.SkillCategory == SkillCategory.Hold)
        {
            Player.StartHoldSkillCoroutine(ReturnTOPool(Duration, TargetLayer), null);
        }
        else
        {
            if (ReturnToPoolCoroutine != null) StopCoroutine(ReturnToPoolCoroutine);
            ReturnToPoolCoroutine = StartCoroutine(ReturnTOPool(Duration, TargetLayer));
        }
    }

    public override void Init(RepeatRangeSkill repeatRangeSkill)
    {
        Player          = repeatRangeSkill.player;
        Skill           = repeatRangeSkill;
        SpawnOffset     = repeatRangeSkill.SpawnOffset;
        SpawnSize       = repeatRangeSkill.SpawnSize;
        ColliderSize    = repeatRangeSkill.ColliderSize;
        ColliderOffset  = repeatRangeSkill.ColliderOffset;
        TickRate        = repeatRangeSkill.TickRate;
        Duration        = repeatRangeSkill.Duration;
        Damage          = repeatRangeSkill.Damage;
        TargetLayer     = repeatRangeSkill.TargetLayer;
        EffectName      = repeatRangeSkill.EffectName;
        MovePosition    = repeatRangeSkill.MovePosition;
        StartSkill();
    }

    public override void Init(RemoteZoneRangeSkill remoteZoneRangeSkill)
    {
        Player          = remoteZoneRangeSkill.player;
        Skill           = remoteZoneRangeSkill;
        SpawnOffset     = remoteZoneRangeSkill.SpawnOffset;
        SpawnSize       = remoteZoneRangeSkill.SpawnSize;
        ColliderSize    = remoteZoneRangeSkill.ColliderSize;
        ColliderOffset  = remoteZoneRangeSkill.ColliderOffset;
        TickRate        = remoteZoneRangeSkill.TickRate;
        Duration        = remoteZoneRangeSkill.Duration;
        Damage          = remoteZoneRangeSkill.Damage;
        TargetLayer     = remoteZoneRangeSkill.TargetLayer;
        EffectName      = remoteZoneRangeSkill.EffectName;
        StartSkill();
    }
    private IEnumerator ReturnTOPool(float seconds, LayerMask targetLayer)
    {
        if (Skill != null && Skill.SkillCategory == SkillCategory.Hold)
        {
            Player.StopHoldSkillCoroutine();
        }
        else
        {
            yield return new WaitForSeconds(seconds);
            if (effectPrefab != null)
                effectPrefab.SetActive(false);
            ReturnToPool();
        }
    }

    private void Test()
    {
        effectPrefab.SetActive(false);
        ReturnToPool();
    }

    // 애니메이터 활성화
    private void SetActiveAnimator()
    {
        //EffectName과 관련된 애니메이터가 있는지 확인하고 가져오고 실행.
        RuntimeAnimatorController pathAnimator = ChangeAnimatior(EffectName);
        if(pathAnimator != null)
        {
            //SpriteRenderer와 연결이 안 되어 있을 경우 가져옴
            if(SpriteRenderer == null) 
                SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            //Animator와 연결이 안되어 있을 경우 가져옴
            if(Animator == null)
                Animator = GetComponentInChildren<Animator>();

            //찾아둔 애니메이터와 연결
            Animator.runtimeAnimatorController = pathAnimator;

            //스프라이트 값 초기화
            SpriteRenderer.transform.localScale = Vector3.one;
            SpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            SpriteRenderer.flipX = Player.IsFlipX;

            if (Animator != null && Animator.runtimeAnimatorController != null)
            {
                // 첫 번째 클립 길이 가져오기
                var clips = Animator.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                {
                    Animator.Play(clips[0].name, 0, 0f);
                }
            }
            effectPrefab.SetActive(true);
        }
    }

    // 애니메이터 가져오기 << 나중에 미리 캐싱해두기
    private RuntimeAnimatorController ChangeAnimatior(string effectName)
    {
        return Resources.Load<RuntimeAnimatorController>("Effect/Animator/" + effectName);
    }


    // 기즈모 범위 씬 뷰에서 범위 확인용
    private void OnDrawGizmosSelected()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        Gizmos.color = Color.cyan;
        Vector3 size = new Vector3(col.size.x, col.size.y, 0f);
        Gizmos.DrawWireCube(transform.position, size);
    }

}
