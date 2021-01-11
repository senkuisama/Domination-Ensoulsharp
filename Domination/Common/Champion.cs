using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.Common
{
    using EnsoulSharp;
    using Menu = EnsoulSharp.SDK.MenuUI.Menu;
    using Spell = EnsoulSharp.SDK.Spell;
    using EnsoulSharp.SDK;
    using static EnsoulSharp.SDK.AntiGapcloser;

    internal abstract class Champion
    {
        public static AIHeroClient Player => ObjectManager.Player;
        internal static Menu ComboMenu { get; set; } = default(Menu);

        internal static Menu FarmMenu { get; set; } = default(Menu);
        internal static Menu EvadeMenu { get; set; }

        internal static Menu KillstealMenu { get; set; } = default(Menu);

        internal static Spell E { get; set; } = default(Spell);

        internal static Menu HarassMenu { get; set; } = default(Menu);

        internal static Menu DrawMenu { get; set; } = default(Menu);
        internal static Menu WhiteList { get; set; } = default(Menu);

        internal static Spell Q { get; set; } = default(Spell);

        internal static Spell R { get; set; } = default(Spell);
        internal static Spell W2 { get; set; } = default(Spell);
        internal static Menu RootMenu { get; set; } = default(Menu);

        internal static Spell W { get; set; } = default(Spell);
        internal static Spell Flash { get; set; } = default(Spell);

        internal void Initiate()
        {
            GameEvent.OnGameLoad += delegate
            {
                this.SetSpells();
                this.SetMenu();
                this.SetEvents();
            };
        }

        internal virtual void OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || Player.IsRecalling()) return;
            Killsteal();
            SemiR();
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.None: break;
                case OrbwalkerMode.Combo:
                    this.Combo();
                    break;
                case OrbwalkerMode.Harass:
                    this.Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    this.Farming();
                    break;
                case OrbwalkerMode.LastHit: break;
            }
        }


        internal virtual void SetEvents()
        {
            Game.OnUpdate += this.OnGameUpdate;
            EnsoulSharp.Drawing.OnDraw += Drawing;
            //Orbwalker.OnAction += OnAction;
            Spellbook.OnCastSpell += OnCastSpell;
            AIBaseClient.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnGapcloser += OnGapcloser;
        }

        internal virtual void OnGapcloser(AIBaseClient Sender, AntiGapcloser.GapcloserArgs args)
        {

        }

        internal virtual void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs e)
        {

        }


        /*internal virtual void OnAction(object sender, OrbwalkerActionArgs args)
        {

        }*/

        internal virtual void OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {

        }

        public void Drawing(EventArgs args)
        {
            Drawings();
        }


        protected abstract void Combo();

        protected abstract void SemiR();
        protected abstract void Farming();


        protected abstract void Drawings();
        protected abstract void Killsteal();
        protected abstract void Harass();

        protected abstract void SetMenu();

        protected abstract void SetSpells();
    }
}
