using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum keyAction
{
    None = 0,

    DefaultAttack= 1,
    Dash,

    Skill1 = 10,
    Skill2,
    Skill3,

    UpMove = 20,
    LeftMove,
    RightMove,
    DownMove,
    Jump,
}


public class KeyBindPanel : Singleton<KeyBindPanel>
{
    // 키 바인드 변경 이벤트
    public event Action<keyAction, KeyCode> OnKeyBindChanged;

    [Serializable]
    public class KeyBind
    {
        public keyAction actionName;       // 무슨 키랑 연결될 지
        public Button bindButton;       // 연결될 버튼
        public KeyCode defaultKey;      // 기본 키값
    }

    [SerializeField] private List<KeyBind> keys = new();
    [SerializeField] private Button resetButton;

    private Dictionary<keyAction, Button> keyBindButtonMap = new();
    private Dictionary<keyAction, KeyCode> inputBindKeyMap = new();    // 입력해서 바뀐 값


    // 키 기다리기
    private bool isWaitingForKey = false;
    // 지금 키
    private keyAction curBindAction = keyAction.None;

    // 이전 키 저장(복구용)
    private KeyCode prevKey;

    // 한 번만 초기화 해야됨
    private bool initSettingKey = false;

    // 맵에다가 리스너 붙이기
    protected override void Awake()
    {
        if (initSettingKey) return;

        foreach (var key in keys)
        {
            keyBindButtonMap[key.actionName] = key.bindButton;
        }

        LoadKeyBinds();

        // 초기화 => 플레이어 프리팹에 저장되어있는 애 가져오기
        foreach (var key in keys)
        {
            // 리스너 등록
            key.bindButton.onClick.AddListener(() =>
            {
                StartRebind(key.actionName);
            });
        }

        try
        {
            resetButton.onClick.AddListener(ResetAllKeys);
        }
        catch { }

        initSettingKey = true;
    }

    // 일단 업데이트에 박아두고 리팩하자
    private void Update()
    {
        UpdateBinds();
    }

    private void UpdateBinds()
    {
        // 키 기다려
        if (!isWaitingForKey) return;

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                // esc누르면 취소
                if (key == KeyCode.Escape)
                {
                    CancledKey();
                    break;
                }

                // 키가 중복인지 확인
                foreach (var inputKey in inputBindKeyMap)
                {
                    if (inputKey.Key != curBindAction && inputKey.Value == key)
                    {
                        inputBindKeyMap[inputKey.Key] = KeyCode.None;
                        SetButtonText(keyBindButtonMap[inputKey.Key], "None");
                        OnKeyBindChanged?.Invoke(inputKey.Key, KeyCode.None);
                        break;
                    }
                }

                // 키 바인드 하기
                // 키를 저장해
                inputBindKeyMap[curBindAction] = key;

                // 버튼 텍스트를 바꿔
                SetButtonText(keyBindButtonMap[curBindAction], key.ToString());

                // 이벤트 발생
                OnKeyBindChanged?.Invoke(curBindAction, key);

                // 상태 초기화
                isWaitingForKey = false;
                curBindAction = keyAction.None;

                // 스킬 슬롯에 키 설정
                UISkillSlotManager.Instance.SettingKeySlot();

                // 플레이어 프리팹에도 저장
                SaveKeyBinds();
                break;
            }
        }
    }

    // 버튼 텍스트 해주는 메서드
    private void SetButtonText(Button button, string newText)
    {
        if(button == null) return;

        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null) text.text = newText;
    }

    // 다시 바인드 하는 메서드
    private void StartRebind(keyAction actionName)
    {
        if (isWaitingForKey) return;

        isWaitingForKey = true;
        curBindAction = actionName;
        prevKey = inputBindKeyMap[actionName];

        var button = keyBindButtonMap[actionName];
        SetButtonText(button, "키를 입력해주세요.");
    }

    // 취소하면 이전 키로 돌리기
    private void CancledKey()
    {
        // 취소되면 텍스트 다시 돌려놓기
        SetButtonText(keyBindButtonMap[curBindAction], prevKey.ToString());
        // 다 돌리기
        isWaitingForKey = false;
        curBindAction = keyAction.None;
    }

    private void ResetAllKeys()
    {
        foreach(var key in keys)
        {
            inputBindKeyMap[key.actionName] = key.defaultKey;
            SetButtonText(key.bindButton, key.defaultKey.ToString());
        }
        SaveKeyBinds();
    }

    /// <summary>
    /// 액션 이름으로 키코드 반환시켜주는 메서드
    /// </summary>
    /// <param name="actionName">keyAction enum으로 가져옴</param>
    /// <returns></returns>
    public KeyCode GetKeyCode(keyAction actionName)
    {
        if(inputBindKeyMap.TryGetValue(actionName, out var key)) return key;    

        return KeyCode.None;
    }

    /// <summary>
    /// 플레이어 프리팹에 키 저장
    /// </summary>
    private void SaveKeyBinds()
    {
        foreach(var pair in inputBindKeyMap)
        {
            PlayerPrefs.SetString($"KeyBind_{pair.Key}", pair.Value.ToString());
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 저장한 플레이어 프리팹 로드하기
    /// </summary>
    public void LoadKeyBinds()
    {
        foreach (var key in keys)
        {
            string prefkey = $"KeyBind_{key.actionName}";
            if(PlayerPrefs.HasKey(prefkey))
            {
                if(Enum.TryParse(PlayerPrefs.GetString(prefkey), ignoreCase: true, out KeyCode savedKey))
                {
                    inputBindKeyMap[key.actionName] = savedKey;
                    SetButtonText(key.bindButton, savedKey.ToString());
                }
            }
            else
            {
                inputBindKeyMap[key.actionName] = key.defaultKey;
                SetButtonText(key.bindButton, key.defaultKey.ToString());
            }
        }
    }
}

public static class KeyBindStorage
{
    public static Dictionary<keyAction, KeyCode> Load()
    {
        var dict = new Dictionary<keyAction, KeyCode>();
        foreach (keyAction action in Enum.GetValues(typeof(keyAction)))
        {
            if (action == keyAction.None) continue;

            string prefKey = $"KeyBind_{action}";
            if (PlayerPrefs.HasKey(prefKey))
            {
                if (Enum.TryParse(PlayerPrefs.GetString(prefKey), ignoreCase: true, out KeyCode key))
                {
                    dict[action] = key;
                }
            }
        }
        return dict;
    }
}
