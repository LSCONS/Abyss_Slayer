using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : Singleton<LoadSceneManager>
{

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log("[LoadSceneManager] LoadScene");
        //     LoadScene("UITestScene");
        // }
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadingAsync(sceneName));
    }

    IEnumerator LoadingAsync(string sceneName)
    {
        // 로딩 씬 로드
        yield return SceneManager.LoadSceneAsync("LoadingScene");

        // 진짜 찐 씬 로드
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            Debug.Log($"[LoadingScene] Progress: {asyncOperation.progress}");
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        asyncOperation.allowSceneActivation = true;
    }
}
