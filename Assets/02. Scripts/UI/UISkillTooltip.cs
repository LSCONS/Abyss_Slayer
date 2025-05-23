using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UISkillTooltip : UIPopup
{
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescText;

    private Skill skillData;
    RectTransform tooltipRect;
    private Canvas canvas;


    private void Start()
    {
        tooltipRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public override void Init()
    {
# if AllMethodDebug
        Debug.Log("Init");
#endif
        base.Init();

        // 툴팁 텍스트 초기화
        skillNameText.text = "";
        skillDescText.text = "";
    }

    public void SetSkill(Skill skillData)
    {
# if AllMethodDebug
        Debug.Log("SetSkill");
#endif
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

        skillNameText.text = skillData.SkillName;
        skillDescText.text = skillData.SkillDesription;
    }

    public void SetBuff(string buffName, string buffDesc)
    {
# if AllMethodDebug
        Debug.Log("SetBuff");
#endif
        if (skillNameText == null || skillDescText == null)
        {
            Debug.LogError("[UISkillTooltip] TextMeshProUGUI가 연결되지 않았습니다.");
            return;
        }

        skillNameText.text = buffName;
        skillDescText.text = buffDesc;
    }

    public void SetTooltipPosition(RectTransform slotRect)
    {
# if AllMethodDebug
        Debug.Log("SetTooltipPosition");
#endif
        // 필요한 거 없으면 무시
        if (slotRect == null)
            return;

        // 슬롯의 월드 좌표 네 모서리 정보 저장
        Vector3[] corners = new Vector3[4];
        slotRect.GetWorldCorners(corners);

        // 코너 세팅 (ui 좌표 기준임)
        Vector3 slotBottomLeft = corners[0];                                        // 왼쪽 아래
        Vector3 slotTopRight = corners[2];                                          // 오른쪽 위

        // 툴팁 높이 계산
        float tooltipHeight = tooltipRect.rect.height * canvas.scaleFactor;         // 캔버스 스케일 생각
        float screenHeight = Screen.height;                                         // 현재 스크린 높이 저장

        float heightOffset = tooltipHeight / 2;
        // 오프셋 적용
        Vector3 desiredPos = slotTopRight + new Vector3(0f, heightOffset);

        // 만약 툴팁이 화면 상단을 넘을 경우 -> 아래로 배치
        if (desiredPos.y + tooltipHeight > screenHeight)
        {
            desiredPos = slotBottomLeft - new Vector3(0f, heightOffset/2);     // 아래쪽 배치
        }

        // 최종 계산 된 
        this.transform.position = desiredPos;
    }


    public void ShowTooltip(string name, string desc, RectTransform target)
    {
# if AllMethodDebug
        Debug.Log("ShowTooltip");
#endif
        this.gameObject.SetActive(false);
        SetBuff(name, desc);                 // 이름 설명 세팅
        Open();                              // 툴팁 열기
        SetTooltipPosition(target);
    }

    public void ShowTooltip(Skill skillData, RectTransform target)
    {
# if AllMethodDebug
        Debug.Log("ShowTooltip");
#endif
        this.gameObject.SetActive(false);
        SetSkill(skillData);                 // 이름 설명 세팅
        Open();                              // 툴팁 열기
        SetTooltipPosition(target);
    }

}
