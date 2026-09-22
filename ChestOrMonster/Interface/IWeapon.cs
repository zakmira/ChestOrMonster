using ChestOrMonster.Model;

namespace ChestOrMonster.Interface;

public interface IWeapon : IBaseItem
{
    public double Damage { get; }
    public abstract DamageInfo Attack(double accuracy);
}