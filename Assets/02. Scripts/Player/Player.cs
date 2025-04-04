using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInput input;
    public PlayerControllerTest playerControllerTest;
    public Rigidbody2D playerRigidbody;
    public PlayerCheckGround playerCheckGround;

    public PlayerData playerData;

    private void OnValidate()
    {
        input = GetComponent<PlayerInput>();
        playerControllerTest = GetComponent<PlayerControllerTest>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCheckGround = transform.GetComponentForTransformFindName<PlayerCheckGround>("Collider_GroundCheck");
    }
}
