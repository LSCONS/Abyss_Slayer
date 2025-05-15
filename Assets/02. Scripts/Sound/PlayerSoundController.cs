using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] private Player player;

    private void Awake()
    {
        if(player == null)
            player = PlayerManager.Instance.Player;
    }

    // enter 부분 딕셔너리에 매핑해놓기
    private Dictionary<System.Type, ESFXType> enterSFXMap = new()
    {
        {typeof(PlayerWalkState), ESFXType.Walk},
        {typeof(PlayerWalkState), ESFXType.Dash},
    };

    // exit 부분 매핑
    private Dictionary<System.Type, ESFXType> exitSFXMap = new()
    {
        {typeof(PlayerWalkState), ESFXType.Walk},
        {typeof(PlayerWalkState), ESFXType.Dash},
    };

    // 상태에 맞게 미리 사운드 메서드 넣어두기
    public void OnStateEnter(PlayerBaseState state)
    {
        if(enterSFXMap.TryGetValue(state.GetType(), out var SFXType))
        {
            if(SFXType == ESFXType.Walk)
                SoundManager.Instance.PlaySFX(SFXType, true, 1.5f);
            else
                return;
                //SoundManager.Instance.PlaySFX(SFXType);
        }
    }

    // 상태에 맞게 미리 사운드 메서드 넣어두기
    public void OnStateExit(PlayerBaseState state)
    {
        if (enterSFXMap.TryGetValue(state.GetType(), out var SFXType))
        {
            SoundManager.Instance.StopSFX(SFXType);
        }
    }
}
