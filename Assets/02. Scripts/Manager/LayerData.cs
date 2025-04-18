using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerData
{
    /// README
    /// gameObject.layer 또는 transform.layer로 레이어를 확인하는 방법은
    /// 반환값이 비트마스트의 int값이 아닌, 레이어 순서의 index값을 반환합니다
    /// 따라서 위의 경우에는 Index부분으로 비교 연산자로 체크하시면 됩니다.
    /// 
    /// 하지만 RayCast와 같이 비트마스크의 int값으로 반환되는 부분은 Index가 아닌
    /// Mask부분을 사용해야 합니다. 만일 마스크 값에서 다양한 마스크 값을 비교하고 싶을 경우
    /// 예)플레이어, 그라운드 동시 체크일 경우 -> GoundLayermask | PlayerLayerMask
    /// 와 같이 비트연산자로 합쳐서 넣어주시면 됩니다.

    public static readonly LayerMask GroundPlaneLayerIndex = LayerMask.NameToLayer("GroundPlane");
    public static readonly LayerMask GroundPlaneLayerMask = 1 << GroundPlaneLayerIndex;

    public static readonly LayerMask GroundPlatformLayerIndex = LayerMask.NameToLayer("GroundPlatform");
    public static readonly LayerMask GroundPlatformLayerMask = 1 << GroundPlatformLayerIndex;

    public static readonly LayerMask PlayerLayerIndex = LayerMask.NameToLayer("Player");
    public static readonly LayerMask PlayerLayerMask = 1 << PlayerLayerIndex;

    public static readonly LayerMask EnemyLayerIndex = LayerMask.NameToLayer("Enemy");
    public static readonly LayerMask EnemyLayerMask = 1 << EnemyLayerIndex;

    public static readonly LayerMask ShieldLayerIndex = LayerMask.NameToLayer("Shield");
    public static readonly LayerMask ShieldLayerMask = 1 << ShieldLayerIndex;
}
