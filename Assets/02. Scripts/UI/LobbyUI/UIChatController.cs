using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector.Libs;

public class UIChatController : MonoBehaviour
{
    [field: SerializeField] public TMP_InputField InputChatting { get; private set; }   // 입력받을 애
    [field: SerializeField] public TextMeshProUGUI TextChattingRecord { get; private set; }      // 채팅창 띄울 애
    [field: SerializeField] public ScrollRect ScrollRect { get; private set; }         // 스크롤 내려야되니까 받아야됨
    private StringBuilder textBuilder = new StringBuilder();

    // 엔터키 눌렀을 때 채팅 전송해야됨
    // TODO: input에 합성해야됨 지금은 getkeydown임

    private void Awake()
    {
        ServerManager.Instance.ChattingTextController = this;
        TextChattingRecord.text = "";
        InputChatting.onValueChanged.AddListener(ChangeInputText);
    }

    private void Update()
    {
        // 엔터 누르면 채팅창에 메시지 전송 
        // 근데 포커스 안되어있는 상태에서 엔터 누르면 포커스 되게 설정
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (!InputChatting.isFocused)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(InputChatting.gameObject);
            }
            EnterMessage();
        }
    }

    private void ChangeInputText(string text)
    {
        if(!(text.IsEmpty()))
        SoundManager.Instance.PlayTypingSoundSFX();
    }

    private void EnterMessage()
    {
        string trimmedText = InputChatting.text.Trim();

        if (string.IsNullOrEmpty(trimmedText)) return;
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
        textBuilder.Append(ServerManager.Instance.PlayerName);
        textBuilder.Append(": ");
        textBuilder.Append(trimmedText);
        textBuilder.AppendLine();
        byte[] bytes = textBuilder.ToString().StringToBytes();

        // 입력필드값 지워야겠지
        InputChatting.text = "";
        textBuilder.Clear();

        //TODO: 여기에 네트워크로 메시지 전달
        ServerManager.Instance.ThisPlayerData.Rpc_EnterToChatting(bytes);

        StartCoroutine(FocusInputNextFrame());
    }

    private IEnumerator FocusInputNextFrame()
    {
        // 현재 선택된 것 해제
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        yield return null; // 한 프레임 대기

        // 다시 선택
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(InputChatting.gameObject);
        InputChatting.ActivateInputField(); // 커서 강제 활성화
    }

    // 메시지 보내기
    public void SendChatMessage(byte[] textBytes)
    {
        // 기존 채팅창 텍스트에다가 한 줄 띄고 메시지 붙여야됨
        TextChattingRecord.text += textBytes.BytesToString();

        // 스크롤 아래로 내려야겠지? 안내려가 알아서;
        Canvas.ForceUpdateCanvases();
        ScrollRect.verticalNormalizedPosition = 0f;
    }
}
