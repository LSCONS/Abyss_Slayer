using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BasePoolable
{
    int _damage;
    Vector3 _direction;
    float _warningTime;
    float _width;
    LayerMask _layerMask;
    bool _isPiercing;
    RaycastHit2D _hit;
    bool _isFiered;
    float _angle;
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if(!_isFiered)
            Scale();
    }
    void Scale()
    {
        _hit = Physics2D.Raycast(transform.position, _direction, 1000f, _layerMask);
        transform.localScale = new Vector3(Vector2.Distance(transform.position, _hit.point),transform.localScale.y,1);
    }

    public override void Init()
    {
    }
    public void Init(int damage, Vector3 position, Vector3 direction,float width =1f, float warningTime = 0.5f, bool isPiercing = true)
    {
        _isFiered = false;

        _damage = damage;

        transform.position = position;

        transform.localScale = new Vector3(1, width, 1);

        _direction = direction.normalized;
        _angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,_angle);

        _animator.SetFloat("WarningTime",1/warningTime);

        _isPiercing = isPiercing;
        _layerMask = isPiercing? LayerMask.GetMask("GroundPlane","Shield") : LayerMask.GetMask("GroundPlane", "Shield","Player");

        _animator.SetTrigger("Start");
    }
    public void Damage()
    {
        _isFiered=true;
        if (_isPiercing)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll((_hit.point + (Vector2)transform.position) / 2, transform.localScale, _angle, LayerMask.NameToLayer("Player"));
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
