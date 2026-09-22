using ChestOrMonster.Interface;

namespace ChestOrMonster.Model.Item;

public class Weapon : IWeapon
{
    public string Name { get; private set; }
    public double Damage { get; private set; }

    public WeaponType Type { get; private set; }
    public double Accuracy { get; private set; }

    public Weapon(string name, double damage, WeaponType type, double accuracy)
    {
        Name = name;
        Damage = damage;
        Type = type;
        Accuracy = accuracy;
    }

    public DamageInfo Attack(double hitRoll)
    {
        if (Type == WeaponType.Ranged)
        {
            if (hitRoll > Accuracy)
            {
                return new DamageInfo(0, DamageType.Pure);
            }
        }
        return new DamageInfo(Damage, DamageType.Usual);
    }
}