using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
public class UIGameScene : MonoBehaviour
{

    private async Task Init() 
    {
        await ManagerHub.Instance.UIManager.LoadAllUI(UIType.Popup);
        await ManagerHub.Instance.UIManager.LoadAllUI(UIType.Bottom);
        await ManagerHub.Instance.UIManager.LoadAllUI(UIType.Top);
        await ManagerHub.Instance.UIManager.LoadAllUI(UIType.Permanent);
        await ManagerHub.Instance.UIManager.LoadAllUI(UIType.TopMid);


        ManagerHub.Instance.UIManager.CreateAllUI(UIType.GamePlay);
        await ManagerHub.Instance.UIManager.UIInit();

        // 나중에 게임씬 들어갔는지 확인 후 게임씬 들어가면 bind 하는 방식으로 변경
        //UIBinder.Bind<HPTest, UIHealthBar, HealthPresenter>(playerName, playerHpName); // 플레이어 체력 바 바인딩

        // ManagerHub.Instance.UIManager.OpenAllPermanent();
    }


}
