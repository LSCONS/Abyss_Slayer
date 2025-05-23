using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueTextPopupUI : UIPopup
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private float typingSpeed = 0.25f;

    private Coroutine typingCoroutine;

    public override void Open(params object[] args)
    {
        base.Open(args); // base 먼저

        if (args != null && args.Length > 0 && args[0] is string message)
        {
            if (textComponent == null)
            {
                return;
            }

            textComponent.text = ""; // 초기화

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            typingCoroutine = StartCoroutine(TypeText(message));
        }
    }

    private IEnumerator TypeText(string message)
    {
        textComponent.text = "";
        foreach (char letter in message)
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
