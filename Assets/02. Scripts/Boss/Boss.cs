using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    GameObject targetCrosshairPrefab;
    public Transform targetCrosshair;
    public SpriteRenderer targetCrosshairRenderer;

    private BossStateMachine bossStateMachine;
    private void Awake()
    {
        targetCrosshair = Instantiate(targetCrosshairPrefab,transform).transform;
        targetCrosshairRenderer = targetCrosshair.GetComponent<SpriteRenderer>();
        bossStateMachine = new BossStateMachine(this);
    }

    private void Start()
    {
        bossStateMachine.ChangeState(bossStateMachine.StartState);
    }

    private void Update()
    {
        bossStateMachine.Update();
        bossStateMachine.HandleInput();
    }

    private void FixedUpdate()
    {
        bossStateMachine.FixedUpdate();
    }
}
