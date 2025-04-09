using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInput input;
    public Rigidbody2D playerRigidbody;
    public PlayerCheckGround playerCheckGround;
    public CinemachineVirtualCamera mainCamera;//TODO: 나중에 초기화 필요
    public Animator PlayerAnimator { get; private set; }//TODO: 나중에 초기화 필요
    public SkillSet skillSet; // 스킬셋 데이터
    public Dictionary<SkillSlotKey, SkillData> equippedSkills = new(); // 스킬 연결용 딕셔너리
    private PlayerStateMachine playerStateMachine;
    public BoxCollider2D playerGroundCollider;
    public SpriteRenderer playerSpriteRenderer;

    [field: SerializeField] public PlayerData playerData { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    private void Awake()
    {
        playerData = Resources.Load<PlayerData>("Player/Player");
        InitSkilData();
        input = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCheckGround = transform.GetComponentForTransformFindName<PlayerCheckGround>("Collider_GroundCheck");
        playerGroundCollider = transform.GetComponentForTransformFindName<BoxCollider2D>("Collider_GroundCheck");
        playerSpriteRenderer = transform.GetComponentForTransformFindName<SpriteRenderer>("Sprtie_Player");
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerStateMachine = new PlayerStateMachine(this);
        playerCheckGround.playerTriggerOff += PlayerColliderTriggerOff;
    }

    private void Start()
    {
        playerStateMachine.ChangeState(playerStateMachine.IdleState);
    }

    private void InitSkilData()
    {
        equippedSkills = new();
        foreach (var slot in skillSet.skillSlots)
        {
            if (slot.skillData != null)
            {
                equippedSkills[slot.key] = slot.skillData;
            }
            else
            {
                Debug.LogWarning($"Skill in slot {slot.key} is null!");
            }
        }
    }

    private void Update()
    {
        playerStateMachine.Update();
        playerStateMachine.HandleInput();

        if (Input.GetKey(KeyCode.S))
        {
            UseSkill(SkillSlotKey.S);
        }
    }


    private void FixedUpdate()
    {
        playerStateMachine.FixedUpdate();
    }


    /// <summary>
    /// 스킬의 쿨타임을 돌릴 메서드
    /// </summary>
    /// <param name="slotKey">쿨타임 돌릴 스킬 키</param>
    public void SkillCoolTimeUpdate(SkillSlotKey slotKey)
    {
        Debug.Log("쿨타임 계산 시작");
        //TODO: 해당 슬롯키와 맞는 스킬 키에서 쿨타임을 가져옴
        //TODO: 임시로 대시만 확인하기 위해 대시 쿨타임을 사용함
        float coolTime;
        if (equippedSkills.ContainsKey(slotKey))
        {
            SkillData skillData = equippedSkills[slotKey];
            coolTime = skillData.coolTime;
        }
        else
        {
            Debug.LogError($"{slotKey}에 대한 정보를 찾을 수 없습니다. (송제우: Z에 대한 정보면 무시해도 됩니다.)");
            coolTime = 0.5f;
        }
        Debug.Log("coolTime = " + coolTime);
        StartCoroutine(SkillCoolTimeUpdateCoroutine(coolTime, slotKey));
    }

    /// <summary>
    /// 스킬을 사용하는 메서드
    /// </summary>
    /// <param name="key">사용할 스킬 키</param>
    public void UseSkill(SkillSlotKey key)
    {
        if (equippedSkills == null)
        {
            Debug.LogError("equippedSkills is null!");
            return;
        }

        // 해당 키의 스킬을 찾고 있다면 실행
        if (equippedSkills.TryGetValue(key, out var skill))
        {
            skill.Execute(this, null);
        }
    }

    /// <summary>
    /// 스킬의 쿨타임을 계산하고 다시 사용 가능하게 바꿔주는 코루틴
    /// </summary>
    /// <param name="coolTIme">쿨타임 시간</param>
    /// <param name="slotKey">초기화 시킬 스킬 키</param>
    private IEnumerator SkillCoolTimeUpdateCoroutine(float coolTIme, SkillSlotKey slotKey)
    {
        float temp = 0;
        while (temp <= coolTIme)
        {
            temp += Time.deltaTime;
            yield return null;
        }

        switch (slotKey)
        {
            case SkillSlotKey.X:
                equippedSkills[SkillSlotKey.X].canUse = true;
                break;
            case SkillSlotKey.Z:
                playerData.PlayerAirData.CanDash = true;
                break;
            case SkillSlotKey.A:
                break;
            case SkillSlotKey.S:
                break;
            case SkillSlotKey.D:
                break;
        }
    }

    /// <summary>
    /// 플레이어가 다운점프를 하고 빠져나온 뒤 isTrigger을 false로 되돌리는 메서드
    /// </summary>
    private void PlayerColliderTriggerOff()
    {
        playerGroundCollider.isTrigger = false;
    }
}
