using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LaserBoxProjectile : BasePoolable
{
    [SerializeField] AnimationCurve curveX;
    [SerializeField] AnimationCurve curveY;
    [SerializeField] public Transform[] transforms = new Transform[4];
    [SerializeField] Transform virtualTarget;
    [SerializeField] SpriteRenderer sprite;
    int _damage;
    Transform _target;
    float _scale;
    Vector3 _firePosition;
    float _moveTime;
    float _chasingTime;
    float _delayTime;
    bool _randomRotation;
    int _fireCount;
    bool _isPiercing;
    bool _chasing;
    [Networked] public Vector3 BoxRotate {  get; set; }

    public void Update()
    {
        if(Runner.IsServer)
        {
            BoxRotate = transform.localEulerAngles;
        }
        else
        {
            transform.localEulerAngles = BoxRotate;
        }
    }


    public override void Rpc_Init()
    {
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(int damage, PlayerRef target, Vector3 startPosition, float scale, Vector3 firePosition, float moveTime, float chasingTime, float delayTime,bool isPiercing, int fireCount = 1, bool chasing = true)
    {
        gameObject.SetActive(true);
        _damage = damage;
        _target = ServerManager.Instance.DictRefToPlayer[target].transform;
        transform.position = startPosition;
        _scale = scale;
        transform.localScale = Vector3.zero;
        transform.rotation = Quaternion.identity;
        _firePosition = firePosition;
        _moveTime = moveTime;
        _chasingTime = chasingTime;
        _delayTime = delayTime;
        _fireCount = fireCount;
        _isPiercing = isPiercing;
        _chasing = chasing;
        sprite.color = chasing ? Color.white : Color.gray;

        _randomRotation = UnityEngine.Random.value > 0.5f;

        StartCoroutine(BoxStart());
    }

    IEnumerator BoxStart()
    {
        NetworkId objID = GetComponent<NetworkObject>().Id;
        float time = 0;
        while ( time < 0.5f * _delayTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * _scale, time / (0.5f * _delayTime));
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one * _scale;

        yield return new WaitForSeconds(0.5f * _delayTime);

        Vector3 startPos = transform.position;
        Vector3 deltaPos = _firePosition - startPos;
        time = 0f;
        while (time < _moveTime)
        {
            
            float x = startPos.x + deltaPos.x * curveX.Evaluate(time / _moveTime);
            float y = startPos.y + deltaPos.y * curveX.Evaluate(time / _moveTime);
            transform.position = new Vector3(x, y);

            transform.rotation = Quaternion.Euler(0, 0, 360 * time/_moveTime);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = _firePosition;

        for(int i = 0; i < _fireCount; i++)
        {
            if (!_chasing)
            {
                virtualTarget.SetParent(null);
                float degree = UnityEngine.Random.Range(-180, 180);
                virtualTarget.position = transform.position + new Vector3(Mathf.Sin(degree), Mathf.Cos(degree));
                _target = virtualTarget;
            }
            time = 0f;
            while (time < 1f)
            {
                Vector3 direction = _target.position - transform.position;
                float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, rotZ), 360 * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            for (int j = 0; j < 4; j++)
            {
                if(Runner.IsServer)
                PoolManager.Instance.Get<Laser>().Rpc_Init(_damage, transform.position, objID, j, 0.5f, 0.5f, _chasingTime,_isPiercing);
            }

            time = 0f;
            while (time < _chasingTime)
            {
                Vector3 direction = _target.position - transform.position;
                float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, rotZ);
                time += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            virtualTarget.SetParent(this.transform);
        }
        Rpc_ReturnToPool();
    }

}
