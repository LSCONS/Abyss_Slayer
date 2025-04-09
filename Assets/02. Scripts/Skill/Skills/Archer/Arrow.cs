using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private int damage = 50;   // 화살 데미지
    private float maxRange;
    private Vector3 spawnPosition;

    private void Update()
    {
        if (Vector3.Distance(spawnPosition, transform.position) >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    public void SetRange(float range)
    {
        spawnPosition = transform.position;
        maxRange = range;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 객체에 Boss 컴포넌트가 있으면 데미지 적용
        //var boss = other.GetComponent<Boss>();
        //if (boss != null)
        //{
        //    boss.TakeDamage(damage);  // 데미지 전달
        //    Destroy(gameObject);      // 화살 제거
        //}
    }
}
