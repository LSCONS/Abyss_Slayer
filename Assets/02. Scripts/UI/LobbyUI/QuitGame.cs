using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    [Header("룸 나가기 할 건가요?")]
    [SerializeField] private bool isQuitRoom = false;

    public void Quit()
    {
        if (!isQuitRoom)
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            ManagerHub.Instance.ServerManager.ExitRoom();
        }
    }
}
