using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class GravityProjectile : BasePoolable
{
    Rigidbody2D _rigidbody;
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] NormalDamageCollider _bossProjectileCollider;
    [SerializeField] GameObject _sprite;
    [SerializeField] Collider2D _collider;
    Vector3 _position;
    float _gravity;
    int _damage;
    float _velocityX;
    float _baseSpeed;
    float _maxSpeed;
    float _minSpeed;

    
    Transform _target;
    float _throwTime;
    bool _throwed;

    public override void Spawned()
    {
        base.Spawned();
        _rigidbody = GetComponent<Rigidbody2D>();
        _sprite.SetActive(false);
    }

    public override void FixedUpdateNetwork()
    {
        if (!_throwed && Time.time >= _throwTime)
        {
            _throwed = true;
            Throw();
        }
        else if(!_throwed)
        {
            transform.position = _position;
        }
    }

    public override void Rpc_Init()
    {
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(int damage, Vector3 position, float baseSpeed, float maxSpeed,float minSpeed, PlayerRef target,float delayThorwTime,int piercingCount , float size = 3f, float gravityScale = 1f)
    {
        gameObject.SetActive(true);
        _sprite.SetActive(true);
        _collider.enabled = false;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _throwed = false;
        _damage = damage;
        _position = new Vector3(Mathf.Clamp(position.x, -20 + size * 1.81f, 20 - size * 1.81f), position.y);
        transform.position = _position;
        transform.rotation = Quaternion.identity;
        _baseSpeed = baseSpeed;
        _maxSpeed = maxSpeed;
        _minSpeed = minSpeed;
        _target = ServerManager.Instance.DictRefToPlayer[target].transform;
        _throwTime = Time.time + delayThorwTime + 1.2f;
        transform.localScale = Vector3.one * size;
        _gravity = 9.81f * gravityScale;
        _rigidbody.gravityScale = gravityScale;
        _bossProjectileCollider.Init(_damage,Rpc_Destroy,piercingCount);
    }

    public void Init(Vector3 direction, int damage, float speed, int piercingCount, float size = 1f, float gravityScale = 1f)
    {
    }

    void Throw()
    {
        _velocityX = Mathf.Min(Mathf.Abs(_target.position.x - transform.position.x) *_baseSpeed,_maxSpeed);
        _collider.enabled = true;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        Vector3 _targetPosition = _target.position;
        float time = Mathf.Abs(_targetPosition.x - transform.position.x)/_velocityX;    //목표물까지 걸리는 시간
        float deltaY = _targetPosition.y - transform.position.y;                        //목표물과 발사지점 y축 차이
        float velocityY = deltaY / time + (_gravity * time / 2);                        //시작지점 y축 속도

        _velocityX = _velocityX * Mathf.Sign(_targetPosition.x - transform.position.x); //시작지점 x축 속도

        Vector2 velocity = new Vector2(_velocityX, velocityY);                          //목표물의 x축 거리가 짧을때 y속도가 과도하게 높아지는 현상 방지
        velocity = velocity.normalized * Mathf.Clamp(velocity.magnitude,_minSpeed,_maxSpeed + 1);
        _rigidbody.velocity = velocity;

        _rigidbody.angularVelocity = Random.Range(-90f, 90f);                           //랜덤 회전값
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_Destroy()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.velocity = Vector2.zero;
        _sprite.SetActive(false);
        _particleSystem.Play();
        _collider.enabled = false;
        Invoke("Rpc_ReturnToPool", 3f);
    }
}
