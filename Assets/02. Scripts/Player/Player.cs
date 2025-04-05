using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInput input;
    public PlayerControllerTest playerControllerTest;
    public Rigidbody2D playerRigidbody;
    public PlayerCheckGround playerCheckGround;
    public CinemachineVirtualCamera mainCamera;//TODO: 나중에 초기화 필요
    public Animator PlayerAnimator {  get; private set; }//TODO: 나중에 초기화 필요

    private PlayerStateMachine playerStateMachine;

    [field: SerializeField]public PlayerData playerData { get; private set; }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        playerControllerTest = GetComponent<PlayerControllerTest>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCheckGround = transform.GetComponentForTransformFindName<PlayerCheckGround>("Collider_GroundCheck");
        playerStateMachine = new PlayerStateMachine(this);
    }

    private void Start()
    {
        playerStateMachine.ChangeState(playerStateMachine.IdleState);
    }

    private void Update()
    {
        playerStateMachine.Update();
        playerStateMachine.HandleInput();
    }

    private void FixedUpdate()
    {
        playerStateMachine.FixedUpdate();
    }
}
