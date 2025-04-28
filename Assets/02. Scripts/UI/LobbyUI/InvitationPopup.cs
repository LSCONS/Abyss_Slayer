using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvitationPopup : UIPopup
{
    [SerializeField] private Button closeButton;


    public override void Init()
    {
        base.Init();

        closeButton = transform.GetGameObjectSameNameDFS("Close").GetComponent<Button>();
    }

    public override void Open(params object[] args)
    {
        base.Open();
        closeButton.onClick.AddListener(OnClose);
    }
    public override void Close()
    {
        base.Close();
    }
    public override void OnClose()
    {
        Debug.Log("OnClose");
        base.OnClose();
    }
}
