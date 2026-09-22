using ChestOrMonster.Interface;
using ChestOrMonster.Model.Item;

namespace ChestOrMonster.Factory;

public static class ItemFactory
{
    private static Random _random = Random.Shared;

    private static readonly (string Name, double Damage, WeaponType Type, double Accuracy)[] Weapons =
     [
        ("Деревянный меч", 5, WeaponType.Melee, 1.0),
        ("Стальной меч", 10, WeaponType.Melee, 1.0),
        ("Боевой топор", 12, WeaponType.Melee, 1.0),
        ("Длинный лук", 8,  WeaponType.Ranged, 0.75),
        ("Магический посох",15, WeaponType.Melee, 1.0),
        ("Лук", 20, WeaponType.Ranged, 0.60)
     ];

    private static readonly (string Name, double Def)[] Armors =
    [
        ("Кожаная броня", 3),
        ("Кольчуга", 6),
        ("Латные доспехи", 10),
        ("Магический плащ", 8)
    ];

    public static IBaseItem CreateRandomItem()
    {
        int itemType = _random.Next(0, 3);
        return itemType switch
        {
            0 => CreateRandomWeapon(),
            1 => CreateRandomArmor(),
            2 => new HealingPotion()
        };
    }

    public static Weapon CreateWeapon(int index)
    {
        var w = Weapons[index];
        return new Weapon(w.Name, w.Damage, w.Type, w.Accuracy);
    }

    private static Weapon CreateRandomWeapon()
    {
        var template = Weapons[_random.Next(0, Weapons.Length)];
        return new Weapon(template.Name, template.Damage, template.Type, template.Accuracy);
    }

    private static Armor CreateRandomArmor()
    {
        var template = Armors[_random.Next(0, Armors.Length)];
        return new Armor(template.Name, template.Def);
    }
}