using Fusion;
using System.Collections;
using System.Collections.Generic;
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
    [field: SerializeField] public Dictionary<SessionInfo, UIRoomPrefab> DictSessionObject { get; private set; } = new();
    [field: SerializeField] public UIRoomPrefab RoomPrefabs { get; private set; }
    public SessionInfo SelectRoomSession { get; set; } = null;
    
    private async void Awake()
    {
        ServerManager.Instance.RoomSearch = this;
        BtnJoin.onClick.AddListener(TryJoinRoom);
        BtnSearchAgain.onClick.AddListener(UpdateRoomList);
        var data = Addressables.LoadAssetAsync<GameObject>("RoomPrefab");
        await data.Task;
        RoomPrefabs = data.Result.GetComponent<UIRoomPrefab>();
    }

    private void OnEnable()
    {
        BtnJoin.interactable = false;
    }


    /// <summary>
    /// 서버에 저장된 세션 정보를 기준으로 새로고침 해주는 메서드
    /// </summary>
    public void UpdateRoomList()
    {
        foreach (SessionInfo sessionInfo in ServerManager.Instance.CurrentSessionList)
        {
            if (DictSessionObject.ContainsKey(sessionInfo))
            {
                //인원수가 변했는지 확인하고 텍스트 업데이트
                DictSessionObject[sessionInfo].CheckSameHeadCount();
            }
            else
            {
                //없는 session일 경우 딕셔너리에 프리팹을 추가함.
                DictSessionObject[sessionInfo] = Instantiate(RoomPrefabs, TrRoomList);
                DictSessionObject[sessionInfo].Init(sessionInfo, this);
            }
        }

        //목록 세션이 매개변수 세션보다 많다면 삭제할 세션을 찾고 목록에서 제거
        if(DictSessionObject.Count > ServerManager.Instance.CurrentSessionList.Count)
        {
            RemoveRoomList(ServerManager.Instance.CurrentSessionList);
        }
    }

    /// <summary>
    /// List에서 사라진 세션을 Dict에서 삭제하는 메서드
    /// </summary>
    /// <param name="sessionInfos"></param>
    private void RemoveRoomList(List<SessionInfo> sessionInfos)
    {
        var sessionSet = new HashSet<SessionInfo>(sessionInfos);
        var keysRemove = new List<SessionInfo>();

        foreach(SessionInfo key in DictSessionObject.Keys)
        {
            if (!sessionSet.Contains(key))
                keysRemove.Add(key);
        }

        foreach(SessionInfo key in keysRemove)
        {
            UIRoomPrefab obj = DictSessionObject[key];
            Destroy(obj.gameObject);
            DictSessionObject.Remove(key);
        }
    }


    /// <summary>
    /// 선택한 방에 참가하기를 시도하는 메서드
    /// </summary>
    private void TryJoinRoom()
    {
        //새로고침 한 번 실행
        UpdateRoomList();
        if(SelectRoomSession.PlayerCount < ServerManager.Instance.MaxHeadCount)
        {
            ServerManager.Instance.JoinRoom(SelectRoomSession);
        }
    }
}
