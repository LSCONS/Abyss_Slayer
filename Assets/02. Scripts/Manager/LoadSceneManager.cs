using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
public class LoadSceneManager : Singleton<LoadSceneManager>
{
    private ProgressBar progressBar;
    private string targetSceneName;
    private TaskCompletionSource<bool> tcs;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }


    public async Task LoadScene(string sceneName)
    {
        targetSceneName = sceneName;
        tcs = new TaskCompletionSource<bool>();

        // 콜백 등록을 해
        SceneManager.sceneLoaded += OnLoadingSceneLoaded;

        // 붙여서 로드해
        SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);

        // 로딩 완료 대기
        await tcs.Task;
    }

    private async void OnLoadingSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 로딩씬은 무시해
        if (scene.name != "LoadingScene") return;

        // 콜백은 한 번만 실행
        SceneManager.sceneLoaded -= OnLoadingSceneLoaded;

        // 프로그래스바 연결해
        progressBar = GameObject.Find("ProgressBar")?.GetComponent<ProgressBar>();
        if (progressBar == null)
        {
            return;
        }

        // 이제 진짜 씬 로드해
        var op = SceneManager.LoadSceneAsync(targetSceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            progressBar.SetProgressValue(op.progress);
            await Task.Yield();
        }

        progressBar.SetProgressValue(1f);

        op.allowSceneActivation = true;

        // 로딩씬 언로드해
        SceneManager.UnloadSceneAsync("LoadingScene");

        tcs.SetResult(true);
    }
}
