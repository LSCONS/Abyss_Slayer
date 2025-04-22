using UnityEngine;

public class MShockWave : BasePoolable
{
    private int damage;
    private float height, width;

    public override void Init()
    {

    }

    public void Init(Vector3 position, int damage, float height = 3, float width = 1f)
    {
        this.damage = damage;
        this.height = height;
        this.width = width;
        transform.position = position;
        transform.localScale = new Vector3(width, height, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Boss>(out Boss boss))
        {
            boss.Damage((int)damage); // 데미지 전달
            ReturnToPool(); // 풀에 반환
        }
    }
}
