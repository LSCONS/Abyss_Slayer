using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerState : UIPermanent
{
    [field: SerializeField] public UIHealthBar UIHealthBar { get; set; }
    [field: SerializeField] public Image classIcon { get; set; }

    private void Awake()
    {
#if AllMethodDebug
        Debug.Log("Awake");
#endif
        ManagerHub.Instance.UIConnectManager.UIPlayerState = this;
    }

    public override void Init()
    {
#if AllMethodDebug
        Debug.Log("Init");
#endif
        base.Init();
        SetIcon(ManagerHub.Instance.ServerManager.ThisPlayerData.Class);
    }

    public void SetIcon(CharacterClass cls)
    {
#if AllMethodDebug
        Debug.Log("SetIcon");
#endif
        if (ManagerHub.Instance.DataManager.DictClassToCharacterData.TryGetValue(cls, out CharacterClassData classImage))
        {
            classIcon.sprite = classImage.IconClass;
        }
        else return;
    }

}
