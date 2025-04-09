using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : BasePoolable
{
    [SerializeField] List<Sprite> _sprites;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Collider2D _collider;
    List<Player> hitPlayers = new List<Player>();

    private void OnEnable()
    {
        //사운스 삽입
    }

    public override void Init()
    {
        //호출용 실제 초기화는 오버로드한 init에서 실행
    }

    /// <summary>
    /// 폭발이펙트 초기화(생성위치,폭발크기,폭말모양)
    /// </summary>
    /// <param name="position">생성위치</param>
    /// <param name="size">폭발크기 기본값 = 1</param>
    /// <param name="spriteNum">폭발모양 기본값 = 0</param>
    public void Init(Vector3 position, float size = 1, int spriteNum = 0)
    {
        transform.position = position;
        transform.localScale = Vector3.one * size;
        _spriteRenderer.sprite = _sprites[spriteNum];
    }
    public void StartExplosion()
    {
        Collider2D[] hitplayers = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.NameToLayer("Player"));

        for(int i = 0; i < hitplayers.Length; i++)
        {
            if(hitplayers[i].TryGetComponent<Player>(out Player player))
            {
                //데미지주기 코드 삽입필요
                hitPlayers.Add(player);
            }
        }
        _collider.enabled = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player) && !hitPlayers.Contains(player))
        {
            //데미지주기 코드 삽입필요
            hitPlayers.Add(player);
        } 
    }
    public void StopDamage()
    {
        _collider.enabled = false;
    }

    
}
