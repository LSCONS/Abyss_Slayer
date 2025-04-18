using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityProjectile : BasePoolable
{
    Rigidbody2D _rigidbody;
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] BaseBossDamageCollider _bossProjectileCollider;
    [SerializeField] GameObject _sprite;

    float _gravity;
    int _damage;
    float _velocityX;
    Vector3 _targetPosition;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _sprite.SetActive(false);
    }
    public override void Init()
    {
    }
    public void Init(int damage, Vector3 position, float speedX, Vector3 targetPosition,int piercingCount , float size = 3f, float gravityScale = 1f)
    {
        _sprite.SetActive(true);
        _damage = damage;
        transform.position = position;
        _velocityX = speedX;
        _targetPosition = targetPosition;
        transform.localScale = Vector3.one * size;
        _gravity = 9.81f * gravityScale;
        _rigidbody.gravityScale = gravityScale;
        _bossProjectileCollider.Init(_damage,Destroy,piercingCount);

        Throw();

    }

    public void Init(Vector3 direction, int damage, float speed, int piercingCount, float size = 1f, float gravityScale = 1f)
    {

    }

    void Throw()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        float time = Mathf.Abs(_targetPosition.x - transform.position.x)/_velocityX;    //목표물까지 걸리는 시간
        float deltaY = _targetPosition.y - transform.position.y;                        //목표물과 발사지점 y축 차이
        float velocityY = deltaY / time + (_gravity * time / 2);                        //시작지점 y축 속도

        _velocityX = _velocityX * Mathf.Sign(_targetPosition.x - transform.position.x); //시작지점 x축 속도

        Vector2 velocity = new Vector2(_velocityX, velocityY);                          //목표물의 x축 거리가 짧을때 y속도가 과도하게 높아지는 현상 방지
        if (velocity.magnitude >= 20)
        {
            velocity = velocity.normalized * 20f;
        }


        _rigidbody.velocity = velocity;
        _rigidbody.angularVelocity = Random.Range(-90f, 90f);                           //랜덤 회전값
    }

    void Destroy()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.velocity = Vector2.zero;
        _sprite.SetActive(false);
        _particleSystem.Play();
        Invoke("ReturnToPool", 3f);
    }
}
