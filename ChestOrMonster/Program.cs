using System.Text;
using ChestOrMonster.Factory;
using ChestOrMonster.Interface;
using ChestOrMonster.Model;
using ChestOrMonster.Model.Enemy;
using ChestOrMonster.Model.Enemy.Boss;
using ChestOrMonster.Model.Item;

namespace ChestOrMonster;

class Program
{
    private static Random _random = Random.Shared;

    private static Game _gameInstance;
    static void Main(string[] args)
    {
        Console.WriteLine("Добро пожаловать в игру \"Сундуки и Монстры\"!");
        StartMenu();
    }

    static void StartMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Добро пожаловать в игру \"Сундуки и Монстры\"!");
            Console.WriteLine("1. Начать");
            Console.WriteLine("2. Об игре");
            Console.WriteLine("0. Выйти");

            int playerChoice = UserChoice(0, 2);
            switch (playerChoice)
            {
                case 1:
                    Console.Clear();
                    Console.Write("Введите имя: ");
                    string playerName = Console.ReadLine() ?? string.Empty;
                    while (string.IsNullOrWhiteSpace(playerName))
                    {
                        Console.Write("Введите имя: ");
                        playerName = Console.ReadLine() ?? string.Empty;
                    }
                    StartGame(playerName);
                    break;
                case 2:
                    WriteHelp();
                    break;
                case 0:
                    Console.WriteLine("Прощай!");
                    return;
            }
        }
    }

    static void WriteHelp()
    {
        Console.Clear();
        Console.WriteLine("Игра \"Сундуки и Монстры\" представляет собой пошаговую текстовую игру рогалик.\nПрактические все элементы игры генерируются случайно.");
        Console.WriteLine("Вы искатель приключений и бродите по миру в поиске приключений на свою голову.\nНа каждом ходу вам может попасться *сундук* или *монстр*.");
        Console.WriteLine("Из сундука вам может выпасть предмет - лечебное зелье, оружие или доспех.\n\tОт оружия зависит сила атаки.\n\tОт доспехов зависит защита.");
        Console.WriteLine("Если вы наткнулись на врага, то начинается бой. Вы можете атаковать и защищаться. При выбора защиты, есть шанс до 40% полностью уклониться от атаки.");
        Console.WriteLine("\nПосле прочтения, нажмите любую кнопку.");
        Console.ReadKey();
    }

    static void StartGame(string playerName)
    {
        _gameInstance = new Game(playerName);
        while (true)
        {
            _gameInstance.MoveStep();
            Console.WriteLine($"Сейчас {_gameInstance.CurrentStep} ход.");
            if (_gameInstance.CurrentStep % 10 == 0)
            {
                StartFight(true);
            }
            else
            {
                switch (_gameInstance.CurrentStepType)
                {
                    case StepType.Chest:
                        StartChest();
                        break;
                    case StepType.Enemy:
                        bool fightResult = StartFight();
                        if (!fightResult)
                        {
                            return;
                        }
                        break;
                }
            }
            Thread.Sleep(1000);
        }
    }

    static void StartChest()
    {
        var item = ItemFactory.CreateRandomItem();
        Console.WriteLine($"Вам выпал {item.Name}!");
        switch (item)
        {
            case Weapon or Armor:
                ChangeEquipment(item);
                break;
            case HealingPotion:
                UseHealingPotion(item);
                break;
        }
    }

    static bool StartFight(bool isBoss = false)
    {
        BaseEntity enemy;
        if (isBoss)
        {
            enemy = EnemyFactory.CreateRandomBoss();
        }
        else
        {
            enemy = EnemyFactory.CreateRandomEnemy();
        }
        Console.WriteLine($"Вы наткнулись на {enemy.Name}!");
        while (_gameInstance.Player.Hp > 0 & enemy.Hp > 0)
        {
            bool dodged = false;
            Console.WriteLine($"Характеристики врага:\n\tИмя: {enemy.Name}\n\tHP: {enemy.Hp:F0}\n\tАтака: {enemy.Atk}\n\tЗащита: {enemy.Def}");
            Console.WriteLine($"Ваши характеристики:\n\tHP: {_gameInstance.Player.Hp:F0}\n\tАтака: {_gameInstance.Player.Weapon?.Damage}\n\tЗащита: {_gameInstance.Player.Armor?.Def}");
            switch (_gameInstance.Player.Effect)
            {
                case StatusEffect.Frozen:
                    Console.WriteLine("Вы заморожены! Пропуск вашего хода...");
                    _gameInstance.Player.UpdateStatusEffect();
                    break;
                case StatusEffect.None:
                    Console.WriteLine("Выберите действие:\n\t1. Атаковать\n\t2. Защищаться");
                    int playerChoice = UserChoice(1, 2);
                    switch (playerChoice)
                    {
                        case 1:
                            DamageInfo playerAtk = _gameInstance.Player.Attack();

                            if (playerAtk.Amount == 0)
                            {
                                Console.WriteLine($"Вы стреляете из {_gameInstance.Player.Weapon.Name}, но промахиваетесь!");
                            }
                            else
                            {
                                playerAtk = enemy.TakeDamage(playerAtk);
                                Console.WriteLine($"Вы нанесли врагу {playerAtk.Amount:F2} урона!");
                            }
                            break;
                        case 2:
                            if (_gameInstance.Player.Dodge())
                            {
                                Console.WriteLine("Вы уклонились от атаки врага!");
                                dodged = true;
                            }
                            else
                            {
                                Console.WriteLine("Вы не смогли уклониться!");
                            }
                            break;
                    }
                    break;
            }
            if (enemy.Hp > 0 && !dodged)
            {
                DamageInfo enemyAtk = enemy.Attack();
                enemyAtk = _gameInstance.Player.TakeDamage(enemyAtk);
                Console.WriteLine($"Враг нанёс вам {enemyAtk.Amount:F2}!");
            }
            Thread.Sleep(1000);
        }

        if (_gameInstance.Player.Hp <= 0)
        {
            Console.WriteLine($"Вас убил {enemy.Name}! Вы проиграли, GGWP :(");
            Thread.Sleep(15000);
            return false;
        }

        Console.WriteLine($"Вы убили {enemy.Name}!");
        return true;

    }

    static int UserChoice(int minChoice, int maxChoice)
    {
        while (true)
        {
            string choice = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(choice, out var choiceNumber))
            {
                continue;
            }

            if (choiceNumber < minChoice || choiceNumber > maxChoice)
            {
                continue;
            }
            return choiceNumber;
        }
    }

    static void UseHealingPotion(IBaseItem healingPotion)
    {
        Console.WriteLine($"У вас сейчас {_gameInstance.Player.Hp:F0} HP. Хотите выпить зелье или выбросить его?\n\t1. Выпить\t2. Выбросить");
        int playerChoice = UserChoice(1, 2);
        switch (playerChoice)
        {
            case 1:
                _gameInstance.Player.UseItem(healingPotion);
                Console.WriteLine("Вы восстановили HP до максимума!");
                break;
            case 2:
                Console.WriteLine("Вы выкинули лечебное зелье.");
                break;
        }
    }

    static void ChangeEquipment(IBaseItem equipment)
    {
        StringBuilder stringBuilder = new StringBuilder();
        switch (equipment)
        {
            case Weapon weapon:
                stringBuilder.AppendLine(
                    $"{($"Ваши характеристики сейчас:\n{_gameInstance.Player.Weapon.Name}, {_gameInstance.Player.Weapon.Damage}.")}");
                stringBuilder.AppendLine(
                    $"Характеристики нового оружия: {weapon.Name}, {weapon.Damage}");
                stringBuilder.AppendLine("Хотите сменить оружие или оставить текущее?");
                break;
            case Armor armor:
                stringBuilder.AppendLine(
                    $"{($"Ваши характеристики сейчас:\n{_gameInstance.Player.Armor.Name}, {_gameInstance.Player.Armor.Def}.")}");
                stringBuilder.AppendLine(
                    $"Характеристики новых доспехов: {armor.Name}, {armor.Def}");
                stringBuilder.AppendLine("Хотите сменить доспехи или оставить текующие?");
                break;
        }
        stringBuilder.AppendLine("\t1. Сменить\t2. Оставить");
        Console.WriteLine(stringBuilder.ToString());
        int playerChoice = UserChoice(1, 2);
        switch (playerChoice)
        {
            case 1:
                _gameInstance.Player.UseItem(equipment);
                break;
            case 2:
                Console.WriteLine($"Вы выкинули {equipment.Name}!");
                break;
        }
    }
}