using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class RogueProjectile : BasePoolable
{
    // Init으로 전달받은 화살 데이터 저장용 변수
    private float damage, speed, maxRange;
    private Vector3 direction, initPos;
    [SerializeField] Sprite sprite;
    [SerializeField] SpriteRenderer spriteRenderer;
    private Player Player { get; set; }

    private LayerMask includeLayer; // 화살 충돌 레이어

    private void Awake()
    {
        includeLayer = LayerData.EnemyLayerMask | LayerData.GroundPlaneLayerMask; 
    }

    private void Update()
    {
        // 최대 거리 도달 시 풀에 반환
        if (Vector3.Distance(initPos, transform.position) >= maxRange)
        {
            ReturnToPool();
        }

        // 화살 이동
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public override void Init()
    {
        // BasePoolable의 추상 메서드 구현
    }

    /// <summary>
    /// 각 스킬 클래스에서 전달받은 데이터로 화살을 초기화하는 메서드
    /// </summary>
    /// <param name="spawnPos">화살 생성 위치</param>
    /// <param name="dir">화살 이동 방향</param>
    /// <param name="range">표창 최대 이동 거리</param>
    /// <param name="speed">표창 이동 속도</param>
    /// <param name="damage">표창 데미지</param>
    public void Init(Player player, Vector3 spawnPos, Vector3 dir, float range, float speed, float damage)
    {
        transform.position = spawnPos; // 실제 화살 위치
        initPos = spawnPos; // 최대 거리 체크용 초기 위치
        direction = dir.normalized; // 방향 정규화
        maxRange = range; // 최대 거리
        this.speed = speed; // 이동 속도
        this.damage = damage; // 데미지
        spriteRenderer.sprite = sprite;
        spriteRenderer.flipX = direction.x < 0 ? true : false;
        Player = player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerData.EnemyLayerIndex && collision.TryGetComponent<IHasHealth>(out IHasHealth enemy))
        {
            enemy.Damage((int)(damage * Player.DamageValue.Value), transform.position.x); // 데미지 전달
        }

        if((1 << collision.gameObject.layer | includeLayer) == includeLayer)
        {
            ReturnToPool(); // 투사체 반환
        }
    }
}
