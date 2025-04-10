using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public class UIGameScene : MonoBehaviour
{
    [SerializeField] private Button settingsButton;

    private async void Awake()
    {
        await LoadPopup<UIPopupSettings>("UIPopupSettings");
    }
    private void Start()
    {
        // 설정창 버튼 설정
        settingsButton.onClick.AddListener(async()=>
        {
            await UIManager.Instance.OpenPopupAsyncToName<UIPopupSettings>("UIPopupSettings");
        });
    }   

    // 팝업 오브젝트 Load
    private async Task LoadPopup<T>(string name) where T : UIPopup
    {
        Debug.Log($"[LoadPopup] {name} 시작");

        if (UIManager.Instance.cachedPopups.ContainsKey(typeof(T)))
        {
            Debug.Log($"[LoadPopup] {name} 이미 캐싱됨");
            return;
        }

        string key = name ?? typeof(T).Name;
        var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(key);
        await handle.Task;

        if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[LoadPopup] {name} 로드 실패");
            return;
        }
        UIManager.Instance.cachedPopups[typeof(T)] = handle.Result.GetComponentInChildren<T>();
        Debug.Log($"[LoadPopup] {name} 캐싱 완료");
    }
}