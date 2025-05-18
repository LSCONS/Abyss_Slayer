using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DummyRecovery : MonoBehaviour
{
    private Boss boss;
    private Coroutine slowRecoveryCoroutine;
    [field: Header("자연회복 시간(최대 체력보다 작아진 후)")]
    [field: SerializeField] float recoverySeconds { get; set; } = 5;
    [Header("자연회복 시간(마지막 공격 받은 후)")]
    [SerializeField] private float takeDamageSeconds = 3f;
    [Header("자연회복량")]
    [SerializeField] int recoveryAmounts = 10;
    [Header("자연회복(몇 초마다 회복)")]
    [SerializeField] float recoveryTerm = 0.1f;

    private float lastDamagedTime;
    private float previousHp = 0;

    private void Awake()
    {
        boss = GetComponent<Boss>();
        lastDamagedTime = Time.time;
    }

    private void Start()
    {
        // 체력 1 이하일 때 즉시 회복
        boss.Hp
            .Where(hp => hp <= 1)
            .Subscribe(_ => RecoverHpImmediately())
            .AddTo(this);

        // 체력 변화 감지하고 피격 감지 + 회복 시도
        boss.Hp
            .DistinctUntilChanged()
            .Subscribe(OnHpChanged)
            .AddTo(this);
    }
    private void OnHpChanged(int currentHp)
    {
        // 체력이 줄어들었으면 피격 타이밍 업데이트
        if (previousHp > currentHp)
        {
            lastDamagedTime = Time.time;

            // 회복 중이었다면 중단
            if (slowRecoveryCoroutine != null)
            {
                StopCoroutine(slowRecoveryCoroutine);
                slowRecoveryCoroutine = null;
            }
        }

        // 회복 조건 가능 여부 체크
        if (currentHp < boss.MaxHp.Value && slowRecoveryCoroutine == null)
        {
            slowRecoveryCoroutine = StartCoroutine(SlowRecoveryCoroutine());
        }

        previousHp = currentHp;
    }

    private void RecoverHpImmediately()
    {
        boss.Hp.Value = boss.MaxHp.Value;
    }

    private void TryStartRecovery()
    {
        if (slowRecoveryCoroutine == null)
        {
            slowRecoveryCoroutine = StartCoroutine(SlowRecoveryCoroutine());
        }
    }

    private IEnumerator SlowRecoveryCoroutine()
    {
        // 데미지 이후 일정 시간 대기
        while (Time.time - lastDamagedTime < takeDamageSeconds)
        {
            yield return null;
        }

        // 회복 시작
        while (boss.Hp.Value < boss.MaxHp.Value)
        {
            boss.Hp.Value = Mathf.Min(boss.Hp.Value + recoveryAmounts, boss.MaxHp.Value);
            yield return new WaitForSeconds(recoveryTerm);

            // 회복 중 도중에 또 데미지를 받았는지 확인해야됨
            if (Time.time - lastDamagedTime < takeDamageSeconds)
            {
                slowRecoveryCoroutine = null;
                yield break;
            }
        }

        slowRecoveryCoroutine = null;
    }


}
