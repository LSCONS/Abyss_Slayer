using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    public PlayerIdleState IdleState { get; private set; }
    public float MovementSpeed {  get; private set; }
    public float MovementSpeedModifier { get; set; } = 1f;

    public PlayerStateMachine(Player player)
    {
        this.Player = player;

        IdleState = new PlayerIdleState();

        MovementSpeed = Player.playerData.PlayerGroundData.BaseSpeed;
    }
}
