using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BasePoolable
{
    int _damage;
    Transform _target;
    float _warningTime;
    float _chasingTime;
    float _width;
    LayerMask _layerMask;
    bool _isPiercing;
    RaycastHit2D _hit;
    bool _isFiered;
    float _angle;
    Animator _animator;
    float _startTime;
    [SerializeField] Transform laserSprite;

    bool _chasingEnd;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if(!_chasingEnd)
        {
            if(Time.time - _startTime >= _chasingTime)
            {
                _chasingEnd = true;
                _animator.SetTrigger("EndChasing");
            }
            Chasing();
        }
        if (!_isFiered)
            Scale();
    }
    void Scale()
    {
        _hit = Physics2D.Raycast(transform.position, transform.right, 1000f, _layerMask);
        transform.localScale = new Vector3(Vector2.Distance(transform.position, _hit.point),transform.localScale.y,1);
    }
    void Chasing()
    {
        Vector3 _direction = _target.position - transform.position;
        _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, _angle);
    }

    public override void Init()
    {
    }
    public void Init(int damage, Vector3 position, Transform target,float width =0.5f, float warningTime = 0.5f, float chasingTime = 0f, bool isPiercing = true)
    {
        _isFiered = false;

        _damage = damage;

        transform.position = position;

        laserSprite.localScale = new Vector3(laserSprite.localScale.x, width, 1);

        _target = target;
        //_direction = _target.position - transform.position;
        //_angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,_angle);

        _animator.SetFloat("WarningTime",1/warningTime);
        _chasingTime = chasingTime;
        _startTime = Time.time;
        _chasingEnd = false;

        _isPiercing = isPiercing;
        _layerMask = isPiercing? LayerMask.GetMask("GroundPlane","Shield") : LayerMask.GetMask("GroundPlane", "Shield","Player");


    }
    public void Damage()
    {
        _isFiered=true;
        if (_isPiercing)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll((_hit.point + (Vector2)transform.position) / 2, new Vector2(transform.localScale.x,laserSprite.localScale.y), _angle, LayerMask.NameToLayer("Player"));
            for (int i = 0; i < colliders.Length; i++)
            {
                if(colliders[i].TryGetComponent<Player>(out Player player))
                {
                    //player데미지주기
                }
            }
        }
        else
        {
            if(_hit.transform.TryGetComponent<Player>(out Player player))
            {
                //플레이어 데미지주기
            }
        }
    }
}
