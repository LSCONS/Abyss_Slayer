using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPopup : UIBase
{
    public override void Init()
    {
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    public override void Open(params object[] args)
    {
        base.Open(args);
        transform.SetAsLastSibling();   // 제일 위로 올려줌
        // 나중에 dotween 사용해서 애니메이션 추가
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    public override void Close()
    {
        base.Close();
        // 나중에 dotween 사용해서 애니메이션 추가
    }

    /// <summary>
    /// 열기 / 버튼 이거 연결해주면 됨
    /// </summary>
    public virtual void OnOpen()
    {
        Open();
        UIManager.Instance.OpenPopup(this);
    }

    /// <summary>
    /// 닫기 / 버튼 이거 연결해주면 됨
    /// </summary>
    public virtual void OnClose()
    {
        UIManager.Instance.CloseCurrentPopup(this);
    }


}
