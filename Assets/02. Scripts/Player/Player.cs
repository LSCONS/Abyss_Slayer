using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    public PlayerInput input;
    public Rigidbody2D playerRigidbody;
    public PlayerCheckGround playerCheckGround;
    public CinemachineVirtualCamera mainCamera;//TODO: 나중에 초기화 필요
    public Animator PlayerAnimator { get; private set; }//TODO: 나중에 초기화 필요
    public SkillSet skillSet;
    private Dictionary<SkillSlotKey, SkillData> equippedSkills = new(); // 임시
    private PlayerStateMachine playerStateMachine;

    [field: SerializeField]public PlayerData playerData { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    private void Awake()
    {
        playerData = Resources.Load<PlayerData>("Player/Player");
        input = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCheckGround = transform.GetComponentForTransformFindName<PlayerCheckGround>("Collider_GroundCheck");
        playerStateMachine = new PlayerStateMachine(this);
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        equippedSkills = new();
        playerStateMachine.ChangeState(playerStateMachine.IdleState);
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

        // 임시
        if (Input.GetKeyDown(KeyCode.X))
        {
            UseSkill(SkillSlotKey.X);
        }
    }

    private void FixedUpdate()
    {
        playerStateMachine.FixedUpdate();
    }

    public void SkillCoolTimeUpdate(SkillSlotKey slotKey)
    {
        Debug.Log("쿨타임 계산 시작");
        //TODO: 해당 슬롯키와 맞는 스킬 키에서 쿨타임을 가져옴
        //TODO: 임시로 대시만 확인하기 위해 대시 쿨타임을 사용함
        float coolTime = playerData.PlayerAirData.DashCoolTime;
        Debug.Log("coolTime = " +  coolTime);
        StartCoroutine(SkillCoolTimeUpdateCoroutine(coolTime, slotKey));
    }

    // 임시
    public void UseSkill(SkillSlotKey key)
    {
        if (equippedSkills == null)
        {
            Debug.LogError("equippedSkills is null!");
            return;
        }

        if (equippedSkills.TryGetValue(key, out var skill))
        {
            skill.Execute(this, null);
        }
    }

    private IEnumerator SkillCoolTimeUpdateCoroutine(float coolTIme, SkillSlotKey slotKey)
    {
        float temp = 0;
        while (temp <= coolTIme)
        {
            temp += Time.deltaTime;
            yield return null;
        }

        switch(slotKey)
        {
            case SkillSlotKey.X:
                break;
            case SkillSlotKey.Z:
                playerData.PlayerAirData.CanDash = true;
                Debug.Log("쿨타임 초기화 종료");
                break;
            case SkillSlotKey.A:
                break;
            case SkillSlotKey.S:
                break;
            case SkillSlotKey.D:
                break;
        }
    }
}
