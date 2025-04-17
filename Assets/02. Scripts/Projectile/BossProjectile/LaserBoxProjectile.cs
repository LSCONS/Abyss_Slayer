using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LaserBoxProjectile : BasePoolable
{
    [SerializeField] AnimationCurve curveX;
    [SerializeField] AnimationCurve curveY;
    [SerializeField] Transform[] transforms = new Transform[4];
    int _damage;
    Transform _target;
    float _scale;
    Vector3 _firePosition;
    float _moveTime;
    float _chasingTime;
    float _delayTime;
    bool _randomRotation;
    int _fireCount;
    public override void Init()
    {
    }
    public void Init(int damage, Transform target, Vector3 startPosition, float scale, Vector3 firePosition, float moveTime, float chasingTime, float delayTime, int fireCount = 1)
    {
        _damage = damage;
        _target = target;
        transform.position = startPosition;
        _scale = scale;
        transform.localScale = Vector3.zero;
        _firePosition = firePosition;
        _moveTime = moveTime;
        _chasingTime = chasingTime;
        _delayTime = delayTime;
        _fireCount = fireCount;

        _randomRotation = UnityEngine.Random.value > 0.5f;

        StartCoroutine(BoxStart());
    }

    IEnumerator BoxStart()
    {
        while ( transform.localScale.x < _scale - 0.05f)
        {
            
            transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one * _scale, 30 * Time.deltaTime);
            yield return null;
        }
        transform.localScale = Vector3.one * _scale;

        yield return new WaitForSeconds(0.5f);

        Vector3 startPos = transform.position;
        Vector3 deltaPos = _firePosition - startPos;
        float time = 0f;
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
                PoolManager.Instance.Get<Laser>().Init(_damage, transform.position, transforms[j], 0.5f, 0.5f, _chasingTime);
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
        }
        ReturnToPool();
    }

}
