using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;


/// <summary>
/// 방 참가 팝업에서 방을 선택하고 관리하는 UI
/// </summary>
public class UIRoomSearch : UIPopup
{
    //입장하기 버튼
    [field: SerializeField] public Button BtnJoin {  get; private set; }
    //새로고침 버튼
    [field: SerializeField] public Button BtnSearchAgain { get; private set; }
    //돌아가기 버튼
    [field: SerializeField] public Button BtnExit { get; private set; }
    //룸 리스트를 담을 오브젝트 트랜스폼
    [field: SerializeField] public Transform TrRoomList { get; private set; }
    //생성한 룸을 오브젝트 형태로 저장할 딕셔너리
    [field: SerializeField] public Dictionary<SessionInfo, UIRoomPrefab> DictSessionToRoom { get; private set; } = new();
    [field: SerializeField] public UIRoomPrefab RoomPrefabs { get; private set; }
    public SessionInfo SelectRoomSession { get; set; } = null;
    
    private async void Awake()
    {
#if AllMethodDebug
        Debug.Log("Awake");
#endif
        ManagerHub.Instance.UIConnectManager.UIRoomSearch = this;
        BtnJoin.onClick.AddListener(TryJoinRoom);
        BtnSearchAgain.onClick.AddListener(UpdateRoomList);
        BtnSearchAgain.onClick.AddListener(()=>ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick));
        var data = Addressables.LoadAssetAsync<GameObject>("RoomPrefab");
        await data.Task;
        RoomPrefabs = data.Result.GetComponent<UIRoomPrefab>();
    }

    private void OnEnable()
    {
#if AllMethodDebug
        Debug.Log("OnEnable");
#endif
        BtnJoin.interactable = false;
    }


    /// <summary>
    /// 서버에 저장된 세션 정보를 기준으로 새로고침 해주는 메서드
    /// </summary>
    public void UpdateRoomList()
    {
#if AllMethodDebug
        Debug.Log("UpdateRoomList");
#endif
        foreach (SessionInfo sessionInfo in ManagerHub.Instance.ServerManager.CurrentSessionList)
        {
            if (DictSessionToRoom.ContainsKey(sessionInfo))
            {
                //인원수가 변했는지 확인하고 텍스트 업데이트
                DictSessionToRoom[sessionInfo].CheckSameHeadCount();
            }
            else
            {
                //없는 session일 경우 딕셔너리에 프리팹을 추가함.
                DictSessionToRoom[sessionInfo] = Instantiate(RoomPrefabs, TrRoomList);
                DictSessionToRoom[sessionInfo].Init(sessionInfo, this);
            }
        }

        //목록 세션이 매개변수 세션보다 많다면 삭제할 세션을 찾고 목록에서 제거
        if(DictSessionToRoom.Count > ManagerHub.Instance.ServerManager.CurrentSessionList.Count)
        {
            RemoveRoomList(ManagerHub.Instance.ServerManager.CurrentSessionList);
        }


        foreach (SessionInfo sessionInfo in ManagerHub.Instance.ServerManager.CurrentSessionList)
        {
            if(!(sessionInfo.IsOpen) || sessionInfo.PlayerCount >= ManagerHub.Instance.ServerManager.MaxHeadCount)
            {
                DictSessionToRoom[sessionInfo].BtnRoom.interactable = false;
                if (SelectRoomSession == sessionInfo)
                {
                    SelectRoomSession = null;
                    BtnJoin.interactable = false;
                }
            }
            else
            {
                DictSessionToRoom[sessionInfo].BtnRoom.interactable = true;
            }
        }
    }

    /// <summary>
    /// List에서 사라진 세션을 Dict에서 삭제하는 메서드
    /// </summary>
    /// <param name="sessionInfos"></param>
    private void RemoveRoomList(List<SessionInfo> sessionInfos)
    {
#if AllMethodDebug
        Debug.Log("RemoveRoomList");
#endif
        var sessionSet = new HashSet<SessionInfo>(sessionInfos);
        var keysRemove = new List<SessionInfo>();

        foreach(SessionInfo key in DictSessionToRoom.Keys)
        {
            if (!sessionSet.Contains(key))
                keysRemove.Add(key);
        }

        foreach(SessionInfo key in keysRemove)
        {
            UIRoomPrefab obj = DictSessionToRoom[key];
            Destroy(obj.gameObject);
            DictSessionToRoom.Remove(key);
        }
    }


    /// <summary>
    /// 선택한 방에 참가하기를 시도하는 메서드
    /// </summary>
    private void TryJoinRoom()
    {
#if AllMethodDebug
        Debug.Log("TryJoinRoom");
#endif
        ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
        //새로고침 한 번 실행
        UpdateRoomList();

        if (SelectRoomSession.PlayerCount >= ManagerHub.Instance.ServerManager.MaxHeadCount)
        {
            //TODO: 인원 수 초과 알림 필요
            string strWarningMaxPlayerCount = "방의 플레이어 제한 수가 가득 찼습니다.";
            UIPopup popup = ManagerHub.Instance.UIManager.GetPopup("WarningGameVersionPopup");
            popup.desc.text = strWarningMaxPlayerCount;
            ManagerHub.Instance.UIManager.OpenPopup(popup);
            return;
        }

        if (!(SelectRoomSession.IsOpen))
        {
            //TODO: 이미 시작하거나 파괴된 방입니다.
            string strWarningNotOpenRoom = "이미 시작하거나 파괴된 방입니다.";
            UIPopup popup = ManagerHub.Instance.UIManager.GetPopup("WarningGameVersionPopup");
            popup.desc.text = strWarningNotOpenRoom;
            ManagerHub.Instance.UIManager.OpenPopup(popup);
            return;
        }

        if(!(SelectRoomSession.Properties.TryGetValue(ManagerHub.Instance.ServerManager.StrHostVersionKey, out SessionProperty value)))
        {
            //TODO: 호스트의 버전을 확인할 수 없다는 알림 필요
            string strWarningNotUseVersion = "호스트의 버전을 찾을 수 없습니다.";
            UIPopup popup = ManagerHub.Instance.UIManager.GetPopup("WarningGameVersionPopup");
            popup.desc.text = strWarningNotUseVersion;
            ManagerHub.Instance.UIManager.OpenPopup(popup);
            return;
        }

        string strHostVersion = (string)value;
        if (ManagerHub.Instance.ServerManager.ServerVersion != strHostVersion)
        {
            //TODO: 호스트와의 버전이 다르다는 알림 필요
            string strWarningNotSameVersion = $"호스트와 게임 버전이 달라\r\n접속할 수 없습니다.\r\n\r\n요구 버전: {strHostVersion}V\r\n현재 버전: {ManagerHub.Instance.ServerManager.ServerVersion}V";
            UIPopup popup = ManagerHub.Instance.UIManager.GetPopup("WarningGameVersionPopup");
            popup.desc.text = strWarningNotSameVersion;
            ManagerHub.Instance.UIManager.OpenPopup(popup);
            return;
        }

        ManagerHub.Instance.ServerManager.JoinRoom(SelectRoomSession);
    }
}
