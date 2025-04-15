using UnityEngine;

public class PlayerSightTest : MonoBehaviour
{
    //TODO: 테스트 스크립트. 추후 삭제 예정
    public SpriteRenderer parentRenderer;
    private float sightLenght = 0.3f;
    private void Awake()
    {
        parentRenderer = transform.parent.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector3 temp = transform.localPosition;

        if(parentRenderer.flipX) temp.x = -sightLenght;
        else temp.x = sightLenght;

        transform.localPosition = temp;
    }
}
