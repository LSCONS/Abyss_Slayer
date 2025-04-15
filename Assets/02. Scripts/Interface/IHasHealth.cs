using UniRx;
public interface IHasHealth
{
    ReactiveProperty<int> Hp { get; }
    ReactiveProperty<int> MaxHp { get; }
}
