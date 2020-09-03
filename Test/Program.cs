namespace Template
{
    // namespace
    using System;
    using System.Linq;

    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.MenuUI.Values;
    using EnsoulSharp.SDK.Prediction;
    using EnsoulSharp.SDK.Utility;
    using FunnySlayerCommon;
    using SharpDX;
    using Color = System.Drawing.Color;

    public class Program
    {
        private static Menu MainMenu = new Menu("MainMenu", "MainMenu " + GameObjects.Player.CharacterName, true);
        private static MenuList Prediction = new MenuList("Prediction", "Prediction : ", new string[] { "FS Prediction", "SDK Prediction" }, 0);

        private static Spell Q;

        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Ezreal")
            {
                return;
            }
            Q = new Spell(SpellSlot.Q, 1200f)
            {
                Delay = 0.25f,
                Width = 55f,
                Speed = 2000,
            };
            Q.SetSkillshot(0.25f, 55f, 2000f, true, SkillshotType.Line);
            FSpred.Prediction.Prediction.Initialize();
            MainMenu.Add(Prediction).Permashow();
            MainMenu.AddTargetSelectorMenu();
            MainMenu.Attach();

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            Drawing();
            if(Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                CastQ();
            }
        }

        private static void CastQ()
        {
            var target = FSTargetSelector.GetFSTarget(10000);
            if (target == null)
            {
                return;
            }

            var SDK = Q.GetPrediction(target).CastPosition;
            var FS = FSpred.Prediction.Prediction.GetPrediction(Q, target, false, -1, new FSpred.Prediction.CollisionableObjects[] { FSpred.Prediction.CollisionableObjects.Minions}).CastPosition;
            var fspred = FSpred.Prediction.Prediction.GetPrediction(Q, target, false, -1, new FSpred.Prediction.CollisionableObjects[] { FSpred.Prediction.CollisionableObjects.Minions });
            if (SDK != Vector3.Zero)
                Render.Circle.DrawCircle(SDK, 20, System.Drawing.Color.Blue, 10);

            if (FS != Vector3.Zero)
                Render.Circle.DrawCircle(FS, 10, System.Drawing.Color.Red, 10);

            if (Prediction.Index == 0)
            {
                if(FS != Vector3.Zero && FS.DistanceToPlayer() <= Q.Range && Q.IsReady() && fspred.Hitchance >= FSpred.Prediction.HitChance.High && fspred.CollisionObjects.All(i => !(i is AIHeroClient)))
                {
                    Q.Cast(FS);
                }
            }
            else
            {
                if(SDK.DistanceToPlayer() <= Q.Range && Q.IsReady())
                {
                    Q.Cast(SDK);
                }
            }
        }

        private static void Drawing()
        {
            var target = FSTargetSelector.GetFSTarget(10000);
            if (target == null)
            {
                return;
            }

            var SDK = Q.GetPrediction(target).CastPosition;
            var FS = FSpred.Prediction.Prediction.GetPrediction(Q, target, false, -1, new FSpred.Prediction.CollisionableObjects[] { FSpred.Prediction.CollisionableObjects.Minions }).CastPosition;           
        }
    }
}
