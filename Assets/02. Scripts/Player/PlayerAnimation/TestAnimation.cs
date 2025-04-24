using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public class TestAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private string addressKey = "cloth1_c1_bot";

    private List<Sprite> sortedFrames;
    private float frameDelay = 0.1f; // 10 FPS
    private float timer = 0f;
    private Sprite[] currentFrames;
    private int currentFrame;
    private async void Start()
    {
        await LoadAndSortSprites(addressKey);
        SetAnimation(AnimationState.Idle1);
    }


    // 스프라이트 로드한 다음에 정렬해줌
    private async System.Threading.Tasks.Task LoadAndSortSprites(string addressKey)
    {
        var handle = Addressables.LoadAssetAsync<Sprite[]>(addressKey);         // 우선 스프라이트 시트를 로드함 Sprite[]로 로드해서 스프라이트를 가져옴
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite[] spriteArray = handle.Result;
            sortedFrames = new List<Sprite>(spriteArray);

            // 스프라이트를 이름 숫자 부분으로 정렬함 (왜? addressable은 보장을 안해준대서)
            sortedFrames = spriteArray
                .OrderBy(sprite =>
                {
                    string name = sprite.name;
                    int number = int.Parse(name.Split('_').Last());
                    return number;
                }).ToList();

            // Debug.Log($"스프라이트 {sortedFrames.Count}개 로드 완료");
        }
    }

    // 애니메이션 스테이트 넣어주면 그 애니메이션을 재생해줌
    private void SetAnimation(AnimationState state)
    {
        if (sortedFrames == null) return;

        // SpriteSlicer로 정렬된 전체 시트(sortedFrames)를 애니메이션 상태별로 분리함
        var animationDict = SpriteSlicer.SliceSprite(sortedFrames.ToArray());

        // 선택된 애니메이션 상태에 해당되는 프레임의 배열을 currentFrames에 넣어줌 -> 업데이트에서 이 프레임 배열만 재생하는거임
        if (animationDict.TryGetValue(state, out var frames))
        {
            currentFrames = frames;
            currentFrame = 0;
            timer = 0f;
        }
    }


    // [테스트] 애니메이션 반복재생함. GetKeyDown을 추가해서 애니메이션 정상 재생되는지 확인가능
    private void Update()
    {
        if (sortedFrames == null || currentFrames.Length == 0)
            return;

        timer += Time.deltaTime;

        // frameDelay 초마다 다음 프레임으로 넘어감 (반복재생)
        if (timer >= frameDelay)   
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % currentFrames.Length;
            spriteRenderer.sprite = currentFrames[currentFrame];
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SetAnimation(AnimationState.Idle1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetAnimation(AnimationState.Idle2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetAnimation(AnimationState.Run1);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetAnimation(AnimationState.Run2);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetAnimation(AnimationState.Jump);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetAnimation(AnimationState.Fall);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SetAnimation(AnimationState.Attack1);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetAnimation(AnimationState.Attack2);
    }
}
