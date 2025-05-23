using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : BasePoolable
{
    [SerializeField] List<Sprite> _sprites;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Collider2D _collider;
    [SerializeField] NormalDamageCollider _baseDamageCollider;
    List<Player> hitPlayers = new List<Player>();
    int _damage;
    [SerializeField] EAudioClip sound;

    private void OnEnable()
    {
        //폭발 사운드 삽입
    }

    public override void Rpc_Init()
    {
        //호출용 실제 초기화는 오버로드한 init에서 실행
    }

    /// <summary>
    /// 폭발이펙트 초기화(생성위치,폭발크기,폭말모양)
    /// </summary>
    /// <param name="position">생성위치</param>
    /// <param name="size">폭발크기 기본값 = 1</param>
    /// <param name="spriteNum">폭발모양 기본값 = 0</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(Vector3 position,int damage, float size = 1, int spriteNum = 0)
    {
        transform.position = position;
        _damage = damage;
        transform.localScale = Vector3.one * size;
        _spriteRenderer.sprite = _sprites[spriteNum];

        _baseDamageCollider.Init(_damage, null, int.MaxValue);
    }
    public void StartExplosion()
    {
        _collider.enabled = true;
    }

    public void StopDamage()
    {
        _collider.enabled = false;
    }

    public void Sound()
    {
        SoundManager.Instance.PlaySFX(sound);
    }
}
