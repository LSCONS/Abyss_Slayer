using Fusion;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class TestSctript : MonoBehaviour
{
    private async void Awake()
    {
        var current = SceneManager.GetActiveScene();
        // 2) 경로로 SceneRef 생성
        SceneRef sceneRef = SceneRef.FromIndex(current.buildIndex);

        var runner = RunnerManager.Instance.GetRunner();
        Debug.Log("방 만들기 시작");
        var temp = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = "RoomName",
            Scene = sceneRef
        });

        await DataManager.Instance.Init();
        runner.Spawn(
            DataManager.Instance.PlayerNetworkDataPrefab,           // NetworkPrefabRef
            Vector3.zero,         // 초기 위치
            Quaternion.identity,  // 초기 회전
            runner.LocalPlayer,               // Input Authority 지정
            (r, spawnedObj) =>
            {
                var data = spawnedObj.GetComponent<NetworkData>();
                data.PlayerDataRef = runner.LocalPlayer;
                data.IsServer = (runner.LocalPlayer == runner.LocalPlayer);
                data.IsReady = (runner.LocalPlayer == runner.LocalPlayer);
                data.IntPlayerClass = (int)CharacterClass.Rogue;
                data.FaceKey = 1;
                data.SkinKey = 1;
                data.HairStyleKey = 1;
                data.HairColorKey = 5;
            }
        );

        runner.Spawn(DataManager.Instance.InitSupporterPrefab);

        var spawnResult = runner.Spawn
        (
            DataManager.Instance.PlayerPrefab,
            new Vector3(0, 2, 0),
            Quaternion.identity,
            runner.LocalPlayer,
            (runner, obj) =>
            {
                Player player = obj.GetComponent<Player>();
                player.PlayerRef = runner.LocalPlayer;
                player.PlayerPosition = new Vector3(0, 2, 0);
            }
        );
        var a = RunnerManager.Instance.GetRunner().Spawn(DataManager.Instance.DictEnumToBossObjcet[EBossStage.Boss0]);
        Boss b = a.GetComponent<Boss>();
        b.BossController.StartBossPattern();
        await ServerManager.Instance.ThisPlayer.PlayerPositionReset(new Vector2(0, 2));
        ServerManager.Instance.ThisPlayerData.Rpc_PlayerActiveTrue();
        ServerManager.Instance.ThisPlayerData.Rpc_ConnectInput();
    }
}
