using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private int damage = 50;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //var boss = other.GetComponent<Boss>();
        //if (boss != null)
        //{
        //    boss.TakeDamage(damage);
        //    Destroy(gameObject); // 화살 제거
        //}
    }
}
