using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;


public static class SceneName
{
    public const string IntroScene = "IntroScene";
    public const string StartScene = "StartScene";
    public const string LobbyScene = "LobbyScene";
    public const string RestScene = "RestScene";
    public const string LoadingScene = "LoadingScene";

    public const string BossScenePrefix = "BossScene_";
}

public static class SceneHelper
{
    public static string GetBossSceneName(int index)
    {
        return $"{SceneName.BossScenePrefix}{index}";
    }
}


/// <summary>
/// Addressables 기반 씬 로드
/// </summary>
public static class SceneLoader
{
    public static async Task LoadSceneAsync(string sceneName)
    {
        var handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[SceneLoader] Failed to load scene {sceneName}");
        }
    }
}
