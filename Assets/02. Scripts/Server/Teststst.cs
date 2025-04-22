using System.Collections.Generic;
using UnityEngine;

public class Teststst : MonoBehaviour
{
    //각 State의 이름에 Sprite 배열만큼 선언해두기.
    public int[] idleAnimation = new int[6];
    public int[] walkAnimations = new int[8];
    public int[] jumpAnimations = new int[5];

    //Dictionary에 번호에 따른 Sprite를 저장함.
    public Dictionary<int, Sprite> hairSprite = new Dictionary<int, Sprite>();
    public Dictionary<int, Sprite> eyeSprite = new Dictionary<int, Sprite>();
    public Dictionary<int, Sprite> clothSprite = new Dictionary<int, Sprite>();
    public Dictionary<int, Sprite> skinSprite = new Dictionary<int, Sprite>();
    public Dictionary<int, Sprite> weaponSprite = new Dictionary<int, Sprite>();


    //아래 메서드로 idleAnimation부터 0번의 숫자가 차례대로 int[]배열에 초기화해줌.
    public void AnimationSetting()
    {
        int cur = 0;
        // 매번 List 안 쓰고 바로 배열 배열을 묶어서 순회
        int[][] groups = { idleAnimation, walkAnimations, jumpAnimations };

        foreach (var arr in groups)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = cur++;
            }
        }
    }

    //해당 매개변수 Dictionary에 같이 들어온 Sprite[]의 수만큼 집어넣고 초기화해줌.
    public void DictionarySetting(Dictionary<int, Sprite> dict, Sprite[] sprites)
    {
        for(int i = 0;i < sprites.Length;i++)
        {
            dict[i] = sprites[i];
        }
    }
}
