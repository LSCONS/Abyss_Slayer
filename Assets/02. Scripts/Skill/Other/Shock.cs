using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class Shock : BasePoolable
{
    private float damage, duration, delay;
    private LayerMask targetLayer;
    private BoxCollider2D boxCollider;
    private Coroutine deactivateCoroutine;

    private void Awake()
    {
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
    }

    public override void Init()
    {
        // 기본 풀링 초기화가 필요하면 여기에 작성
    }

    public void Init(Vector3 spawnPos, Vector2 size, float damage, float duration, float delay, LayerMask targetLayer, string effectName)
    {
        transform.position = spawnPos;
        transform.rotation = Quaternion.identity;

        this.damage = damage;
        this.duration = duration;
        this.delay = delay;
        this.targetLayer = targetLayer;

        if (boxCollider != null)
            boxCollider.size = size;

        if (deactivateCoroutine != null)
            StopCoroutine(deactivateCoroutine);
        deactivateCoroutine = StartCoroutine(DeactivateAfterDuration());
    }

    private IEnumerator DeactivateAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Boss>(out Boss boss))
        {
            boss.Damage((int)damage);
        }
    }
}
