using ChestOrMonster.Interface;

namespace ChestOrMonster.Model.Enemy;

public class Zombie : BaseEntity
{
    public override string Name { get; }
    public override double Hp { get; protected set; }
    public override double Atk { get; }
    public override double Def { get; }
    public override DamageType AttackType { get; }
    public override StatusEffect Effect { get; protected set; }

    public Zombie()
    {
        Name = "Зомби";
        Hp = 10;
        Atk = 10;
        Def = 10;
        AttackType = DamageType.Pure;
        Effect = StatusEffect.Poison;
    }

    public override DamageInfo Attack()
    {
        return new DamageInfo(Atk, AttackType);
    }
}

