using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPhase2Data", menuName = "BossPattern/BossPhase2")]
public class BossPhase2Data : BasePatternData
{
    [Header("2페이즈 연출 설정")]
    [Tooltip("연출 시작 전 대기 시간")]
    [SerializeField] private float delayBeforeStart = 1.0f;

    [Tooltip("카메라 확대 배율")]
    [SerializeField] private float phase2ZoomScale = 3.0f;

    [Tooltip("카메라 줌과 UI 연출 유지 시간")]
    [SerializeField] private float phase2Duration = 3.0f;

    [Tooltip("Phase2 UI에 표시할 대사 텍스트")]
    [SerializeField] private string phase2UIText = "2페이즈 대사";
    public override bool IgnoreAvailabilityCheck => true;

    public override IEnumerator ExecutePattern()
    {
        // 1. 보스 멈춤
        bossController.IsRun = false;
        bossController.ChasingTarget = false;
        bossController.HitCollider.enabled = false;

        // 2. 플레이어도 멈추고 무적
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_DisconnectInput();
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_SetInvincibilityAllPlayer(true);

        // 3. 카메라 줌하기
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_VirtualCamera(phase2ZoomScale, 20);

        // 4. 사운드 넣고
        if (EAudioClip != null && EAudioClip.Count > 0)
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip[0]);

        // 5. UI 넣고
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_OpenPopup("Phase2StartPopup".StringToBytes(), phase2UIText.StringToBytes());

        // 6. 잠깐 대기
        yield return new WaitForSeconds(delayBeforeStart);

        // 7. UI 유지 시간
        yield return new WaitForSeconds(phase2Duration);

        // 8. UI 닫기\
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_ClosePopup("Phase2StartPopup".StringToBytes());

        // 9. 카메라 복원
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_VirtualCamera(5f, 5);

        // 10. 보스 피격 가능 & 플레이어 조작 복구
        bossController.HitCollider.enabled = true;
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_ConnectInput();
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_SetInvincibilityAllPlayer(false);
    }
}
