using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UISkillTooltip : UIPopup
{
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescText;

    private Skill skillData;
    public override void Init()
    {
        base.Init();

        // 툴팁 텍스트 초기화
        skillNameText.text = "";
        skillDescText.text = "";
    }

    public void SetSkill(Skill skillData)
    {
        if (skillData == null)
        {
            Debug.LogError("[UISkillTooltip] skillData is NULL!!");
            return;
        }

        this.skillData = skillData;

        if (skillNameText == null || skillDescText == null)
        {
            Debug.LogError("[UISkillTooltip] TextMeshProUGUI가 연결되지 않았습니다.");
            return;
        }

        skillNameText.text = skillData.Name;
        skillDescText.text = skillData.Info;
    }

    
}
