using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<UIBase> allUIList = new();
    private Stack<UIPopup> popupStack = new();
    private Dictionary<System.Type, UIBase> UIs = new();    // 정적 ui들
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
    }

    protected override void Awake() {   // 싱글톤 초기화 후 allUIList에 있는 모든 UI들을 딕셔너리에 추가하고 초기화함
        base.Awake();
        InitAll();
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
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    public void OpenPopup<T>(params object[] args) where T : UIPopup
    {
        var popup = GetUI<T>();
        OpenPopup(popup);
    }

    public void ClosePopup<T>() where T : UIPopup
    {
        var popup = GetUI<T>();
        
        CloseCurrentPopup(popup);
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
        if(popupStack.Count == 0) return;

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


}
