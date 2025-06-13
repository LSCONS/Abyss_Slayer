using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        BtnCreateRoomEnter.interactable = false;
        InputRoomName.onValueChanged.AddListener(CheckNameLenght);
        BtnCreateRoomEnter.onClick.AddListener(CreateRoom);
        BtnCreateRoomExit.onClick.AddListener(() => InputRoomName.text = string.Empty);
    }

    private void CheckNameLenght(string inputRoomName)
    {
        ManagerHub.Instance.SoundManager.PlayTypingSoundSFX();
        string temp = inputRoomName.Trim();

        if(temp.Length >= 2 && temp.Length <= 8)
        {
            if (ManagerHub.Instance.ServerManager.CheckSameRoomName(temp))
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
#if AllMethodDebug
        Debug.Log("CreateRoom");
#endif
        ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
        if(ManagerHub.Instance.ServerManager.CheckSameRoomName(roomName))
        {
            CheckNameLenght(roomName);
        }
        else
        {
            OnClose();
            ManagerHub.Instance.ServerManager.CreateRoom(roomName);
            InputRoomName.text = string.Empty;
        }
    }
}
