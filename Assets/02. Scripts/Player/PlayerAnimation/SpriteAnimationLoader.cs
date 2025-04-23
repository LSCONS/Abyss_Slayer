using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteAnimationData
{
    public static Dictionary<AnimationState, int> spriteFrame = new()      // 여기다가 스프라이트 어떻게 자를건지 룰을 정의함
    {
        { AnimationState.Idle1, 6 },
        { AnimationState.Idle2, 6 },
        { AnimationState.Run1, 8 },
        { AnimationState.Run2, 8 },
        { AnimationState.Jump, 4 },
        { AnimationState.Fall, 4 },
        { AnimationState.Attack1, 6 },
        { AnimationState.Attack2, 6 },
        { AnimationState.Attack3, 4 },
        { AnimationState.AirAtk1, 6 },
        { AnimationState.AirAtk2, 4 },
        { AnimationState.Casting1, 5 },
        { AnimationState.Casting2, 5 },
        { AnimationState.Hurt, 4 },
        { AnimationState.Dying, 5 },
        { AnimationState.Dash, 8 },
        { AnimationState.Block, 5 },
        { AnimationState.Roll, 8 }
    };

}


public static class SpriteSlicer
{

    /// <summary>
    /// 원본 스프라이트 시트 배열 넣어서 애니메이션을 분리해서 딕셔너리로 반환해주는 함수
    /// </summary>
    /// <param name="spriteSheetOrigin"></param>
    /// <returns></returns>
    public static Dictionary<AnimationState, Sprite[]> SliceSprite(Sprite[] spriteSheetOrigin)
    {
        var result = new Dictionary<AnimationState, Sprite[]>();
        int index = 0;

        foreach (var item in SpriteAnimationData.spriteFrame)
        {
            // 잘라낼 수 있는 길이 초과 방지
            if (index + item.Value > spriteSheetOrigin.Length)
            {
                Debug.LogWarning($"스프라이트 시트가 부족합니다. {item.Key} 자르기 실패");
                break;
            }

            var frames = new Sprite[item.Value];
            Array.Copy(spriteSheetOrigin, index, frames, 0, item.Value);
            result[item.Key] = frames;
            index += item.Value;
        }

        return result;
    }

}



public enum AnimationState
{
    Idle1,
    Idle2,
    Run1,
    Run2,
    Jump,
    Fall,
    Attack1,
    Attack2,
    Attack3,
    AirAtk1,
    AirAtk2,
    Casting1,
    Casting2,
    Hurt,
    Dying,
    Dash,
    Block,
    Roll
}
