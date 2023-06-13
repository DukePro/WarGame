namespace WarGame
{
    class Programm
    {
        static void Main()
        {
            Menu menu = new Menu();
            menu.Run();
        }
    }

    class Menu
    {
        private const string StartFight = "1";
        private const string MenuExit = "0";
        private Battlefield _battlefield = new Battlefield();

        public void Run()
        {
            bool isExit = false;
            string userInput;

            while (isExit == false)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine(StartFight + " - Начать бой!");
                Console.WriteLine(MenuExit + " - Выход");

                userInput = Console.ReadLine();

                switch (userInput)
                {
                    case StartFight:
                        _battlefield.PerformBattle();
                        break;

                    case MenuExit:
                        isExit = true;
                        break;
                }
            }
        }
    }

    class Battlefield
    {
        public void PerformBattle()
        {
            Console.WriteLine("Введите название первой страны");
            Squad _country1 = new Squad(Console.ReadLine());
            Console.WriteLine("Введите название второй страны");
            Squad _country2 = new Squad(Console.ReadLine());

            while (_country1.IsAllDead == false && _country2.IsAllDead == false)
            {
                _country2.TakeArmyDamage(_country1.GetAttackingSoldier());
                Console.WriteLine("___________________________________________________________________________________________________________________________");
                _country1.TakeArmyDamage(_country2.GetAttackingSoldier());
                Console.WriteLine("___________________________________________________________________________________________________________________________");
            }
        }
    }

    class Squad
    {
        private List<Soldier> _army;
        private int _currentIndex = 0;
        private int _attackingSoldierIndex = -1;
        public string CountryName;

        public Squad(string countryName)
        {
            CountryName = countryName;
            _army = CreateArmy();
        }

        public bool IsAllDead { get; private set; } = false;

        private List<Soldier> CreateArmy()
        {
            List<Soldier> squad = new List<Soldier>();
            Random random = new Random();
            int armySize = 20;

            Soldier[] soldiers = new Soldier[]
            {
                new Trooper(),
                new Marksman(),
                new MachineGunner(),
            };

            for (int i = 0; i < armySize; i++)
            {
                Soldier soldier = soldiers[random.Next(0, soldiers.Length)];
                soldier.Index = _currentIndex++;
                squad.Add(soldier);
            }

            return squad;
        }

        public Soldier GetAttackingSoldier()
        {
            if (_army.Count != 0)
            {
                if (_attackingSoldierIndex >= _army.Count() - 1)
                {
                    _attackingSoldierIndex = 0;
                }
                else
                {
                    _attackingSoldierIndex++;
                }

                Soldier soldier = _army[_attackingSoldierIndex];

                return soldier;
            }
            else
            {
                return null;
            }
        }

        public void TakeArmyDamage(Soldier soldier)
        {
            if (soldier != null)
            {

                for (int i = 0; i < soldier.TargetsAvailable; i++)
                {
                    soldier.AttackTarget(_army);
                    RemoveDead();

                    if (_army.Count() == 0)
                    {
                        IsAllDead = true;
                    }
                }
            }
            else
            {
                Console.WriteLine("Ошибка, аттакующие солдаты померли");
            }
        }

        private void RemoveDead() //удаляет трупы из основного
        {
            for (int i = 0; i < _army.Count; i++)
            {
                if (_army[i].Health <= 0)
                {
                    _army.RemoveAt(i);
                }
            }
        }
    }

    class Soldier
    {
        public int Index;
        protected int Armor;
        protected int HitDamage;
        protected int BaseDamage = 15;
        protected static Random Random = new Random();

        public Soldier(int index = 0)
        {
            Index = index;
            Health = 500;
            Armor = 30;
            HitDamage = 100;
            TargetsAvailable = 1;
        }

        public int Health { get; protected set; }
        public int TargetsAvailable { get; protected set; }

        public void AttackTarget(List<Soldier> enemySquad)
        {
            if(enemySquad.Count > 0) 
            {
                int targetIndex = ChooseTarget(enemySquad);
            
                if (enemySquad[targetIndex].Health > 0)
                {
                    enemySquad[targetIndex].TakeUnitDamage(CalculateDamage());
                }
            }
        }

        protected virtual int CalculateDamage()
        {
            int alternateDamage = UseAttackAbility();

            double minDamageMod = 0.8;
            double maxDamageMod = 1.2;
            double damageMod = minDamageMod + (Random.NextDouble() * (maxDamageMod - minDamageMod));

            if (alternateDamage != 0)
            {
                int damage = alternateDamage;
                Console.Write($"Солдат {Index} Пытается нанести {damage} урона ");
                return damage;
            }
            else
            {
                int damage = (int)(HitDamage * damageMod);
                Console.Write($"Солдат {Index} Пытается нанести {damage} урона ");
                return damage;
            }
        }

        public virtual void TakeUnitDamage(int damage)
        {
            int reducedDamage = damage - Armor;

            if (reducedDamage < BaseDamage)
            {
                reducedDamage = BaseDamage;
            }
            else
            {
                Health -= reducedDamage;
            }
            if (Health > 0)
            {
                Console.WriteLine($"Солдат {Index} получает {reducedDamage} урона, остаётся {Health} хп.");
            }
            else
            {
                Console.WriteLine($"Солдат {Index} получает {reducedDamage} урона и померает.");
            }
        }

        protected virtual int UseAttackAbility()
        {
            return 0;
        }

        private int ChooseTarget(List<Soldier> enemySquad) //случайный выбор цели
        {
            int tempIndex = Random.Next(0, enemySquad.Count()); // получаем случайную ячейку массива
            return tempIndex;
        }
    }

    class Trooper : Soldier
    {
        public Trooper()
        {
            Health = 500;
            Armor = 30;
            HitDamage = 70;
        }
    }

    class Marksman : Soldier
    {
        public Marksman()
        {
            Health = 500;
            Armor = 0;
            HitDamage = 100;
            TargetsAvailable = 2;
        }

        protected override int UseAttackAbility()
        {
            double critMultiplier = 3;
            int critChance = 25;
            int CriticalShot = HitDamage;

            if (Random.Next(0, 100) < critChance)
            {
                CriticalShot = Convert.ToInt32(Math.Round(CriticalShot * critMultiplier));
                Console.WriteLine($"Солдат {Index} наносит критический урон!");
            }

            return CriticalShot;
        }
    }

    class MachineGunner : Soldier
    {
        public MachineGunner()
        {
            int minTargets = 1;
            int maxTargets = 5;
            Health = 500;
            Armor = 15;
            HitDamage = 70;
            TargetsAvailable = Random.Next(minTargets, maxTargets);
        }
    }
}