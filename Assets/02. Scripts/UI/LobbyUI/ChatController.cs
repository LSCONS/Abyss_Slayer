using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;   // 입력받을 애
    [SerializeField] private TextMeshProUGUI chatText;      // 채팅창 띄울 애
    [SerializeField] private ScrollRect scrollRect;         // 스크롤 내려야되니까 받아야됨

    // 엔터키 눌렀을 때 채팅 전송해야됨
    // TODO: input에 합성해야됨 지금은 getkeydown임
    private void Update()
    {
        // 엔터 누르면 채팅창에 메시지 전송하셈
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SubmitMessage();
        }
    }


    // 메시지 보내기
    private void SubmitMessage()
    {
        string message = inputField.text.Trim();

        // 기존 채팅창 텍스트에다가 한 줄 띄고 메시지 붙여야됨
        chatText.text += "\n" + message;

        // 입력필드값 지워야겠지
        inputField.text = "";

        // 스크롤 아래로 내려야겠지? 안내려가 알아서;
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
