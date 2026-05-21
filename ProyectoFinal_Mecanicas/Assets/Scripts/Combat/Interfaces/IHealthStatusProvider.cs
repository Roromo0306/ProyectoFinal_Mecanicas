public interface IHealthStatusProvider
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsDead { get; }
}
