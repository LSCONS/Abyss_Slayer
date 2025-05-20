using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareLaserPorjectile : BasePoolable
{
    int _damage;
    Vector3 _targetPosition;
    float _delayFireTime;
    float _targetTime;
    List<Player> players = new List<Player>();
    bool _onDamage;
    float _time;
    float _attackIntervalTime;
    [SerializeField] TrailRenderer _trailRenderer;
    [SerializeField] Animator _animator;
    private void Update()
    {
        if (!_onDamage) return;
        _time += Time.deltaTime;
        if (_time >= _attackIntervalTime)
        {
            _time = 0;
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Damage(_damage);
            }
        }
    }
    public override void Rpc_Init()
    {
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(int damage,float damageIntervalTime, Vector3 position,Vector3 targetPosition,float delayFireTime, float targetTime, float speed = 1 )
    {
        gameObject.SetActive(true);
        _time = 0f;
        _onDamage = false;
        players.Clear();

        _damage = damage;
        _attackIntervalTime = damageIntervalTime;
        _delayFireTime = delayFireTime;
        _targetTime = targetTime;
        transform.position = position;
        _targetPosition = targetPosition;
        _animator.SetFloat("Speed",speed);
        StartCoroutine(enumerator());
    }
    IEnumerator enumerator()
    {
        float time = 0f;
        while(time <= _delayFireTime / 2)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time / (_delayFireTime / 2f));
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;
        yield return new WaitForSeconds(_delayFireTime / 2);

        _trailRenderer.enabled = true;
        Vector3 startPos = transform.position;
        time = 0f;
        while(time <= _targetTime)
        {
            transform.position = Vector3.Lerp(startPos, _targetPosition, time / _targetTime);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = _targetPosition;
        yield return new WaitForSeconds(0.1f);

        _animator.SetTrigger("Fire");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player) && !players.Contains(player))
        {
            players.Add(player);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player player))
        {
            players.Remove(player);
        }
    }
    public void DamageOn()
    {
        _onDamage = true;
    }
    public void DamageOff()
    {
        _onDamage = false;
    }
    public override void ReturnToPool()
    {
        _trailRenderer.enabled = false;
        base.ReturnToPool();
    }
}
