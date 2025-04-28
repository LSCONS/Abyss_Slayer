using Photon.Realtime;
using System;
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
    public Vector2      SpawnOffset     { get; private set; }
    public Vector2      SpawnSize       { get; private set; }
    public string       EffectName      { get; private set; }
    public Vector2      MovePosition    { get; private set; }
    public MeleeDamageCheckData Data { get; private set; }


    public override void Init()
    {

    }

    public void StartSkill()
    {
        gameObject.SetActive(true);

        // 위치 세팅
        float playerFlipX = Data.Player.IsFlipX ? -1 : 1;
        SpawnOffset += MovePosition;
        Vector3 spawnPosition = Data.Player.transform.position + new Vector3(SpawnOffset.x * playerFlipX, SpawnOffset.y, 0);

        transform.position = spawnPosition;
        transform.localScale = (Vector3)SpawnSize;

        // 애니메이터 세팅
        SetActiveAnimator();

        // meleedamagecheck 세팅
        var meleeCheck = GetComponent<MeleeDamageCheck>();
        meleeCheck.Init(Data);
        // duration 후 풀에 자동 반환

        if (Data.Skill != null && Data.Skill.SkillCategory == SkillCategory.Hold)
        {
            Data.Player.StartHoldSkillCoroutine(ReturnTOPool(Data.Duration, Data.TargetLayer), Test);
        }
        else
        {
            if (ReturnToPoolCoroutine != null) StopCoroutine(ReturnToPoolCoroutine);
            ReturnToPoolCoroutine = StartCoroutine(ReturnTOPool(Data.Duration, Data.TargetLayer));
        }
    }

    public override void Init(RepeatRangeSkill repeatRangeSkill)
    {
        Init((RemoteZoneRangeSkill)repeatRangeSkill, repeatRangeSkill.MovePosition, null);
    }

    public override void Init(DashMeleeSkill dashMeleeSkill, Type effectType)
    {
        Init((RemoteZoneRangeSkill)dashMeleeSkill, Vector2.zero, effectType);
    }

    public override void Init(RemoteZoneRangeSkill remoteZoneRangeSkill, Vector2 move, Type effectType)
    {
        Data = new MeleeDamageCheckData(remoteZoneRangeSkill, effectType);
        SpawnOffset = remoteZoneRangeSkill.SpawnOffset;
        SpawnSize = remoteZoneRangeSkill.SpawnSize;
        EffectName      = remoteZoneRangeSkill.EffectName;
        MovePosition    = move;
        StartSkill();
    }
    private IEnumerator ReturnTOPool(float seconds, LayerMask targetLayer)
    {
            yield return new WaitForSeconds(seconds);
            if (effectPrefab != null)
                effectPrefab.SetActive(false);
            ReturnToPool();
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
            SpriteRenderer.flipX = Data.Player.IsFlipX;

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
}
