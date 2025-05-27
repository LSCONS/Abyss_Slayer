using Fusion;
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
    public List<Player> _players = new List<Player>();

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
                _animator.SetTrigger(AnimationHash.EndChasingParameterHash);
            }
            Chasing();
        }
        if (!_isFiered)
            Scale();
    }
    void Scale()
    {
        PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
        _hit = scene2D.Raycast(transform.position, transform.right, 1000f, _layerMask);
        transform.localScale = _hit? new Vector3(Vector2.Distance(transform.position, _hit.point),transform.localScale.y,1) : new Vector3(100,transform.localScale.y);
    }
    void Chasing()
    {
        Vector3 _direction = _target.position - transform.position;
        _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, _angle);
    }

    public override void Rpc_Init()
    {
    }

    public void Init(int damage, Vector3 position, Transform target,float width =0.5f, float warningTime = 0.5f, float chasingTime = 0f, bool isPiercing = true)
    {
        gameObject.SetActive(true);
        _players.Clear();
        _isFiered = false;

        _damage = damage;

        transform.position = position;

        laserSprite.localScale = new Vector3(laserSprite.localScale.x, width, 1);

        _target = target;
        //_direction = _target.position - transform.position;
        //_angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,_angle);

        _animator.SetFloat(AnimationHash.WarningTimeParameterHash, 1/warningTime);
        _chasingTime = chasingTime;
        _startTime = Time.time;
        _chasingEnd = false;

        _isPiercing = isPiercing;

        LayerMask a = LayerData.GroundPlaneLayerMask | LayerData.ShieldLayerMask | LayerData.GroundWallLayerMask;
        LayerMask b = a | LayerData.PlayerLayerMask;

        _layerMask = isPiercing? a : b;


    }
    public void Damage()
    {
        for(int i = 0; i < _players.Count; i++)
        {
            _players[i].Damage(_damage);
        }
    }
}
