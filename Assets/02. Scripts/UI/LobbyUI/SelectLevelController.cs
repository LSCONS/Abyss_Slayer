using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Level
{
    Easy,
    Normal,
    Hard
}


public class SelectLevelController : UIPermanent
{
    [SerializeField] Button leftBtn;    // 왼쪽 버튼 (난이도 내림)
    [SerializeField] Button rightBtn;   // 오른쪽 버튼 (난이도 올림) 이거 근데 서클처럼 돌아갈 수 있게?

    [SerializeField] private TextMeshProUGUI levelText; // 난이도 텍스트
    [SerializeField] private TextMeshProUGUI descText;  // 난이도 설명 텍스트

    private Level currentLevel = Level.Easy;

    private void Start()
    {
        UpdateUI();

        leftBtn.onClick.AddListener(() => changeLevel(-1));
        rightBtn.onClick.AddListener(() => changeLevel(1));
    }

    public override void Init()
    {
        base.Init();
        Debug.Log("난이도 init됐나요?");
        gameObject.gameObject.SetActive(true);
    }

    // 순환함. 2되면 hard 0 되면 easy로 순환됨
    private void changeLevel(int direction)
    {
        int total = System.Enum.GetValues(typeof(Level)).Length;
        int newIndex = ((int)currentLevel + direction + total) % total;
        currentLevel = (Level)newIndex;

        UpdateUI(); // 값이 바뀌니까 다시 업데이트해야됨

    }

    private void UpdateUI()
    {
        levelText.text = currentLevel.ToString();

        switch (currentLevel)
        {
            case Level.Easy:
                levelText.text = "난이도: 쉬움";
                descText.text = "쉬운 난이도입니다.\n클리어에 실패해도 재도전 가능합니다.";
                break;
            case Level.Normal:
                levelText.text = "난이도: 보통";
                descText.text = "기본 난이도입니다.\n이정도는 기본이죠?";
                break;
            case Level.Hard:
                levelText.text = "난이도: 어려움";
                descText.text = "님들이 깰 수 있을까요?";
                break;
        }
    }
}
