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
        ServerManager.Instance.UIPlayerState = this;
        
    }

    public override void Init()
    {
        base.Init();
        SetIcon(ServerManager.Instance.ThisPlayerData.Class);
    }

    public void SetIcon(CharacterClass cls)
    {
        if (DataManager.Instance.DictClassToImage.TryGetValue(cls, out var classImage))
        {
            classIcon.sprite = classImage;
        }
        else return;
    }

}
