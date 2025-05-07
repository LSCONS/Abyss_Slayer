using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour
{

    #region 프로퍼티 선언
    /// <summary>InputSystem의 C# 제너레이터로 만든 스크립트 저장</summary>
    public PlayerInputs Inputs { get; private set; }

    /// <summary> 플레이어가 입력한 움직임을 벡터로 변환</summary>
    public Vector2 MoveDir      { get; private set; }

    /// <summary>플레이어가 점프키를 입력하고 있는지 확인</summary>
    public bool IsJump          { get; private set; }

    /// <summary>플레이어가 대시키를 입력했는지 확인</summary>
    public bool IsSkillZ        { get; private set; }

    /// <summary>플레이어가 어택키를 입력했는지 확인</summary>
    public bool IsSkillX        { get; private set; }

    /// <summary>플레이어가 스킬A을 입력했는지 확인</summary>
    public bool IsSkillA        { get; private set; }

    /// <summary>플레이어가 스킬S를 입력했는지 확인</summary>
    public bool IsSkillS        { get; private set; }

    /// <summary>플레이어가 스킬D을 입력했는지 확인</summary>
    public bool IsSkillD        { get; private set; }
    /// <summary>어떤 스킬키가 어떤 Input이랑 연결되는지 알려주는 Dictionary</summary>
    public Dictionary<SkillSlotKey, Func<bool>> SkillInputKey { get; private set; } = new();
    #endregion


    private void Awake() 
    {
        Inputs = new PlayerInputs();
    }

    public void InitDictionary()
    {
        SkillInputKey.Add(SkillSlotKey.X, () => IsSkillX);
        SkillInputKey.Add(SkillSlotKey.Z, () => IsSkillZ);
        SkillInputKey.Add(SkillSlotKey.A, () => IsSkillA);
        SkillInputKey.Add(SkillSlotKey.S, () => IsSkillS);
        SkillInputKey.Add(SkillSlotKey.D, () => IsSkillD);
    }


    /// <summary>입력 이벤트 등록</summary>
    public void InputEvent()
    {
        Inputs.Enable();
        var playerAction = Inputs.Player;
        playerAction.Move.performed     += StartMove;
        playerAction.Move.canceled      += StopMove;
        playerAction.Jump.started       += StartJump;
        playerAction.Jump.canceled      += StopJump;
        playerAction.SkillZ.started     += StartSkillZ;
        playerAction.SkillZ.canceled    += StopSkillZ;
        playerAction.SkillX.started     += StartSkillX;
        playerAction.SkillX.canceled    += StopSkillX;
        playerAction.SkillA.started     += StartSkillA;
        playerAction.SkillA.canceled    += StopSkillA;
        playerAction.SkillS.started     += StartSkillS;
        playerAction.SkillS.canceled    += StopSkillS;
        playerAction.SkillD.started     += StartSkillD;
        playerAction.SkillD.canceled    += StopSkillD;
    }


    /// <summary>입력 이벤트 해제</summary>
    public void OutPutEvent()
    {
        Inputs.Disable();
        var playerAction = Inputs.Player;
        playerAction.Move.performed     -= StartMove;
        playerAction.Move.canceled      -= StopMove;
        playerAction.Jump.started       -= StartJump;
        playerAction.Jump.canceled      -= StopJump;
        playerAction.SkillZ.started     -= StartSkillZ;
        playerAction.SkillZ.canceled    -= StopSkillZ;
        playerAction.SkillX.started     -= StartSkillX;
        playerAction.SkillX.canceled    -= StopSkillX;
        playerAction.SkillA.started     -= StartSkillA;
        playerAction.SkillA.canceled    -= StopSkillA;
        playerAction.SkillS.started     -= StartSkillS;
        playerAction.SkillS.canceled    -= StopSkillS;
        playerAction.SkillD.started     -= StartSkillD;
        playerAction.SkillD.canceled    -= StopSkillD;
    }


    /// <summary>플레이어 움직임 감지</summary>
    private void StartMove(InputAction.CallbackContext context) => MoveDir = context.ReadValue<Vector2>();
    /// <summary>플레이어 움직임 해제</summary>
    private void StopMove(InputAction.CallbackContext context) => MoveDir = Vector2.zero;


    /// <summary>플레이어 점프 시작</summary>
    private void StartJump(InputAction.CallbackContext context) => IsJump = true;
    /// <summary>플레이어 점프 종료</summary>
    private void StopJump(InputAction.CallbackContext context) => IsJump = false;


    /// <summary>플레이어 스킬Z 시작</summary>
    private void StartSkillZ(InputAction.CallbackContext context) => IsSkillZ = true;
    /// <summary>플레이어 스킬Z 종료</summary>
    private void StopSkillZ(InputAction.CallbackContext context) => IsSkillZ = false;


    /// <summary>플레이어 스킬X 시작</summary>
    private void StartSkillX(InputAction.CallbackContext context) => IsSkillX = true;
    /// <summary>플레이어 스킬X 종료</summary>
    private void StopSkillX(InputAction.CallbackContext context) => IsSkillX = false;


    /// <summary>플레이어 스킬A 시작</summary>
    private void StartSkillA(InputAction.CallbackContext context) => IsSkillA = true;
    /// <summary>플레이어 스킬A 종료</summary>
    private void StopSkillA(InputAction.CallbackContext context) => IsSkillA = false;


    /// <summary>플레이어 스킬S 시작</summary>
    private void StartSkillS(InputAction.CallbackContext context) => IsSkillS = true;
    /// <summary>플레이어 스킬S 종료</summary>
    private void StopSkillS(InputAction.CallbackContext context) => IsSkillS = false;


    /// <summary>플레이어 스킬D 시작</summary>
    private void StartSkillD(InputAction.CallbackContext context) => IsSkillD = true;
    /// <summary>플레이어 스킬D 종료</summary>
    private void StopSkillD(InputAction.CallbackContext context) => IsSkillD = false;
}
