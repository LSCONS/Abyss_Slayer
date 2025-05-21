using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class IntroController : MonoBehaviour
{
    [SerializeField] private GameObject IntroBG;
    [SerializeField] private IntroData introData;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private AudioSource bgm;

    private Coroutine currentTypingCoroutine;
    private bool isTyping = false;

    private void Start()
    {
        IntroBG.SetActive(true);
        bgm?.Play();
        StartCoroutine(DelayedStart()); // 한 프레임 대기 후 시작
    }

    private IEnumerator DelayedStart()
    {
        yield return null;
        yield return StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        foreach (var cut in introData.cuts)
        {
            backgroundImage.sprite = cut.backgroundImage;

            if (cut.clip != null)
            {
                bgm.Stop();
                bgm.clip = cut.clip;
                bgm.Play();
            }

            isTyping = true;
            currentTypingCoroutine = StartCoroutine(TypeText(cut.line));

            float elapsedTime = 0f;
            bool textFullyShown = false;
            bool spacePressedDuringTyping = false;

            // 1단계: 텍스트 타이핑 중
            while (isTyping && elapsedTime < 5f)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // 타이핑 중에 스페이스 누르면 텍스트 전부 출력
                    StopCoroutine(currentTypingCoroutine);
                    introText.text = cut.line;
                    isTyping = false;
                    textFullyShown = true;
                    spacePressedDuringTyping = true;

                    // Space 떼기까지 기다리기
                    yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space));
                    break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 2단계: 텍스트가 전부 출력된 후 → Space 기다리기 또는 자동 대기
            float waitTime = Mathf.Max(0f, 5f - elapsedTime);

            float waited = 0f;
            while (waited < waitTime)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    break;

                waited += Time.deltaTime;
                yield return null;
            }

            // 만약 타이핑 중 Space 눌러서 텍스트만 출력했을 경우 → 다시 입력을 요구
            if (spacePressedDuringTyping)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            }

            // Space 떼기까지 기다리기 (중복 입력 방지)
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space));
        }

        this.gameObject.SetActive(false);
    }

    private IEnumerator TypeText(string line)
    {
        introText.text = "";
        foreach (char c in line)
        {
            introText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
