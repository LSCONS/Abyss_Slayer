using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 범위 공격에 쓰일 장판 스크립트
public class ZoneAOE : BasePoolable
{
    private Coroutine tickCoroutine; // 틱뎀 줄 코루틴
    private Coroutine ReturnToPoolCoroutine; // 풀로 돌려보내는 코루틴
    private HashSet<IHasHealth> targetsInRange = new();

    public SpriteRenderer SpriteRenderer { get; private set; }
    public Animator Animator { get; private set; }
    public Vector2 SpawnOffset { get; private set; }
    public Vector2 SpawnSize { get; private set; }
    public Vector2 MovePosition { get; private set; }
    public MeleeDamageCheckData Data { get; set; }
    public MeleeDamageCheck MeleeDamageCheck { get; set; }


    public override void Rpc_Init()
    {

    }

    public void UseSkillSetting()
    {
        // 시작 위치 타겟위치 설정
        float playerFlipX = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].IsFlipX ? -1 : 1;
        // 위치 세팅
        SpawnOffset += MovePosition;
        Vector3 spawnPosition = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].transform.position + new Vector3(SpawnOffset.x * playerFlipX, SpawnOffset.y, 0);
        UseSkillStart(playerFlipX, spawnPosition);
    }

    public void UseSkillSetting(float playerFlipX, Vector2 colliderSize, Vector2 colliderOffset)
    {
        // 위치 세팅
        SpawnOffset += MovePosition;
        Vector3 spawnPosition = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].transform.position + new Vector3(SpawnOffset.x * playerFlipX, SpawnOffset.y, 0);
        UseSkillStart(playerFlipX, spawnPosition, colliderSize, colliderOffset);
    }

    public void UseSkillSetting(float flipX, Vector3 playerPosition)
    {
        // 위치 세팅
        SpawnOffset += MovePosition;
        Vector3 spawnPosition = playerPosition + new Vector3(SpawnOffset.x * flipX, SpawnOffset.y, 0);
        UseSkillStart(flipX, spawnPosition);
    }


    public void UseSkillStart(float playerFlipX, Vector3 spawnPosition)
    {
        gameObject.SetActive(true);
        transform.position = spawnPosition;
        transform.localScale = (Vector3)SpawnSize;

        // 애니메이터 세팅
        SetActiveAnimator();
        MeleeDamageCheck.Rpc_Init(Data, playerFlipX);
        // duration 후 풀에 자동 반환
        if (Data.GetSkill() != null && Data.GetSkill().SkillCategory == SkillCategory.Hold)
        {
            ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].StartHoldSkillCoroutine(ReturnTOPool(Data.ColliderDuration), Rpc_Exit);
        }
        else
        {
            if (ReturnToPoolCoroutine != null) StopCoroutine(ReturnToPoolCoroutine);
            ReturnToPoolCoroutine = StartCoroutine(ReturnTOPool(Data.ColliderDuration));
        }
    }


    public void UseSkillStart(float playerFlipX, Vector3 spawnPosition, Vector2 colliderSize, Vector2 colliderOffset)
    {
        gameObject.SetActive(true);
        transform.position = spawnPosition;
        transform.localScale = (Vector3)SpawnSize;

        // 애니메이터 세팅
        SetActiveAnimator();
        MeleeDamageCheck.Rpc_Init(Data, colliderSize, colliderOffset, playerFlipX);
        // duration 후 풀에 자동 반환

        if (Data.GetSkill() != null && Data.GetSkill().SkillCategory == SkillCategory.Hold)
        {
            ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].StartHoldSkillCoroutine(ReturnTOPool(Data.ColliderDuration), Rpc_Exit);
        }
        else
        {
            if (ReturnToPoolCoroutine != null) StopCoroutine(ReturnToPoolCoroutine);
            ReturnToPoolCoroutine = StartCoroutine(ReturnTOPool(Data.ColliderDuration));
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_RepeatInit(MeleeDamageCheckData data, Vector2 spawnSize, Vector2 spawnOffset, Vector2 move, float flipX, Vector3 playerPosition)
    {
        Init(data, spawnSize, spawnOffset, move, flipX, playerPosition);
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_DashInit(MeleeDamageCheckData data, Vector2 spawnSize, Vector2 spawnOffset, float dashDistance, float dashTime)
    {
        gameObject.SetActive(true);
        DataInit(data, spawnSize, spawnOffset, Vector2.zero); 
        Vector2 dashDirection = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].IsFlipX ? Vector2.left : Vector2.right;
        StartCoroutine(DashCoroutine(dashDirection, dashDistance, dashTime));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(MeleeDamageCheckData data, Vector2 spawnSize, Vector2 spawnOffset, Vector2 move)
    {
        DataInit(data, spawnSize, spawnOffset, move);
        UseSkillSetting();
    }

    public void Init(MeleeDamageCheckData data, Vector2 spawnSize, Vector2 spawnOffset, Vector2 move, float flipX, Vector3 playerPosition)
    {
        DataInit(data, spawnSize, spawnOffset, move);
        UseSkillSetting(flipX, playerPosition);
    }

    private void DataInit(MeleeDamageCheckData data, Vector2 spawnSize, Vector2 spawnOffset, Vector2 move)
    {
        Data = data;
        SpawnOffset = spawnOffset;
        SpawnSize = spawnSize;
        MovePosition = move;
        SetActiveFalseInit();
    }

    private void SetActiveFalseInit()
    {
        // meleedamagecheck 세팅
        if (MeleeDamageCheck == null)
            MeleeDamageCheck = GetComponent<MeleeDamageCheck>();
        if (MeleeDamageCheck.BoxCollider == null)
            MeleeDamageCheck.BoxCollider = GetComponent<BoxCollider2D>();
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
        Rpc_Exit();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_Exit()
    {
        if (Runner.IsServer)
        {
            Rpc_ReturnToPool();
        }

        Animator.enabled = false;
        MeleeDamageCheck.Exit();
        if (Data.GetSkill().SkillCategory == SkillCategory.Hold)
            ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].StopHoldSkillNoneCoroutine();
        SpriteRenderer.sprite = null;
    }

    // 애니메이터 활성화
    private void SetActiveAnimator()
    {
        //EffectName과 관련된 애니메이터가 있는지 확인하고 가져오고 실행.
        if(ManagerHub.Instance.DataManager.DictEnumToAnimatorData.TryGetValue((EAnimatorController)Data.AnimatorControllerInt, out var pathAnimator))
        if (pathAnimator != null)
        {
            //찾아둔 애니메이터와 연결하고 활성화
            Animator.runtimeAnimatorController = pathAnimator;
            Animator.enabled = true;
                //스프라이트 값 초기화
            SpriteRenderer.transform.localScale = Vector3.one;
            SpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            SpriteRenderer.flipX = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].IsFlipX;

            if (Animator != null && Animator.runtimeAnimatorController != null)
            {
                // 첫 번째 클립 길이 가져오기
                var clips = Animator.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                {
                    Animator.Play(clips[0].name, 0, 0f);
                }
            }
        }
    }


    private IEnumerator DashCoroutine(Vector2 dashDirection, float dashDistance, float dashTime)
    {
        // 시작 위치 타겟위치 설정
        float playerFlipX = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].IsFlipX ? -1 : 1;
        float time = 0;
        Vector2 startPos = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].transform.position;
        Vector2 targetPos = startPos + dashDirection * dashDistance + Vector2.up * 0.01f; ;
        Debug.Log($"startPos = {startPos}");
        // 대쉬 이펙트 생성
        // 위치 설정
        Vector3 effectOffset = new Vector3(-dashDirection.x, -0.5f, 0);   // 플립되어있으면 1f, 아니면 -1f
        Vector3 effectPos = (Vector3)startPos + effectOffset;

        GameObject dashEffect = GameObject.Instantiate(ManagerHub.Instance.DataManager.DashEffectPrefab, effectPos, Quaternion.identity, ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].transform);
        dashEffect.transform.right = dashDirection;

        // 이펙트 방향 설정
        SpriteRenderer effectSpriteRenderer = dashEffect.GetComponent<SpriteRenderer>();
        if (effectSpriteRenderer != null)
        {
            effectSpriteRenderer.flipX = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].IsFlipX;
        }


        // dashDistance만큼을 dashDuration 시간동안 이동
        while (time < dashTime)
        {
            ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].PlayerRigidbody.MovePosition(Vector2.Lerp(startPos, targetPos, time / dashTime));
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 대시 이후 위치
        ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].PlayerRigidbody.MovePosition(targetPos);

        // 이펙트 제거
        if (dashEffect != null)
        {
            GameObject.Destroy(dashEffect, 0.5f);
        }

        // 끝/중간 위치 설정
        Vector2 endPos = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].transform.position;
        Debug.Log($"endPos = {endPos}");
        float distance = Vector2.Distance(startPos, endPos);
        Debug.Log($"distance = {distance}");

        // 콜라이더 위치 저장 이동
        Vector2 originalPos = ManagerHub.Instance.ServerManager.DictRefToPlayer[Data.PlayerRef].PlayerMeleeCollider.transform.position;

        Vector2 ColliderSize = new Vector2(distance, 1.0f);
        Debug.Log($"ColliderSize = {ColliderSize}");
        Vector2 ColliderOffset = new Vector2(-distance / 2, 0);
        Debug.Log($"ColliderOffset = {ColliderOffset}");
        UseSkillSetting(playerFlipX, ColliderSize, ColliderOffset);
    }
}
