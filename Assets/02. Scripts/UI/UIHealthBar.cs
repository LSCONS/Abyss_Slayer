using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : UIBase
{
    [SerializeField] private Image hpBar;
    [SerializeField] private string targetName;
    [SerializeField] private Transform target;

    private IHasHealth healthTarget;
    private void OnValidate()
    {
        hpBar = transform.GetGameObjectSameNameDFS("HpBar").GetComponent<Image>();
        // 전체에서 오브젝트 찾기
        target = GameObject.Find(targetName).transform;  // 타겟 이름 받아서 OnValidate 에서도 가능하게
    }
    public override void Init()
    {
        this.gameObject.SetActive(true);
        hpBar.fillAmount = 1;
        if (target == null)
        {
            Debug.LogError($"UIHealthBar {targetName} 찾을 수 없음");
        }
        if (!target.TryGetComponent<IHasHealth>(out healthTarget))
        {
            Debug.LogError($"[UIHealthBar] {targetName} 찾을 수 없음");
            return;
        }

        healthTarget.OnHpChanged += SetHp;
    }

    private void OnDestroy()
    {
        if (healthTarget != null)
        {
            healthTarget.OnHpChanged -= SetHp;
        }
    }
    public void SetHp(int hp, int maxHp)
    {
        hpBar.fillAmount = Mathf.Clamp01((float)hp / maxHp);
    }

}
