using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : Singleton<LoadSceneManager>
{
    private ProgressBar progressBar;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         Debug.Log("[LoadSceneManager] LoadScene");
    //         LoadScene("UITestScene");
    //     }
    // }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadingAsync(sceneName));
    }

    IEnumerator LoadingAsync(string sceneName)
    {

        // 페이드인 추가

        // 로딩 씬 로드
        yield return SceneManager.LoadSceneAsync("LoadingScene");
        progressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();

        // 진짜 찐 씬 로드
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            progressBar.SetProgress(asyncOperation.progress);
            yield return null;
        }
        progressBar.SetProgress(1f);
        yield return null;
        asyncOperation.allowSceneActivation = true;
    }
}
