using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private int damage = 50;   // 화살 데미지

    // 충돌 발생 시 호출됨 (IsTrigger가 체크된 Collider2D와 충돌해야 작동)
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
