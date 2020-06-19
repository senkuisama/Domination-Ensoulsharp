using EnsoulSharp;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace PRADA_Vayne.MyUtils
{
    public static class Traps
    {
        private static List<GameObject> _traps;

        private static readonly List<string> _trapNames = new List<string>
            {"teemo", "shroom", "trap", "mine", "ziggse_red"};

        public static List<GameObject> EnemyTraps
        {
            get { return _traps.FindAll(t => t.IsValid && t.IsEnemy); }
        }

        public static void OnCreate(GameObject sender, EventArgs args)
        {
            foreach (var trapName in _trapNames)
                if (sender.Name.ToLower().Contains(trapName))
                    _traps.Add(sender);
        }

        public static void OnDelete(GameObject sender, EventArgs args)
        {
            _traps.RemoveAll(trap => trap.NetworkId == sender.NetworkId);
        }

        public static void Load()
        {
            _traps = new List<GameObject>();
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }
    }

    public static class Turrets
    {
        private static List<AITurretClient> _turrets;

        public static List<AITurretClient> AllyTurrets
        {
            get { return _turrets.FindAll(t => t.IsAlly); }
        }

        public static List<AITurretClient> EnemyTurrets
        {
            get { return _turrets.FindAll(t => t.IsEnemy); }
        }

        public static void Load()
        {
            _turrets = ObjectManager.Get<AITurretClient>().ToList();
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender is AITurretClient) _turrets.Add((AITurretClient)sender);
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            _turrets.RemoveAll(turret => turret.NetworkId == sender.NetworkId);
        }
    }

    public static class HeadQuarters
    {
        private static List<HQClient> _headQuarters;

        public static HQClient AllyHQ
        {
            get { return _headQuarters.FirstOrDefault(t => t.IsAlly); }
        }

        public static HQClient EnemyHQ
        {
            get { return _headQuarters.FirstOrDefault(t => t.IsEnemy); }
        }

        public static void Load()
        {
            _headQuarters = ObjectManager.Get<HQClient>().ToList();
        }
    }

    public static class Heroes
    {
        private static List<AIHeroClient> _heroes;

        public static AIHeroClient Player = ObjectManager.Player;

        public static List<AIHeroClient> AllyHeroes
        {
            get { return _heroes.FindAll(h => h.IsAlly); }
        }

        public static List<AIHeroClient> EnemyHeroes
        {
            get { return _heroes.FindAll(h => h.IsEnemy); }
        }

        public static void Load()
        {
            Player = ObjectManager.Player;
            _heroes = ObjectManager.Get<AIHeroClient>().ToList();
        }
    }

    public static class Cache
    {
        public static void Load()
        {
            Traps.Load();
            Turrets.Load();
            HeadQuarters.Load();
            Heroes.Load();
        }
    }
}