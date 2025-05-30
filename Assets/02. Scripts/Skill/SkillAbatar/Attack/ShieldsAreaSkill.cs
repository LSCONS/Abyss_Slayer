using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using Fusion;

[CreateAssetMenu(menuName = "Skill/AreaSkill/ShieldAreaSkill")]
public class ShieldsAreaSkill : AreaSkill
{
    private FadeController shieldInstance = null;
    public override void Init()
    {
        base.Init();
    }
    public override void UseSkill()
    {
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if(shieldInstance == null)
        {
            shieldInstance = runner.Spawn(DataManager.Instance.ShieldPrefab);
        }
        shieldInstance.Rpc_Init(player.PlayerRef, radius, duration, effectAmount);
    }
}

