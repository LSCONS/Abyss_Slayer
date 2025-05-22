using Fusion;
using System;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class InitSupporter : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        ServerManager.Instance.InitSupporter = this;
    }


    public void Rpc_StartCrossSlashInit(Vector3 position, bool isLeft, int damage, int hash, float speed = 1, float scale = 1f)
    {
        PoolManager.Instance.Get<CrossSlash>().Rpc_Init(position, isLeft, damage, hash, speed, scale);
    }


    public void Rpc_StartDashClawEffectInit(int damage, Vector3 startPosition, bool isLeft, float distance = 15f, float preDelayTime = 1f, float attackTime = 1f)
    {
        PoolManager.Instance.Get<DashClawEffect>().Rpc_Init(damage, startPosition, isLeft, distance, preDelayTime, attackTime);
    }


    public void Rpc_StartExplosionInit(Vector3 position, int damage, float size = 1, int spriteNum = 0)
    {
        PoolManager.Instance.Get<Explosion>().Rpc_Init(position, damage, size, spriteNum);    //폭발이펙트 생성
    }


    public void Rpc_StartTornadoInit(Vector3 position, int damage, float durationTime, float attackPerSec, float warningTime = 1f, float width = 1f)
    {
        PoolManager.Instance.Get<Tornado>().Rpc_Init(position, damage, durationTime, attackPerSec, warningTime, width);
    }




    public void Rpc_StartHomingProjectileInit(int damage, Vector3 position, Quaternion rotate, PlayerRef target, float speed, int HomingProjectileType, float delayFireTime = 0f, float homingPower = 10f, float homingTime = 3f, float explosionSize = 0.5f, int homingCurve = 0, int speedCurve = 0)
    {
        PoolManager.Instance.Get<HomingProjectile>().Rpc_Init(damage, position, rotate, target, speed, HomingProjectileType, delayFireTime, homingPower, homingTime, explosionSize, homingCurve, speedCurve);
    }


    public void Rpc_StartFoxSphereProjectileInit(int damage, Vector3 startPosition, float preDelayTime, PlayerRef target, float speed, float distance, int color)
    {
        PoolManager.Instance.Get<FoxSphereProjectile>().Rpc_Init(damage, startPosition, preDelayTime, target, speed, distance, color);

    }


    public void Rpc_StartLaserBoxProjectileInit(int damage, PlayerRef target, Vector3 startPosition, float scale, Vector3 firePosition, float moveTime, float chasingTime, float delayTime, bool isPiercing, int fireCount = 1, bool chasing = true)
    {
        PoolManager.Instance.Get<LaserBoxProjectile>().Rpc_Init(damage, target, startPosition, scale, firePosition, moveTime, chasingTime, delayTime, isPiercing, fireCount , chasing);
    }


    public void Rpc_StartShockWaveInit(Vector3 position, int damage, float height = 1f, float width = 1f)
    {
        PoolManager.Instance.Get<ShockWave>().Rpc_Init(position, damage, height, width);
    }


    public void Rpc_StartNormalSlashInit(Vector3 position, int damage, bool isleft, float angle, float speed = 1)
    {
        PoolManager.Instance.Get<NormalSlash>().Rpc_Init(position, damage, isleft, angle, speed);
    }


    public void Rpc_StartJumpEffectInit(Vector3 position, float size = 1f)
    {
        PoolManager.Instance.Get<JumpEffect>().Rpc_Init(position, size);
    }


    public void Rpc_StartGravityProjectileInit(int damage, Vector3 position, float speedX, PlayerRef target, float delayThrowTime, int piercingCount, float size = 3f, float gravityScale = 1f)
    {
        PoolManager.Instance.Get<GravityProjectile>().Rpc_Init(damage, position, speedX, target, delayThrowTime, piercingCount, size, gravityScale);
    }
}
