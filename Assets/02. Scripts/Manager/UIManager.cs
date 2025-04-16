using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Canvas canvas;
    public Transform permanentParent;
    public Transform topParent;
    public Transform bottomParent;
    public Transform popupParent;


    private Stack<UIPopup> popupStack = new();

    private Dictionary<string, GameObject> UICachedMap = new();
    private Dictionary<string, GameObject> UIMap = new(); // 고정 ui 이름 맵

    // private void OnValidate() // 에디터에서 컴포넌트 변경 시 자동으로 호출됨 -> allUIList에 UIBase 가진 오브젝트들을 자동으로 추가함
    // {
    //     canvas = transform.GetGameObjectSameNameDFS("Canvas").GetComponent<Canvas>();
    //     permanentParent = transform.GetGameObjectSameNameDFS("UI_Permanent").transform;
    //     topParent = transform.GetGameObjectSameNameDFS("UI_Top").transform;
    //     bottomParent = transform.GetGameObjectSameNameDFS("UI_Bottom").transform;
    //     popupParent = transform.GetGameObjectSameNameDFS("UI_Popup").transform;
    // }

    protected override void Awake() {   
        base.Awake();
        if(canvas == null){
            canvas = transform.GetGameObjectSameNameDFS("Canvas").GetComponent<Canvas>();
            if(canvas == null) return;
        }
        if(popupParent == null){
            popupParent = new GameObject("UI_Popup").transform;
        }
        if(permanentParent == null){
            permanentParent = new GameObject("UI_Permanent").transform;
        }
        if(topParent == null){
            topParent = new GameObject("UI_Top").transform;
        }   
        if(bottomParent == null){
            bottomParent = new GameObject("UI_Bottom").transform;
        }
    }

    /// <summary>
    /// 모든 ui 초기화  
    /// </summary>
    public void Init()
    {
       // 모든 ui init 실행
       foreach(var ui in UIMap)
       {
            Debug.Log($"[Init] {ui.Key} 초기화 시작");
            ui.Value.GetComponentInChildren<UIBase>().Init();
       }
    }

    /// <summary>
    /// 라벨로 UI 로드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public async Task LoadUI(UIType type)
    {
        // 라벨로 LoadAssetsAsync 로드
        var handle = Addressables.LoadAssetsAsync<GameObject>(type.ToString(), null);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            return;
        }

        GameObject prefab = handle.Result[0];
        UICachedMap.Add(prefab.name, prefab);
    }

    // 라벨로 모든 ui 로드
    public async Task LoadAllUI(UIType type){
        Debug.Log($"[LoadAllUI] {type} 타입의 UI 로드 시작");
        var label = type.ToString();
        var handle = Addressables.LoadAssetsAsync<GameObject>(label, null);
    
        await handle.Task;

        // 로드 실패 체크
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[LoadAllUI] {type} 로드 실패");
            return;
        }

        foreach(var prefab in handle.Result){
            // 타입 찾아
            var uiType = prefab.GetComponentInChildren<UIBase>();
            if(uiType == null) continue;    // 타입이 없으면 패스

           // 이미 로드 되어있는지 확인해야됨
           if(!UICachedMap.ContainsKey(prefab.name))
            {
                UICachedMap.Add(prefab.name, prefab);
                Debug.Log($"[LoadAllUI] {prefab.name} 로드 완료");
            }
        }
    }

    /// <summary>
    /// 타입으로 UI 생성
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public void CreateUI(string name)
    {
        Debug.Log($"[CreateUI] {name} UI 생성 시작");
        // 만약에 UIMap에 있으면 패스
        if(UIMap.ContainsKey(name)){
            return;
        }
        // 그리고 만약에 UICachedMap에 없어도 패스
        if(!UICachedMap.TryGetValue(name, out var prefab)){     // 타입에 따른 프리펩 가져오기
            Debug.LogError($"[CreateUI] {name} UI가 없달까?");
            return;
        }

        var uiBase = prefab.GetComponentInChildren<UIBase>();
        // 타입에 따라 부모 선택 근데 popup / top / bottom 이렇게 3개 중에 하나여야만 함 만약 다 아니면 permanent 부모에 만들기
        Transform parent = permanentParent; // 기본
        if((uiBase.uiType & UIType.Popup) != 0){
            parent = popupParent;
        }
        else if((uiBase.uiType & UIType.Top) != 0){
            parent = topParent;
        }
        else if((uiBase.uiType & UIType.Bottom) != 0){
            parent = bottomParent;
        }

        Debug.Log($"[CreateUI] {uiBase.uiType} 타입의 UI 생성 완료");    
        // 부모에 만들기
        var ui = Instantiate(prefab, parent);
        UIMap.Add(name, ui);    // 맵에 추가
    }


    /// <summary>
    /// 원하는 타입의 ui 모두 생성  
    /// </summary>
    /// <param name="type"></param>
    public void CreateAllUI(UIType type){
        Debug.Log($"[CreateAllUI] {type} 타입의 UI 생성 시작");
        foreach(var ui in UICachedMap){
            var prefab = ui.Value;
            var uiBase = prefab.GetComponentInChildren<UIBase>();
            if(uiBase == null) continue;    // 없으면 패스
            if((uiBase.uiType & type) != 0){
                CreateUI(ui.Key);
            }
        }
    }


    /// <summary>
    /// 원하는 타입의 ui 전부 제거
    /// </summary>
    /// <param name="type"></param>
    public void ClearUI(UIType type)
    {
        List<UIBase> uiList = new();    // 제거할 ui 리스트

        foreach(Transform child in popupParent)
        {
            if(child.TryGetComponent<UIBase>(out var ui) && (ui.uiType & type) != 0)
            {
                Destroy(child.gameObject);
                uiList.Add(ui);
            }
        }
        foreach(Transform child in permanentParent)
        {
            if(child.TryGetComponent<UIBase>(out var ui) && (ui.uiType & type) != 0)
            {
                Destroy(child.gameObject);
                uiList.Add(ui);

                // uimap에서도 제거
                UIMap.Remove(ui.name);
            }
        }
        // 제거할 ui 리스트에 있는 ui 제거
        foreach(var ui in uiList){
            UIMap.Remove(ui.name);
        }
    }

    // uiscenetype 에 따른 ui 오픈
    public void OpenUI(UISceneType type){
        foreach (var ui in UIMap)
        {
            var sceneType = ui.Value.GetComponentInChildren<UIBase>().uISceneType;
            if ((type & sceneType) != 0)
            {
                ui.Value.SetActive(true);
            }
        }
    }
    
    // uiscenetype 에 따른 ui 닫기
    public void CloseUI(UISceneType type){
        foreach (var ui in UIMap)
        {
            var sceneType = ui.Value.GetComponentInChildren<UIBase>().uISceneType;
            if ((type & sceneType) != 0)
            {
                ui.Value.SetActive(false);
            }
        }
    }
    
    // 모든 고정 ui 켜기
    public void OpenAllPermanent(){
        foreach(var ui in UIMap){
            if((ui.Value.GetComponentInChildren<UIBase>().uiType & UIType.Top) != 0){
                ui.Value.SetActive(true);
            }
            else if((ui.Value.GetComponentInChildren<UIBase>().uiType & UIType.Bottom) != 0){
                ui.Value.SetActive(true);
            }     
            else if((ui.Value.GetComponentInChildren<UIBase>().uiType & UIType.Permanent) != 0){
                ui.Value.SetActive(true);
            }
        }
    }

    // 모든 고정 ui 끄기
    public void CloseAllPermanent(){
        foreach(var ui in UIMap){
            if((ui.Value.GetComponentInChildren<UIBase>().uiType & UIType.Top) != 0){
                ui.Value.SetActive(false);
            }
            else if((ui.Value.GetComponentInChildren<UIBase>().uiType & UIType.Bottom) != 0){
                ui.Value.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    public void OpenPopup(UIPopup popup)
    {
        if(popup==null) return;
        if(popupStack.Contains(popup)) return;
        popupStack.Push(popup);
        popup.Open();
    }

    /// <summary>
    /// 현재 팝업 닫기
    /// </summary>
    public void CloseCurrentPopup(UIPopup popup)
    {
        Debug.Log($"[CloseCurrentPopup] stackCount={popupStack.Count} target={popup.name}");
        if (popupStack.Count == 0) return;

        if(popupStack.Peek() == popup){
            popupStack.Pop();
            popup.Close();
        }
        else
        {
            Debug.Log("현재 팝업이 아님. 못닫는다.");
        }
    }

    public void CloseAllPopup(){
        while(popupStack.Count > 0){
            CloseCurrentPopup(popupStack.Pop());
        }
    }

    /// <summary>
    /// 지금 팝업 열려있는지 검사
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool isPopupOpen<T>() where T : UIPopup
    {
        foreach(var popup in popupStack){
            if(popup is T && popup.gameObject.activeSelf){
                return true;
            }
        }
        return false;
    }


    // ui정렬 깨짐현상을 해결하기 위한 레이아웃 빌드 딜레이
    public void DelayRebuildLayout(UIPopup popup)
    {
        StartCoroutine(DelayRebuildLayoutCoroutine(popup));
    }

    private IEnumerator DelayRebuildLayoutCoroutine(UIPopup popup)
    {
        popup.gameObject.SetActive(true);
        yield return null; // 한 프레임 대기 후

        var layoutRoot = popup.GetComponentInChildren<VerticalLayoutGroup>()?.GetComponent<RectTransform>()
                       ?? popup.GetComponent<RectTransform>();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);

    }

    /// <summary>
    /// 딕셔너리에서 팝업 이름으로 팝업 찾기
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UIPopup FindPopupByName(string name)
    {
        if (UIMap.TryGetValue(name, out var go))
        {
            return go.GetComponentInChildren<UIPopup>(true); // includeInactive = true
        }
        return null;
    }

    /// <summary>
    /// UI 탐색
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns> 
    public T GetUI<T>() where T : UIBase
    {
        foreach (var ui in UIMap)
        {
            if (ui.Value.GetComponentInChildren<T>() != null)
                return ui.Value.GetComponentInChildren<T>();
        }

        return null;
    }

}
