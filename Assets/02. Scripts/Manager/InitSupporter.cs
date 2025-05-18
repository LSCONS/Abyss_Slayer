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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartCrossSlashInit(Vector3 position, bool isLeft, int damage, int typeNum, float speed = 1, float scale = 1f)
    {
        PoolManager.Instance.Get<CrossSlash>().Init(position, isLeft, damage, 2, speed, scale);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartDashClawEffectInit(int damage, Vector3 startPosition, bool isLeft, float distance = 15f, float preDelayTime = 1f, float attackTime = 1f)
    {
        PoolManager.Instance.Get<DashClawEffect>().Init(damage, startPosition, isLeft, distance, preDelayTime, attackTime);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartExplosionInit(Vector3 position, int damage, float size = 1, int spriteNum = 0)
    {
        PoolManager.Instance.Get<Explosion>().Init(position, damage, size, spriteNum);    //폭발이펙트 생성
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartTornadoInit(Vector3 position, int damage, float durationTime, float attackPerSec, float warningTime = 1f, float width = 1f)
    {
        PoolManager.Instance.Get<Tornado>().Init(position, damage, durationTime, attackPerSec, warningTime, width);
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartHomingProjectileInit(int damage, Vector3 position, Quaternion rotate, PlayerRef target, float speed, int HomingProjectileType, float delayFireTime = 0f, float homingPower = 10f, float homingTime = 3f, float explosionSize = 0.5f, int homingCurve = 0, int speedCurve = 0)
    {
        PoolManager.Instance.Get<HomingProjectile>().Init(damage, position, rotate, target, speed, HomingProjectileType, delayFireTime, homingPower, homingTime, explosionSize, homingCurve, speedCurve);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartFoxSphereProjectileInit(int damage, Vector3 startPosition, float preDelayTime, PlayerRef target, float speed, float distance)
    {
        PoolManager.Instance.Get<FoxSphereProjectile>().Init(damage, startPosition, preDelayTime, target, speed, distance);

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartLaserBoxProjectileInit(int damage, PlayerRef target, Vector3 startPosition, float scale, Vector3 firePosition, float moveTime, float chasingTime, float delayTime, bool isPiercing, int fireCount = 1)
    {
        PoolManager.Instance.Get<LaserBoxProjectile>().Init(damage, target, startPosition, scale, firePosition, moveTime, chasingTime, delayTime, isPiercing, fireCount);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartShockWaveInit(Vector3 position, int damage, float height = 1f, float width = 1f)
    {
        PoolManager.Instance.Get<ShockWave>().Init(position, damage, height, width);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartNormalSlashInit(Vector3 position, int damage, bool isleft, float angle, float speed = 1)
    {
        PoolManager.Instance.Get<NormalSlash>().Init(position, damage, isleft, angle, speed);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartJumpEffectInit(Vector3 position, float size = 1f)
    {
        PoolManager.Instance.Get<JumpEffect>().Init(position, size);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartGravityProjectileInit(int damage, Vector3 position, float speedX, Vector3 targetPosition, int piercingCount, float size = 3f, float gravityScale = 1f)
    {
        PoolManager.Instance.Get<GravityProjectile>().Init(damage, position, speedX, targetPosition, piercingCount, size, gravityScale);
    }
}
