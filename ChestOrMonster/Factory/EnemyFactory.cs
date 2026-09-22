using ChestOrMonster.Interface;
using ChestOrMonster.Model;
using ChestOrMonster.Model.Enemy;
using ChestOrMonster.Model.Enemy.Boss;

namespace ChestOrMonster.Factory;

public static class EnemyFactory
{
    private static Random _random = Random.Shared;

    public static BaseEntity CreateRandomEnemy()
    {
        int roll = _random.Next(0, 4);
        return roll switch
        {
            0 => new Goblin(),
            1 => new Skeleton(),
            2 => new Mage(),
            3 => new Zombie()
        };
    }

    public static BaseEntity CreateRandomBoss()
    {
        int roll = _random.Next(0, 3);
        return roll switch
        {
            0 => new Orc(),
            1 => new GiantSkeleton(),
            2 => new Archmage()
        };
    }
}