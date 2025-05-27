using UniRx;
using UnityEngine;
public interface IHasHealth
{
    ReactiveProperty<int> Hp { get; }
    ReactiveProperty<int> MaxHp { get; }
    public abstract void Rpc_Damage(int damage, float attackPosX = -1000);
}
