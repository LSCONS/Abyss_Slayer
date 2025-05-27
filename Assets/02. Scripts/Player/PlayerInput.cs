using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Player player; // Player 컴포넌트 참조

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
    public bool IsConnectInput { get; private set; }

    /// <summary>현재 키 셋팅 설정</summary>
    private Dictionary<keyAction, InputAction> keySettings = new Dictionary<keyAction, InputAction>();
    #endregion


    private void Awake() 
    {
        Inputs = new PlayerInputs();
        InitializeKeySettings();

        ApplyLoadedKeyBinds();  // 플레이어 프리팹에 있는거 불러온걸로 업데이트
    }

    // 키 설정 초기화
    private void InitializeKeySettings()
    {
        var playerAction = Inputs.Player;
        keySettings[keyAction.DefaultAttack] = playerAction.SkillX;
        keySettings[keyAction.Dash] = playerAction.SkillZ;
        keySettings[keyAction.Skill1] = playerAction.SkillA;
        keySettings[keyAction.Skill2] = playerAction.SkillS;
        keySettings[keyAction.Skill3] = playerAction.SkillD;
        keySettings[keyAction.Jump] = playerAction.Jump;
    }


    /// <summary>입력 이벤트 등록</summary>
    public void InputEvent()
    {
        Inputs.Enable();
        var playerAction = Inputs.Player;
        IsConnectInput = true;
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

        // 키 바인드 변경 이벤트 구독
        KeyBindPanel.Instance.OnKeyBindChanged += UpdateKeyBinding;
    }


    /// <summary>입력 이벤트 해제</summary>
    public void OutPutEvent()
    {
        Inputs.Disable();

        MoveDir = Vector2.zero;
        IsJump = false;
        IsSkillA = false;
        IsSkillD = false;
        IsSkillS = false;
        IsSkillX = false;
        IsSkillZ = false;

        var playerAction = Inputs.Player;
        IsConnectInput = false;
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

        // 키 바인드 변경 이벤트 구독 해제
        if (KeyBindPanel.Instance != null)
        {
            KeyBindPanel.Instance.OnKeyBindChanged -= UpdateKeyBinding;
        }
    }

    /// <summary>
    /// 키 바인드 변경 이벤트 업데이트
    /// </summary>
    /// <param name="action">변경할 키 액션</param>
    /// <param name="newKey">바인딩할 키코드</param>
    private void UpdateKeyBinding(keyAction action, KeyCode newKey)
    {
        if (keySettings.TryGetValue(action, out var inputAction))
        {
            // InputSystem의 키 바인딩 업데이트
            var binding = inputAction.bindings[0]; // 바인딩 첫번째 인덱스 = 키보드 입력 바인딩
            var newPath = $"<Keyboard>/{newKey}"; // 키 바인딩 경로
            inputAction.ChangeBinding(0).WithPath(newPath); // 키 바인딩 경로 변경
        }
    }

    /// <summary>
    /// 플레이어 프리팹에 있는 걸로 키 바인딩 변경
    /// </summary>
    private void ApplyLoadedKeyBinds()
    {
        foreach (var key in keySettings)
        {
            keyAction action = key.Key;
            InputAction inputAction = key.Value;

            KeyCode loadedKey = KeyBindPanel.Instance.GetKeyCode(action);
            if (loadedKey == KeyCode.None) continue;

            // InputSystem의 키 바인딩 업데이트
            var newPath = $"<Keyboard>/{loadedKey.ToString().ToLower()}"; // 키 바인딩 경로
            inputAction.ChangeBinding(0).WithPath(newPath); // 키 바인딩 경로 변경

        }
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
