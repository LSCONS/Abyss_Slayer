using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomPrefab : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI TextRoomName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextRoomHeadCount { get; private set; }
    [field: SerializeField] public Button BtnRoom {  get; private set; }
    public UIRoomSearch UIRoomSearch { get; private set; }
    public SessionInfo SessionInfo { get; private set; }
    public int CurrentHeadCount { get; private set; }

    public void Init(SessionInfo info, UIRoomSearch roomList)
    {
#if AllMethodDebug
        Debug.Log("Init");
#endif
        SessionInfo = info;
        TextRoomName.text = info.Name;
        UpdateHeadCountText();
        UIRoomSearch = roomList;
        BtnRoom.onClick.AddListener(OnClickRoom);
    }

    /// <summary>
    /// 인원 수 업데이트 해주는 메서드
    /// </summary>
    private void UpdateHeadCountText()
    {
#if AllMethodDebug
        Debug.Log("UpdateHeadCountText");
#endif
        CurrentHeadCount = SessionInfo.PlayerCount;
        TextRoomHeadCount.text = $"{CurrentHeadCount.ToString()} / {ServerManager.Instance.MaxHeadCount}";
    }

    /// <summary>
    /// 인원 수의 변동이 있는지 확인하고 인원 수를 업데이트 해주는 메서드
    /// </summary>
    public void CheckSameHeadCount()
    {
#if AllMethodDebug
        Debug.Log("CheckSameHeadCount");
#endif
        if (SessionInfo != null && SessionInfo.PlayerCount != CurrentHeadCount)
        {
            UpdateHeadCountText();

            //방이 꽉 찼을 경우 버튼 비활성화, 비어있을 경우 활성화
            BtnRoom.interactable = CurrentHeadCount != ServerManager.Instance.MaxHeadCount;
        }
    }

    /// <summary>
    /// 버튼을 클릭할 경우 해당 방의 정보를 보내주는 메서드
    /// </summary>
    private void OnClickRoom()
    {
#if AllMethodDebug
        Debug.Log("OnClickRoom");
#endif
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
        UIRoomSearch.SelectRoomSession = SessionInfo;
        UIRoomSearch.BtnJoin.interactable = true;
    }
}
