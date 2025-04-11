using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
public class UIGameScene : MonoBehaviour
{
    [SerializeField] private Button settingsButton;

    private async Task Awake() 
    {
        await UIManager.Instance.LoadAllUI(UIType.Popup);
        await UIManager.Instance.LoadAllUI(UIType.Bottom);
        await UIManager.Instance.LoadAllUI(UIType.Top);



        UIManager.Instance.CreateAllUI(UIType.GamePlay);
        UIManager.Instance.Init();
        // UIManager.Instance.OpenAllPermanent();
    }

}