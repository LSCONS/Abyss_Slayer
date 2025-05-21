using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.VisualScripting;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Canvas canvas;
    public Transform permanentParent;
    public Transform background;
    public Transform topParent;
    public Transform bottomParent;
    public Transform popupParent;
    public Transform topMidParent;
    public Transform followParent;
    public GameObject popupBG;


    public Stack<UIPopup> popupStack = new();

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

    public void CloseAllUIPopup()
    {
        foreach(var popup in popupStack)
        {
            popup.OnClose();
        }
    }

    protected override void Awake() {   
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 모든 ui 초기화  
    /// </summary>
    public Task Init()
    {
       // 모든 ui init 실행
       foreach(var ui in UIMap)
       {
            if (ui.Value == null) continue;
            // Debug.Log($"[Init] {ui.Key} 초기화 시작");
            ui.Value.GetComponentInChildren<UIBase>(true).Init();
       }
       return Task.CompletedTask;
    }
    public void Start()
    {
        if (canvas == null)
        {
            canvas = transform.GetGameObjectSameNameDFS("Canvas").GetComponent<Canvas>();
            if (canvas == null) return;
        }
        if (popupParent == null)
        {
            popupParent = canvas.transform.GetGameObjectSameNameDFS("UI_Popup")?.transform
                    ?? new GameObject("UI_Popup", typeof(RectTransform)).transform;
        }
        if (permanentParent == null)
        {
            permanentParent = canvas.transform.GetGameObjectSameNameDFS("UI_Permanent")?.transform
                      ?? new GameObject("UI_Permanent", typeof(RectTransform)).transform;
        }
        if (background == null)
        {
            background = canvas.transform.GetGameObjectSameNameDFS("UI_Background")?.transform
                      ?? new GameObject("UI_Background", typeof(RectTransform)).transform;
        }
        if (topParent == null)
        {
            topParent = canvas.transform.GetGameObjectSameNameDFS("UI_Top")?.transform
                 ?? new GameObject("UI_Top", typeof(RectTransform)).transform;
        }
        if (bottomParent == null)
        {
            bottomParent = canvas.transform.GetGameObjectSameNameDFS("UI_Bottom")?.transform
                    ?? new GameObject("UI_Bottom", typeof(RectTransform)).transform;
        }
        if (topMidParent == null)
        {
            topMidParent = canvas.transform.GetGameObjectSameNameDFS("UI_TopMid")?.transform
                    ?? new GameObject("UI_TopMid", typeof(RectTransform)).transform;
        }
        if (followParent == null)
        {
            followParent = canvas.transform.GetGameObjectSameNameDFS("UI_Follow")?.transform
                    ?? new GameObject("UI_Follow", typeof(RectTransform)).transform;
        }
        if(popupBG == null)
        {
            popupBG = canvas.transform.GetGameObjectSameNameDFS("PopupBG").gameObject;
            if (popupBG != null) return;
            var bgGO = new GameObject("PopupBG", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            popupBG = bgGO;

            var rect = popupBG.GetComponent<RectTransform>();
            rect.SetParent(background, false);   // 부모를 popupParent로 설정
            rect.anchorMin = Vector2.zero; 
            rect.anchorMax = Vector2.one;    
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;

            var img = popupBG.GetComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.627f);
            img.raycastTarget = true;

            popupBG.SetActive(false);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (popupStack.Count > 0)
            {
                TryCloseTopPopup();
            }
            else
            {
                TryOpenEscPopup(); // escCanOpen 팝업 열기 시도
            }
        }
    }
    public void TryCloseTopPopup()
    {
        if (popupStack.Count > 0)
        {
            var topPopup = popupStack.Peek();
            topPopup.Close();
            CloseCurrentPopup(topPopup); // 스택에서 제거
        }
    }

    /// <summary>
    /// 팝업 닫기 시도(스택에 팝업 있을 때)
    /// </summary>
    private void TryOpenEscPopup()
    {
        foreach (var ui in UIMap)
        {
            // popupButton이 활성화되지 않은 상태에서도 접근 가능하게 true 설정
            var popupButton = ui.Value.GetComponentInChildren<UIPopupButton>(true);
            if (popupButton == null || !popupButton.EscCanOpen()) continue;

            var popup = popupButton.GetPopup(); // 내부에서 다시 FindPopupByName 호출함
            if (popup == null) continue;

            if (popupStack.Contains(popup))
            {
                CloseCurrentPopup(popup);
            }
            else
            {
                OpenPopup(popup);
            }

            break;
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
        // Debug.Log($"[LoadAllUI] {type} 타입의 UI 로드 시작");
        var label = type.ToString();
        var handle = Addressables.LoadAssetsAsync<GameObject>(label, null);
    
        await handle.Task;

        // 로드 실패 체크
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            // Debug.LogError($"[LoadAllUI] {type} 로드 실패");
            return;
        }
        

        foreach(var prefab in handle.Result){
            // 타입 찾아
            var uiType = prefab.GetComponentInChildren<UIBase>(true);
            if(uiType == null) continue;    // 타입이 없으면 패스

           // 이미 로드 되어있는지 확인해야됨
           if(!UICachedMap.ContainsKey(prefab.name))
            {
                UICachedMap.Add(prefab.name, prefab);
               // Debug.Log($"[LoadAllUI] {prefab.name} 로드 완료");
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
        // Debug.Log($"[CreateUI] {name} UI 생성 시작");
        // 만약에 UIMap에 있으면 패스
        if(UIMap.ContainsKey(name)){
            // Debug.LogError($"{name}이 중복되었어...");
            return;
        }
        // 그리고 만약에 UICachedMap에 없어도 패스
        if(!UICachedMap.TryGetValue(name, out var prefab)){     // 타입에 따른 프리펩 가져오기
            // Debug.LogError($"[CreateUI] {name} UI가 없달까?");
            return;
        }


        var uiBase = prefab.GetComponentInChildren<UIBase>(true);

        // 타입에 따라 부모 선택 근데 popup / top / bottom 이렇게 3개 중에 하나여야만 함 만약 다 아니면 permanent 부모에 만들기
        Transform parent = permanentParent; // 기본
        if((uiBase.uiType & UIType.Popup) != 0){
            parent = popupParent;
        }
        else if ((uiBase.uiType & UIType.Background) != 0)
        {
            parent = background;
        }
        else if((uiBase.uiType & UIType.Top) != 0){
            parent = topParent;
        }
        else if((uiBase.uiType & UIType.Bottom) != 0){
            parent = bottomParent;
        }
        else if ((uiBase.uiType & UIType.TopMid) != 0)
        {
            parent = topMidParent;
        }
        else if ((uiBase.uiType & UIType.Follow) != 0)
        {
            parent = followParent;
        }

        // Debug.Log($"[CreateUI] {uiBase.uiType} 타입의 UI 생성 완료");    
        // 부모에 만들기
        var ui = Instantiate(prefab, parent);
        ui.name = name; // 원래 프리팹 이름으로 나오게 설정 => clone 안뜨게
        UIMap.Add(name, ui);    // 맵에 추가
        CreateAllUnder(ui.transform);
    }


    /// <summary>
    /// 원하는 타입의 ui 모두 생성  
    /// </summary>
    /// <param name="type"></param>
    public void CreateAllUI(UIType type){
       // Debug.Log($"[CreateAllUI] {type} 타입의 UI 생성 시작");
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
    /// 자식에 있는 uibase도 가져와서 uimap에 저장하기
    /// </summary>
    /// <param name="parent"></param>
    public void CreateAllUnder(Transform parent)
    {
        var uiBases = parent.GetComponentsInChildren<UIBase>(true);
        foreach (var uiBase in uiBases)
        {
            string name = uiBase.gameObject.name;
            if (UIMap.ContainsKey(name)) continue;

            // 이미 씬에 존재하는 오브젝트면 따로 Instantiate 필요 없음
            UIMap.Add(name, uiBase.gameObject);
        }
    }


    /// <summary>
    /// 원하는 타입의 ui 전부 제거
    /// </summary>
    /// <param name="type"></param>
    public void ClearUI(UIType type)
    {
        // 삭제할 GameObject 모아두기
        var toDestroy = new List<string>();
        foreach (var kv in UIMap)
        {
            var go = kv.Value;
            if (go == null) continue;
            var uiBase = go.GetComponentInChildren<UIBase>(true);
            if (uiBase != null && (uiBase.uiType & type) != 0)
            {
                toDestroy.Add(kv.Key);
            }
        }

        // 한꺼번에 파괴 & 맵에서 제거
        foreach (var name in toDestroy)
        {
            if (UIMap.TryGetValue(name, out var go))
            {
                Destroy(go);
                UIMap.Remove(name);
            }
        }

        CleanupUIMap();
    }

    public void CleanupUIMap()
    {
        List<string> keysToRemove = new List<string>();

        foreach (var ui in UIMap)
        {
            if (ui.Value == null)
            {
                keysToRemove.Add(ui.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            UIMap.Remove(key);
           // Debug.Log($"[CleanupUIMap] 삭제된 UI 제거: {key}");
        }
    }

    // uiscenetype 에 따른 ui 오픈
    public void OpenUI(UISceneType type){
        foreach (var ui in UIMap)
        {
            UIBase uiBase = ui.Value.GetComponentInChildren<UIBase>(true);
            if (uiBase == null)
            {
                // Debug.LogError("범인은 누구 범인은 누구? : " + ui.Value.name);
                continue;
            }
            var sceneType = ui.Value.GetComponentInChildren<UIBase>(true).uISceneType;
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
            if (ui.Value == null) continue;

            var sceneType = ui.Value.GetComponentInChildren<UIBase>(true).uISceneType;

            if ((type & sceneType) != 0)
            {
                ui.Value.SetActive(false);
            }
        }
    }

    // 모든 고정 ui 켜기
    public void OpenAllPermanent(){
        foreach(var ui in UIMap){
            var uiBase = ui.Value.GetComponentInChildren<UIBase>(true);
            if (uiBase == null) continue;

            if ((uiBase.uiType & (UIType.Top | UIType.Bottom | UIType.Follow | UIType.Permanent | UIType.TopMid | UIType.Background)) != 0)
            {
                ui.Value.SetActive(true);
            }
        }
    }

    // 모든 고정 ui 끄기
    public void CloseAllPermanent()
    {
        UIType tempType = UIType.Top | UIType.Bottom | UIType.TopMid | UIType.Follow | UIType.Permanent | UIType.Background;

        foreach(var ui in UIMap)
        {
            UIBase temp = ui.Value.GetComponentInChildren<UIBase>(true);
            if(temp == null)
            {
                // Debug.LogError("범인은 누구 범인은 누구? : " + ui.Value.name);
                continue;
            }
            UIType type = ui.Value.GetComponentInChildren<UIBase>(true).uiType;
            if((type & tempType) != 0) ui.Value.SetActive(false);
        }
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    public void OpenPopup(UIPopup popup)
    {
        if(popup==null) return;
        if(popupStack.Contains(popup)) return;

        popup.gameObject.SetActive(true);
        popupStack.Push(popup);

        if(popupBG != null) popupBG.SetActive(true);

        popup.Open();
    }

    /// <summary>
    /// 현재 팝업 닫기
    /// </summary>
    public void CloseCurrentPopup(UIPopup popup)
    {
       // Debug.Log($"[CloseCurrentPopup] stackCount={popupStack.Count} target={popup.name}");
        if (popupStack.Count == 0) return;

        if(popupStack.Peek() == popup){
            popupStack.Pop();
            popup.Close();
        }
        else
        {
           // Debug.Log("현재 팝업이 아님. 못닫는다.");
        }

        // 팝업 스택이 비워지면 팝업 bg 꺼줌
        if (popupStack.Count == 0 && popupBG != null) popupBG.SetActive(false);

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
    public void DelayRebuildLayout(UIBase uiBase)
    {
        StartCoroutine(DelayRebuildLayoutCoroutine(uiBase));
    }

    private IEnumerator DelayRebuildLayoutCoroutine(UIBase uiBase)
    {
        if (uiBase == null || uiBase.gameObject == null)    // 대상이 없는데 갱신을 하려고하면 오류 생김 체크해줘야됨
        {
            yield break;
        }

        uiBase.gameObject.SetActive(true);
        yield return null; // 한 프레임 대기 후

        RectTransform layoutRoot = null;

        if (uiBase == null || uiBase.gameObject == null) // 대상이 없는데 갱신을 하려고하면 오류 생김 체크해줘야됨
        {
            yield break;
        }

        // VerticalLayoutGroup이나 HorizontalLayoutGroup이 있으면 그것을 기준으로
        var layoutGroup = uiBase.GetComponentInChildren<LayoutGroup>(true);
        var verticalGroups = uiBase.GetComponentsInChildren<VerticalLayoutGroup>(true);
        var horizontalGroups = uiBase.GetComponentsInChildren<HorizontalLayoutGroup>(true);
        if (layoutGroup != null)
        {
            layoutRoot = layoutGroup.GetComponent<RectTransform>();
        }
        else
        {
            // 없으면 자기 자신의 RectTransform 사용
            layoutRoot = uiBase.GetComponent<RectTransform>();
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);

        foreach(var group in verticalGroups)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(group.GetComponent<RectTransform>());
        }
        foreach(var group in horizontalGroups)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(group.GetComponent<RectTransform>());
        }
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
            if (ui.Value == null || ui.Value.Equals(null)) continue;
            if (ui.Value.GetComponentInChildren<T>(true) != null)
                return ui.Value.GetComponentInChildren<T>(true);
        }

        return null;
    }

    /// <summary>
    /// 팝업 스택 클리어
    /// </summary>
    public void ResetPopupState()
    {
        popupStack.Clear();
    }

}
