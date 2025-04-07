using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Input : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button btn_CreateID;

    private void Awake()
    {
        btn_CreateID.onClick.AddListener(OnClickCreateID);
    }

    private void OnClickCreateID()
    {

    }
}
