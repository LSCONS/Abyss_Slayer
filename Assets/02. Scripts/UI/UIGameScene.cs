using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
public class UIGameScene : MonoBehaviour
{
    [SerializeField] private string playerName = "Player1";
    [SerializeField] private string playerHpName = "PlayerHp";
    private async Task Awake() 
    {
        await UIManager.Instance.LoadAllUI(UIType.Popup);
        await UIManager.Instance.LoadAllUI(UIType.Bottom);
        await UIManager.Instance.LoadAllUI(UIType.Top);



        UIManager.Instance.CreateAllUI(UIType.GamePlay);
        UIManager.Instance.Init();

        // 나중에 게임씬 들어갔는지 확인 후 게임씬 들어가면 bind 하는 방식으로 변경
        UIBinder.Instance.Bind<HPTest, UIHealthBar, HealthPresenter>(playerName, playerHpName); // 플레이어 체력 바 바인딩

        // UIManager.Instance.OpenAllPermanent();
    }


}