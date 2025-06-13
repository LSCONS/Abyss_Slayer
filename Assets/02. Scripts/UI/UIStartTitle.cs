using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStartTitle : UIPermanent
{
    [field: SerializeField] public TextMeshProUGUI TextName { get; private set;}
    [field: SerializeField] public TextMeshProUGUI TextSoloClearTime { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextMultiClearTime { get; private set; }
    private void Awake()
    {
#if AllMethodDebug
        Debug.Log("Awake");
#endif
        ManagerHub.Instance.UIConnectManager.UIStartTitle = this;
        SoloClearTimeUpdate();
        MultiClearTimeUpdate();
    }

    public void TextUpdate()
    {
#if AllMethodDebug
        Debug.Log("TextUpdate");
#endif
        TextName.text = ManagerHub.Instance.ServerManager.PlayerName;
    }

    public void SoloClearTimeUpdate()
    {
#if AllMethodDebug
        Debug.Log("SoloClearTimeUpdate");
#endif
        TextSoloClearTime.text = ManagerHub.Instance.GameValueManager.GameSoloClearTime;
    }

    public void MultiClearTimeUpdate()
    {
#if AllMethodDebug
        Debug.Log("MultiClearTimeUpdate");
#endif
        TextMultiClearTime.text = ManagerHub.Instance.GameValueManager.GameMultiClearTime;
    }
}
