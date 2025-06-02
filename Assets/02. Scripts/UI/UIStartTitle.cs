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
        ServerManager.Instance.UIStartTitle = this;
        SoloClearTimeUpdate();
        MultiClearTimeUpdate();
    }

    public void TextUpdate()
    {
#if AllMethodDebug
        Debug.Log("TextUpdate");
#endif
        TextName.text = ServerManager.Instance.PlayerName;
    }

    public void SoloClearTimeUpdate()
    {
#if AllMethodDebug
        Debug.Log("SoloClearTimeUpdate");
#endif
        TextSoloClearTime.text = GameValueManager.Instance.GameSoloClearTime;
    }

    public void MultiClearTimeUpdate()
    {
#if AllMethodDebug
        Debug.Log("MultiClearTimeUpdate");
#endif
        TextMultiClearTime.text = GameValueManager.Instance.GameMultiClearTime;
    }
}
