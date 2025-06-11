using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/Tornado")]
public class TornadoData : BasePatternData
{
    [Header("개별 패턴 세부 정보")]
    [SerializeField] int damage;
    [SerializeField] float attackPerSec = 5f;
    [SerializeField] float preDelayTime = 0.5f;
    [Tooltip("토네이도가 생성될 후보 위치 (월드 x좌표값 -20 ~ 20)")]
    [SerializeField] List<float> spawnPositionsX;
    [Tooltip("생성개수")]
    [SerializeField] int tornadoCount;
    [Tooltip("체크시 보스 자신위치에도 토네이도 생성")]
    [SerializeField] bool selfTornado;
    [SerializeField] Vector2 tornadoScale;
    [SerializeField] float warningTime = 1f;
    [SerializeField] float durationTime = 2.5f;
    [SerializeField] float postDelayTime = 1f;
    List<int> _listIndex = new List<int>();
    public override IEnumerator ExecutePattern()
    {
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        //bossAnimator.SetTrigger("Attack3");                    //공격모션 애니메이션 삽입                              
        yield return new WaitForSeconds(preDelayTime);      //공격모션 선딜

        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Attack3ParameterHash);                    //공격 애니메이션 삽입

        //자신위치에 토네이도 한개 생성
        if (EAudioClip != null && EAudioClip.Count > 0)
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip[0]);
        if (selfTornado)
        {
            //PoolManager.Instance.Get<Tornado>().Init(new Vector3(bossTransform.position.x, groundPositionY), damage, durationTime, attackPerSec, warningTime, tornadoWidth);
            ManagerHub.Instance.ServerManager.InitSupporter.Rpc_StartTornadoInit(new Vector3(bossTransform.position.x, tornadoScale.y / 20), damage, durationTime, attackPerSec, warningTime, tornadoScale.x, tornadoScale.y);
        }
        
        

        _listIndex.Clear();
        for (int i = 0; i < spawnPositionsX.Count; i++)
        {
            _listIndex.Add(i);
        }
        for(int i = spawnPositionsX.Count - 1; i > 0 ; i--)
        {
            int j = Random.Range(0, i + 1);
            (_listIndex[i], _listIndex[j]) = (_listIndex[j], _listIndex[i]);
        }
        if(tornadoCount > spawnPositionsX.Count)
        {
            Debug.LogWarning($"소환할 토네이도개수({tornadoCount})가 토네이도위치후보 개수({spawnPositionsX.Count})보다 많음");
            tornadoCount = spawnPositionsX.Count;
        }
        //지정된 위치 중 지정 숫자만큼 토네이도 생성
        for (int i = 0; i < tornadoCount; i++)
        {
            ManagerHub.Instance.ServerManager.InitSupporter.Rpc_StartTornadoInit(new Vector3(spawnPositionsX[_listIndex[i]], tornadoScale.y/20), damage, durationTime, attackPerSec, warningTime, tornadoScale.x, tornadoScale.y);
            //PoolManager.Instance.Get<Tornado>().Init(new Vector3(spawnPositionsX[i], groundPositionY), damage, durationTime, attackPerSec, warningTime, tornadoWidth);
        }

        yield return new WaitForSeconds(warningTime + durationTime);
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.IdleParameterHash);
        yield return new WaitForSeconds(postDelayTime);
    }

}
