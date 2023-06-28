using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ALIENgame
{
    public abstract class Enemy
    {
        public int Hp;
        public bool IsAlive = true;
        public bool isStunned = false;
        public int MaxHpStat;
        public int AttackStat;
        public int DefenceStat;
        public int Width;
        public int Height;
        public string Name;
        public Texture2D Appearance;
        public bool IsBoss = false;
        public double DefeatTimeStamp;//This is for the defeat animation in the battle system

        public abstract void Restore();
        public abstract void Load(ContentManager content);

        protected void Initialize()
        {
            Hp = MaxHpStat;
            IsAlive = true;
        }

        public abstract string Turn(GameState state);

        public void TakeDamage(int damage, GameTime gameTime)
        {
            DefeatTimeStamp = gameTime.TotalGameTime.TotalSeconds;
            if (!IsAlive)
                return;
            Hp -= damage - DefenceStat;
            if (Hp <= 0)
            {
                IsAlive = false;
                Hp = 0;
            }
            if (Hp > MaxHpStat)
                Hp = MaxHpStat;
        }
    }
    public class ScripulousFingore : Enemy
    {
        Random rng = new Random();
        public ScripulousFingore()
        {
            Restore();
        }

        public override void Restore()
        {
            Name = "Scripulous fingore";
            Width = 288 / 2;
            Height = 170 / 2;
            MaxHpStat = 1000;
            AttackStat = 30;
            DefenceStat = 0;
            Initialize();
        }

        public override void Load(ContentManager content)
        {
            Appearance = content.Load<Texture2D>("Content/Enemies/ScripulousFingore");
        }

        public override string Turn(GameState state)
        {
            int playerToAttack = state.GetRandomAlivePlayer(rng);
            state.DamagePlayer(playerToAttack, AttackStat);
            return "Scripulous fingore pointed at you...&w &nTook a lot of damage from intimidation!";
        }
    }

    public class HunterAlien : Enemy
    {
        Random rng = new Random();
        public HunterAlien()
        {
            Restore();
        }

        public override void Restore()
        {
            Name = "Hunter Alien";
            Width = 32 * 3;
            Height = 61 * 3;
            MaxHpStat = 60;
            AttackStat = 10;
            DefenceStat = 0;
            Initialize();
        }

        public override void Load(ContentManager content)
        {
            Appearance = content.Load<Texture2D>("Content/Enemies/HunterAlien");
        }

        public override string Turn(GameState state)
        {
            int playerToAttack = state.GetRandomAlivePlayer(rng);
            state.DamagePlayer(playerToAttack, AttackStat);
            return "The hunter alien attacked!";
        }
    }

    public class WolfAlien : Enemy
    {
        Random rng = new Random();
        public WolfAlien()
        {
            Restore();
        }

        public override void Restore()
        {
            Name = "Wolf Alien";
            Width = 32 * 3;
            Height = 32 * 3;
            MaxHpStat = 30;
            AttackStat = 5;
            DefenceStat = 0;
            Initialize();
        }

        public override void Load(ContentManager content)
        {
            Appearance = content.Load<Texture2D>("Content/Enemies/WolfAlien");
        }

        public override string Turn(GameState state)
        {
            int playerToAttack = state.GetRandomAlivePlayer(rng);
            state.DamagePlayer(playerToAttack, AttackStat);
            return "The wolf alien bit you!";
        }
    }

    public class AlienServant : Enemy
    {
        Random rng = new Random();
        public AlienServant()
        {
            Restore();
        }

        public override void Restore()
        {
            Name = "Alien servant";
            Width = 32 * 3;
            Height = 43 * 3;
            MaxHpStat = 40;
            AttackStat = 10;
            DefenceStat = 0;
            Initialize();
        }

        public override void Load(ContentManager content)
        {
            Appearance = content.Load<Texture2D>("Content/Enemies/AlienServant");
        }

        public override string Turn(GameState state)
        {
            int playerToAttack = state.GetRandomAlivePlayer(rng);
            state.DamagePlayer(playerToAttack, AttackStat);
            return "The alian servant hit you!";
        }
    }

    public class GoatAlien : Enemy
    {
        Random rng = new Random();
        public GoatAlien()
        {
            Restore();
        }

        public override void Restore()
        {
            Name = "Goat alien";
            Width = 32 * 3;
            Height = 38 * 3;
            MaxHpStat = 30;
            AttackStat = 10;
            DefenceStat = 0;
            Initialize();
        }

        public override void Load(ContentManager content)
        {
            Appearance = content.Load<Texture2D>("Content/Enemies/AlienGoat");
        }

        public override string Turn(GameState state)
        {
            int playerToAttack = state.GetRandomAlivePlayer(rng);
            state.DamagePlayer(playerToAttack, AttackStat);
            return "The alien goat rammed you!";
        }
    }


    public class ALIEN : Enemy
    {
        Random rng = new Random();
        public ALIEN()
        {
            Restore();
        }

        public override void Restore()
        {
            IsBoss = true;
            Name = "A.L.I.E.N";
            Width = 48 * 4;
            Height = 48 * 4;
            MaxHpStat = 700;
            AttackStat = 30;
            DefenceStat = 0;
            Initialize();
        }

        public override void Load(ContentManager content)
        {
            Appearance = content.Load<Texture2D>("Content/Enemies/ALIEN");
        }

        public override string Turn(GameState state)
        {
            int playerToAttack = state.GetRandomAlivePlayer(rng);
            state.DamagePlayer(playerToAttack, AttackStat);
            return "A.L.I.E.N attacked!";
        }
    }
}