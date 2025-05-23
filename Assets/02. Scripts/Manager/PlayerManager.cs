using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerCustomizationInfo PlayerCustomizationInfo { get; private set; }
    public CharacterClass selectedCharacterClass { get; set; } = CharacterClass.Rogue;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    

    /// <summary>
    /// 현재 씬에서 플레이어를 찾고 등록하는 메서드
    /// 플레이어가 있는 씬으로 이동할 때마다 호출해야함.
    /// </summary>
    // 클래스 세팅해주는 메서드
    public void SetSelectedClass(CharacterClass selectedClass)
    {
        selectedCharacterClass = selectedClass;
        ServerManager.Instance.ThisPlayerData.Rpc_ChangeClass(selectedClass);
    }

    public void SetCustomization(int skinId, int faceId, (int style, int color) hairId)
    {
        ServerManager.Instance.ThisPlayerData.Rpc_InitPlayerCustom(hairId.style, hairId.color, skinId, faceId);
    }
}
