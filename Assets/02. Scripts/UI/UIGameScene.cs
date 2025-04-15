using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
public class UIGameScene : MonoBehaviour
{

    private async Task Awake() 
    {
        await UIManager.Instance.LoadAllUI(UIType.Popup);
        await UIManager.Instance.LoadAllUI(UIType.Bottom);
        await UIManager.Instance.LoadAllUI(UIType.Top);
        await UIManager.Instance.LoadAllUI(UIType.Permanent);




        UIManager.Instance.CreateAllUI(UIType.GamePlay);
        UIManager.Instance.Init();

        // 나중에 게임씬 들어갔는지 확인 후 게임씬 들어가면 bind 하는 방식으로 변경
        //UIBinder.Bind<HPTest, UIHealthBar, HealthPresenter>(playerName, playerHpName); // 플레이어 체력 바 바인딩

        // UIManager.Instance.OpenAllPermanent();
    }


}