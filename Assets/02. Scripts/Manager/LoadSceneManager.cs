using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
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
    public Task LoadScene(string sceneName)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(LoadingAsync(sceneName, tcs));
        return tcs.Task;
    }

    private IEnumerator LoadingAsync(string sceneName, TaskCompletionSource<bool> tcs)
    {
        yield return SceneManager.LoadSceneAsync("LoadingScene");
        progressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            progressBar.SetProgress(op.progress);
            yield return null;
        }
        progressBar.SetProgress(1f);
        yield return null;

        op.allowSceneActivation = true;
        tcs.SetResult(true);
    }
}
