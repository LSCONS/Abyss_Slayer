using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    PlayerInputs inputs;

    #region 프로퍼티의 백킹 필드
    private Vector2 moveDir;
    private bool isJump;
    private bool isAttack;
    private bool isDash;
    //private bool isDownJump;
    private bool isSkill1;
    private bool isSkill2;
    private bool isSkill3;
    #endregion

    #region 프로퍼티 선언
    /// <summary> 
    ///플레이어가 입력한 움직임을 벡터로 변환
    /// </summary>
    public Vector2 MoveDir { get => moveDir; }

    /// <summary>
    /// 플레이어가 점프키를 입력하고 있는지 확인
    /// </summary>
    public bool IsJump { get => isJump; }

    /// <summary>
    /// 플레이어가 어택키를 입력했는지 확인(트리거)
    /// </summary>
    public bool IsAttack { get => isAttack; }

    /// <summary>
    /// 플레이어가 대시키를 입력했는지 확인(트리거)
    /// </summary>
    public bool IsDash { get => isDash; }

    ///// <summary>
    ///// 플레이어가 아래키 + 점프를 입력했는지 확인(트리거)
    ///// </summary>
    //public bool IsDownJump { get => isDownJump; }

    /// <summary>
    /// 플레이어가 스킬1을 입력했는지 확인(트리거)
    /// </summary>
    public bool IsSkill1 { get => isSkill1; }

    /// <summary>
    /// 플레이어가 스킬2를 입력했는지 확인(트리거)
    /// </summary>
    public bool IsSkill2 { get => isSkill2; }

    /// <summary>
    /// 플레이어가 스킬3을 입력했는지 확인(트리거)
    /// </summary>
    public bool IsSkill3 { get => isSkill3; }
    #endregion


    private void Awake()
    {
        inputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        InputEvent();
    }

    private void OnDisable()
    {
        OutPutEvent();
    }


    /// <summary>
    /// 입력 이벤트 등록
    /// </summary>
    private void InputEvent()
    {
        inputs.Enable();
        inputs.Player.Move.performed += StartMove;
        inputs.Player.Move.canceled += StopMove;
        inputs.Player.Jump.started += StartJump;
        inputs.Player.Jump.canceled += StopJump;
        inputs.Player.Dash.started += StartDash;
        //inputs.Player.Jump.started += StartDownJump;
        inputs.Player.Attack.started += StartAttack;
        inputs.Player.Skill1.started += StartSkill1;
        inputs.Player.Skill2.started += StartSkill2;
        inputs.Player.Skill3.started += StartSkill3;
    }


    /// <summary>
    /// 입력 이벤트 해제
    /// </summary>
    private void OutPutEvent()
    {
        inputs.Disable();
        inputs.Player.Move.performed -= StartMove;
        inputs.Player.Move.canceled -= StopMove;
        inputs.Player.Jump.started -= StartJump;
        inputs.Player.Jump.canceled -= StopJump;
        inputs.Player.Dash.started -= StartDash;
        //inputs.Player.Jump.started -= StartDownJump;
        inputs.Player.Attack.started -= StartAttack;
        inputs.Player.Skill1.started -= StartSkill1;
        inputs.Player.Skill2.started -= StartSkill2;
        inputs.Player.Skill3.started -= StartSkill3;
    }


    /// <summary>
    /// 플레이어 움직임 감지
    /// </summary>
    private void StartMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }


    /// <summary>
    /// 플레이어 움직임 해제
    /// </summary>
    private void StopMove(InputAction.CallbackContext context)
    {
        moveDir = Vector2.zero;
    }


    /// <summary>
    /// 플레이어 점프 시작
    /// </summary>
    private void StartJump(InputAction.CallbackContext context)
    {
        isJump = true;
    }


    /// <summary>
    /// 플레이어 점프 종료
    /// </summary>
    private void StopJump(InputAction.CallbackContext context)
    {
        isJump = false;
    }


    /// <summary>
    /// 플레이어 어택 트리거
    /// </summary>
    private void StartAttack(InputAction.CallbackContext context)
    {
        StartCoroutine(CoroutineAttack());
    }


    /// <summary>
    /// 플레이어 대시 트리거
    /// </summary>
    private void StartDash(InputAction.CallbackContext context)
    {
        StartCoroutine(CoroutineDash());
    }


    ///// <summary>
    ///// 플레이어 다운점프 트리거
    ///// </summary>
    ///// <param name="context"></param>
    //private void StartDownJump(InputAction.CallbackContext context)
    //{
    //    if (MoveDir.y < 0) StartCoroutine(CoroutineDownJump());
    //}


    /// <summary>
    /// 플레이어 스킬1 트리거
    /// </summary>
    private void StartSkill1(InputAction.CallbackContext context)
    {
        StartCoroutine(CoroutineSkill1());
    }


    /// <summary>
    /// 플레이어 스킬2 트리거
    /// </summary>
    private void StartSkill2(InputAction.CallbackContext context)
    {
        StartCoroutine(CoroutineSkill2());
    }


    /// <summary>
    /// 플레이어 스킬3 트리거
    /// </summary>
    private void StartSkill3(InputAction.CallbackContext context)
    {
        StartCoroutine(CoroutineSkill3());
    }


    /// <summary>
    /// 코루틴으로 어택을 트리거 형태로 실행.
    /// </summary>
    private IEnumerator CoroutineAttack()
    {
        isAttack = true;
        yield return null;
        isAttack = false;
    }


    /// <summary>
    /// 코루틴으로 대시를 트리거 형태로 실행.
    /// </summary>
    private IEnumerator CoroutineDash()
    {
        isDash = true;
        yield return null;
        isDash = false;
    }


    ///// <summary>
    ///// 코루틴으로 다운점프를 트리거 형태로 실행
    ///// </summary>
    ///// <returns></returns>
    //private IEnumerator CoroutineDownJump()
    //{
    //    Debug.Log("정상 진입 완료");
    //    isDownJump = true;
    //    yield return null;
    //    isDownJump = false;
    //}


    /// <summary>
    /// 코루틴으로 스킬1을 트리거 형태로 실행.
    /// </summary>
    private IEnumerator CoroutineSkill1()
    {
        isSkill1 = true;
        yield return null;
        isSkill1 = false;
    }


    /// <summary>
    /// 코루틴으로 스킬2를 트리거 형태로 실행.
    /// </summary>
    private IEnumerator CoroutineSkill2()
    {
        isSkill2 = true;
        yield return null;
        isSkill2 = false;
    }


    /// <summary>
    /// 코루틴으로 스킬3를 트리거 형태로 실행.
    /// </summary>
    private IEnumerator CoroutineSkill3()
    {
        isSkill3 = true;
        yield return null;
        isSkill3 = false;
    }
}
