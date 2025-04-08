using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileNormal : MonoBehaviour
{
    private BossProjectileNormalPool _pool;
    [SerializeField] List<Sprite> _sprites;
    [SerializeField] SpriteRenderer _spriteRenderer;
    float speed;
    Vector3 direction;
    private void OnEnable()
    {
        //사운스 삽입
    }
    public void SetPool(BossProjectileNormalPool pool)
    {
        _pool = pool;
    }

    public void Init(Vector3 position, Vector3 direction, float speed = 1f, float delayTime = 0f, float size = 1f, int spriteNum = 0)
    {
        transform.position = position;
        transform.localScale = Vector3.one * size;
        _spriteRenderer.sprite = _sprites[spriteNum];
    }
    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        _pool.ReturnToPool(this);
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
