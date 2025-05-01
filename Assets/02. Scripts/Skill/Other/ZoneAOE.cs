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

    public SpriteRenderer SpriteRenderer        { get; private set; }
    public Animator     Animator                { get; private set; }
    public Vector2      SpawnOffset             { get; private set; }
    public Vector2      SpawnSize               { get; private set; }
    public Vector2      MovePosition            { get; private set; }
    public MeleeDamageCheckData Data            { get; private set; }
    public MeleeDamageCheck MeleeDamageCheck    { get; set; }


    public override void Init()
    {

    }

    public void UseSkillSetting(string colliderEffectName)
    {
        // 위치 세팅
        float playerFlipX = Data.Player.IsFlipX ? -1 : 1;
        SpawnOffset += MovePosition;
        Vector3 spawnPosition = Data.Player.transform.position + new Vector3(SpawnOffset.x * playerFlipX, SpawnOffset.y, 0);
        UseSkillStart(colliderEffectName, playerFlipX, spawnPosition);
    }

    public void UseSkillSetting(string colliderEffectName, float flipX, Vector3 playerPosition)
    {
        // 위치 세팅
        SpawnOffset += MovePosition;
        Vector3 spawnPosition = playerPosition + new Vector3(SpawnOffset.x * flipX, SpawnOffset.y, 0);
        UseSkillStart(colliderEffectName, flipX, spawnPosition);
    }


    public void UseSkillStart(string colliderEffectName, float playerFlipX, Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        transform.localScale = (Vector3)SpawnSize;

        // 애니메이터 세팅
        SetActiveAnimator(colliderEffectName);
        MeleeDamageCheck.Init(Data);
        // duration 후 풀에 자동 반환

        if (Data.Skill != null && Data.Skill.SkillCategory == SkillCategory.Hold)
        {
            Data.Player.StartHoldSkillCoroutine(ReturnTOPool(Data.Duration), Exit);
        }
        else
        {
            if (ReturnToPoolCoroutine != null) StopCoroutine(ReturnToPoolCoroutine);
            ReturnToPoolCoroutine = StartCoroutine(ReturnTOPool(Data.Duration));
        }
    }

    public void Init(RepeatRangeSkill repeatRangeSkill, float flipX, Vector3 playerPosition)
    {
        Init((RemoteZoneRangeSkill)repeatRangeSkill, repeatRangeSkill.MovePosition, null);
    }

    public void Init(DashMeleeSkill dashMeleeSkill, Type effectType, GameObject effectPrefab)
    {
        DataInit(dashMeleeSkill, Vector2.zero, effectType); 
        Vector2 dashDirection = Data.Player.IsFlipX ? Vector2.left : Vector2.right;
        StartCoroutine(DashCoroutine(dashDirection, dashMeleeSkill, effectPrefab));
    }

    public void Init(RemoteZoneRangeSkill remoteZoneRangeSkill, Vector2 move, Type effectType)
    {
        DataInit(remoteZoneRangeSkill, move, effectType);
        UseSkillSetting(remoteZoneRangeSkill.EffectName);
    }

    public void Init(RemoteZoneRangeSkill remoteZoneRangeSkill, Vector2 move, Type effectType, float flipX, Vector3 playerPosition)
    {
        DataInit(remoteZoneRangeSkill, move, effectType);
        UseSkillSetting(remoteZoneRangeSkill.EffectName, flipX, playerPosition);
    }

    private void DataInit(RemoteZoneRangeSkill remoteZoneRangeSkill, Vector2 move, Type effectType)
    {
        Data = new MeleeDamageCheckData(remoteZoneRangeSkill, effectType);
        SpawnOffset = remoteZoneRangeSkill.SpawnOffset;
        SpawnSize = remoteZoneRangeSkill.SpawnSize;
        MovePosition = move;
        SetActiveFalseInit();
    }

    private void SetActiveFalseInit()
    {
        // meleedamagecheck 세팅
        if (MeleeDamageCheck == null)
            MeleeDamageCheck = GetComponent<MeleeDamageCheck>();
        if (MeleeDamageCheck.BoxCollider == null)
        {
            MeleeDamageCheck.BoxCollider = GetComponent<BoxCollider2D>();
        }
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (Animator == null)
            Animator = GetComponentInChildren<Animator>();

        MeleeDamageCheck.BoxCollider.enabled = false;
        Animator.enabled = false;
    }

    private IEnumerator ReturnTOPool(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Exit();
    }

    private void Exit()
    {
        if (effectPrefab != null)
            effectPrefab.SetActive(false);
        ReturnToPool();
        Animator.enabled = false;
        MeleeDamageCheck.Exit();
        Data.Player.StopHoldSkillNoneCoroutine();
    }

    // 애니메이터 활성화
    private void SetActiveAnimator(string colliderEffectName)
    {
        //EffectName과 관련된 애니메이터가 있는지 확인하고 가져오고 실행.
        RuntimeAnimatorController pathAnimator = ChangeAnimatior(colliderEffectName);
        if(pathAnimator != null)
        {
            //찾아둔 애니메이터와 연결하고 활성화
            Animator.runtimeAnimatorController = pathAnimator;
            Animator.enabled = true;
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

    private IEnumerator DashCoroutine(Vector2 dashDirection, DashMeleeSkill dashMeleeSkill, GameObject dashEffectPrefab)
    {
        // 시작 위치 타겟위치 설정
        float time = 0;
        Vector2 startPos = Data.Player.transform.position;
        Vector2 targetPos = startPos + dashDirection * dashMeleeSkill.DashDistance + Vector2.up * 0.01f; ;

        // 대쉬 이펙트 생성
        GameObject dashEffect = null;
        if (dashEffectPrefab != null)
        {
            // 위치 설정
            Vector3 effectOffset = new Vector3(-dashDirection.x, -0.5f, 0);   // 플립되어있으면 1f, 아니면 -1f
            Vector3 effectPos = (Vector3)startPos + effectOffset;

            dashEffect = GameObject.Instantiate(dashEffectPrefab, effectPos, Quaternion.identity, Data.Player.transform);
            dashEffect.transform.right = dashDirection;

            // 이펙트 방향 설정
            SpriteRenderer effectSpriteRenderer = dashEffect.GetComponent<SpriteRenderer>();
            if (effectSpriteRenderer != null)
            {
                effectSpriteRenderer.flipX = Data.Player.IsFlipX;
            }
        }


        // dashDistance만큼을 dashDuration 시간동안 이동
        while (time < dashMeleeSkill.DashTime)
        {
            Data.Player.playerRigidbody.MovePosition(Vector2.Lerp(startPos, targetPos, time / dashMeleeSkill.DashTime));
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 대시 이후 위치
        Data.Player.playerRigidbody.MovePosition(targetPos);

        // 이펙트 제거
        if (dashEffect != null)
        {
            GameObject.Destroy(dashEffect, 0.5f);
        }

        // 끝/중간 위치 설정
        Vector2 endPos = Data.Player.transform.position;
        float distance = Vector2.Distance(startPos, endPos);

        // 콜라이더 위치 저장 이동
        Vector2 originalPos = Data.Player.PlayerMeleeCollider.transform.position;

        Data.ColliderSize = new Vector2(distance, 1.0f);
        Data.ColliderOffset = new Vector2(-distance / 2, 0);
        Debug.Log("켜짐");
        UseSkillSetting(dashMeleeSkill.EffectName);
    }
}
