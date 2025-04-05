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

    [field: SerializeField]public PlayerData playerData { get; private set; }

    private void OnValidate()
    {
        input = GetComponent<PlayerInput>();
        playerControllerTest = GetComponent<PlayerControllerTest>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCheckGround = transform.GetComponentForTransformFindName<PlayerCheckGround>("Collider_GroundCheck");
    }
}
