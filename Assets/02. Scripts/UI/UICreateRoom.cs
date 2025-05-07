using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICreateRoom : UIPopup
{
    [field: SerializeField] public Button BtnCreateRoomEnter {  get; private set; }
    [field: SerializeField] public Button BtnCreateRoomExit { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextNameError { get; private set; }
    [field: SerializeField] public TMP_InputField InputRoomName { get; private set; }
    private string roomName = string.Empty;

    private void Awake()
    {
        InputRoomName.onValueChanged.AddListener(CheckNameLenght);
        BtnCreateRoomEnter.onClick.AddListener(CreateRoom);
        BtnCreateRoomExit.onClick.AddListener(() => InputRoomName.text = string.Empty);
    }

    private void CheckNameLenght(string inputRoomName)
    {
        string temp = inputRoomName.Trim();

        if(temp.Length >= 2 && temp.Length <= 8)
        {
            if (ServerManager.Instance.CheckSameRoomName(temp))
            {
                TextNameError.text = "이미 생성된 방 이름입니다.";
                TextNameError.color = Color.red;
                BtnCreateRoomEnter.interactable = false;
            }
            else
            {
                TextNameError.text = "생성할 수 있는 이름입니다!";
                TextNameError.color = Color.black;
                BtnCreateRoomEnter.interactable = true;
                roomName = temp;
            }
        }
        else
        {
            TextNameError.text = "2 ~ 8자 이내로 입력해 주세요.";
            TextNameError.color = Color.red;
            BtnCreateRoomEnter.interactable = false;
        }
    }

    private void CreateRoom()
    {
        Debug.Log($"방 이름 : {roomName}");
        if(ServerManager.Instance.CheckSameRoomName(roomName))
        {
            CheckNameLenght(roomName);
        }
        else
        {
            ServerManager.Instance.CreateRoom(roomName);
        }
    }
}
