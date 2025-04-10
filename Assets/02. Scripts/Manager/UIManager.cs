using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public enum UIState{
    None,
    Main,
    Settings,
    
}

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<UIBase> allUIList = new();
    [SerializeField] private Canvas canvas;
    public Transform popupParent;
    private Stack<UIPopup> popupStack = new();
    private Dictionary<System.Type, UIBase> UIs = new();    // 정적 ui들
    public Dictionary<System.Type, UIPopup> cachedPopups = new();
    private void OnValidate() // 에디터에서 컴포넌트 변경 시 자동으로 호출됨 -> allUIList에 UIBase 가진 오브젝트들을 자동으로 추가함
    {
        allUIList.Clear();
        UIBase[] allUIs = FindObjectsOfType<UIBase>();
        foreach (var ui in allUIs)
        {
            // 중복처리
            if (!allUIList.Contains(ui))
            {
                allUIList.Add(ui);
            }
        }
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        popupParent = GameObject.Find("UI_Popup").transform;
    }

    protected override void Awake() {   // 싱글톤 초기화 후 allUIList에 있는 모든 UI들을 딕셔너리에 추가하고 초기화함
        base.Awake();
        InitAll();

        if(canvas == null){
            Debug.LogError("Canvas 찾을 수 없음");
        }
        if(popupParent == null){
            popupParent = new GameObject("UI_Popup").transform;
        }
    }

    public void InitAll(){
        foreach (var ui in allUIList)
        {
            if (!UIs.ContainsKey(ui.GetType()))
            {
                UIs.Add(ui.GetType(), ui);
                ui.Init();
            }
        }
    }

    public T GetUI<T>() where T : UIBase
    {
        if(UIs.TryGetValue(typeof(T), out var ui)){
            return ui as T;
        }
        return null;
    }

    public void OpenUI<T>(params object[] args) where T : UIBase
    {
        GetUI<T>()?.Open(args);
    }

    public void CloseUI<T>() where T : UIBase
    {
        GetUI<T>()?.Close();
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

    /// <summary>
    /// 팝업 열기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    public void OpenPopup<T>(string name) where T : UIPopup // 나중에 name 지우기
    {
        Debug.Log($"[OpenPopupAsyncToName] {name} 시작");

        if (!cachedPopups.TryGetValue(typeof(T), out var popup) || popup == null)
        {
            Debug.LogError($"[OpenPopupAsyncToName] {name} 캐시된 프리팹이 없음");
            return;
        }

        if (isPopupOpen<T>())
        {
            Debug.Log($"[OpenPopupAsyncToName] {name} 이미 열려있음");
            return;
        }
        OpenPopup(popup);
        
    }


}
