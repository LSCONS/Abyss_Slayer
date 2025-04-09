using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileNormal : BasePoolable
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] List<Sprite> _sprites;
    [SerializeField] SpriteRenderer _spriteRenderer;
    float speed;
    Vector3 direction;
    float delayTime;

    float startTime;
    bool movable;

    private void OnEnable()
    {
        movable = false;
        startTime = Time.time;
    }
    private void Update()
    {
        if(!movable)
        {
            if(Time.time - startTime >= delayTime)
            {
                movable = true;
                //발사사운드 삽입
            }
        }
        else
        {
            transform.Translate(direction * speed *  Time.deltaTime);
        }
    }
    public override void Init()
    {
        //호출용 실제 초기화는 오버로드한 init에서 실행
    }
    /// <summary>
    /// 일반탄환 초기화(정해진방향,속도로일정하게 진행되는 탄환)
    /// </summary>
    /// <param name="position">탄환생성좌표</param>
    /// <param name="direction">탄환진행방향</param>
    /// <param name="speed">탄환속도, 기본값=1</param>
    /// <param name="delayTime">탄스폰후 발사까지 시간차, 기본값 0초</param>
    /// <param name="size">탄 사이즈, 기본값 1</param>
    /// <param name="spriteNum">탄 모양, 기본값 0</param>
    public void Init(Vector3 position, Vector3 direction, float speed = 1f, float delayTime = 0f, float size = 1f, int spriteNum = 0)
    {
        transform.position = position;
        this.direction = direction.normalized;
        this.speed = speed;
        this.delayTime = delayTime;
        transform.localScale = Vector3.one * size;
        _spriteRenderer.sprite = _sprites[spriteNum];
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player))
        {
            //데미지주는코드
        }

        ReturnToPool();
    }

    
}
