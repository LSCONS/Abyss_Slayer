using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : BasePoolable
{
    // Init으로 전달받은 화살 데이터 저장용 변수
    private int damage;           
    private float arrowSpeed, maxRange;    
    private Vector3 direction, initPos;

    private void Update()
    {
        // 최대 거리 도달 시 풀에 반환
        if (Vector3.Distance(initPos, transform.position) >= maxRange)
        {
            ReturnToPool();
        }

        // 화살 이동
        transform.Translate(direction * arrowSpeed * Time.deltaTime);
    }

    public override void Init()
    {
        // 호출용
    }

    /// <summary>
    /// 각 스킬 클래스에서 전달받은 데이터로 화살을 초기화하는 메서드
    /// </summary>
    /// <param name="spawnPos">화살 생성 위치</param>
    /// <param name="dir">화살 이동 방향</param>
    /// <param name="range">화살 최대 이동 거리</param>
    /// <param name="speed">화살 이동 속도</param>
    public void Init(Vector3 spawnPos, Vector3 dir, float range, float speed)
    {
        transform.position = spawnPos;
        initPos = spawnPos;
        direction = dir.normalized;
        maxRange = range;
        arrowSpeed = speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Boss>(out Boss boss))
        {
            //boss.TakeDamage(damage); // 데미지 전달
        }

        ReturnToPool(); // 투사체 제거
    }
}
