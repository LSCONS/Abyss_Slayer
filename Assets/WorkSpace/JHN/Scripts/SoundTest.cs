//using UnityEngine;

//public class SoundTest : MonoBehaviour
//{
//    // 테스트할 사운드 이름 (SoundData.soundName)
//    [SerializeField] private string testSoundName = "SFX1";
//    [SerializeField] private string testSoundName2 = "SFX2";


//    [SerializeField] private bool playOnStart = true; // 자동 재생 여부

//    [SerializeField] private string sceneSfxLabel = "Scene1"; // 씬 전용 SFX 라벨


//    [SerializeField] private string bgmName1 = "BGM1";
//    [SerializeField] private string bgmName2 = "BGM2";

//    private async void Start()
//    {
//        await SoundManager.Instance.Init(EGameState.Start);
//        SoundManager.Instance.PlayBGM("BGM1");
//    }


//    private void PlayTestSound()
//    {
//        if (SoundManager.Instance != null)
//        {
//            Debug.Log($"[SoundTest] Try playing sound: {testSoundName}");
//            SoundManager.Instance.PlaySound(testSoundName);
//        }
//        else
//        {
//            Debug.LogWarning("[SoundTest] SoundManager is not initialized.");
//        }
//    }

//    private void PlayTestSound2()
//    {
//        if (SoundManager.Instance != null)
//        {
//            Debug.Log($"[SoundTest] Try playing sound: {testSoundName2}");
//            SoundManager.Instance.PlaySound(testSoundName2);
//        }
//        else
//        {
//            Debug.LogWarning("[SoundTest] SoundManager is not initialized.");
//        }
//    }

//    private void PlayBGM1()
//    {
//        SoundManager.Instance.PlayBGM(bgmName1);
//    }

//    private void PlayBGM2()
//    {
//        SoundManager.Instance.PlayBGM(bgmName2);
//    }

//}
