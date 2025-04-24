using UniRx;
using UnityEngine;
public interface IHasHealth
{
    ReactiveProperty<int> Hp { get; }
    ReactiveProperty<int> MaxHp { get; }
   // public abstract void Damage(int damage, float attackPosX = -1000);
}
