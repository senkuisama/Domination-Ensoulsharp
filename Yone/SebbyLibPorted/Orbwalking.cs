using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Color = System.Drawing.Color;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;


namespace SebbyLibPorted
{
    public static class Orbwalking
    {
        public delegate void AfterAttackEvenH(AttackableUnit unit, AttackableUnit target);

        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);

        public delegate void OnAttackEvenH(AttackableUnit unit, AttackableUnit target);

        public delegate void OnNonKillableMinionH(AttackableUnit minion);

        public delegate void OnTargetChangeH(AttackableUnit oldTarget, AttackableUnit newTarget);

        public enum OrbwalkingMode
        {
            LastHit, Mixed, LaneClear, Combo, Freeze, CustomMode, None
        }

        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fioraflurry", "garenq",
            "gravesmove", "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane",
            "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade",
            "gangplankqwrapper", "poppypassiveattack", "powerfist", "renektonpreexecute", "rengarq",
            "shyvanadoubleattack", "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble",
            "vie", "volibearq", "xenzhaocombotarget", "yorickspectral", "reksaiq", "itemtitanichydracleave", "masochism",
            "illaoiw", "elisespiderw", "fiorae", "meditate", "sejuaninorthernwinds","asheq"
        };

        private static readonly string[] NoAttacks =
        {
            "volleyattack", "volleyattackwithsound",
            "jarvanivcataclysmattack", "monkeykingdoubleattack", "shyvanadoubleattack", "shyvanadoubleattackdragon",
            "zyragraspingplantattack", "zyragraspingplantattack2", "zyragraspingplantattackfire",
            "zyragraspingplantattack2fire", "viktorpowertransfer", "sivirwattackbounce", "asheqattacknoonhit",
            "elisespiderlingbasicattack", "heimertyellowbasicattack", "heimertyellowbasicattack2",
            "heimertbluebasicattack", "annietibbersbasicattack", "annietibbersbasicattack2",
            "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack", "yorickspectralghoulbasicattack",
            "malzaharvoidlingbasicattack", "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
            "kindredwolfbasicattack"
        };

        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "masteryidoublestrike", "quinnwenhanced", "renektonexecute", "renektonsuperexecute",
            "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff"
        };

        private static readonly string[] NoCancelChamps = { "Kalista" };

        public static List<AIBaseClient> MinionListAA = new List<AIBaseClient>();

        public static int LastAATick;

        private static int DelayOnFire = 0;

        private static int DelayOnFireId = 0;

        private static int BrainFarmInt = -100;

        public static bool Attack = true;

        public static bool DisableNextAttack;

        public static bool Move = true;

        public static int LastAttackCommandT;

        public static int LastMoveCommandT;

        public static Vector3 LastMoveCommandPosition = Vector3.Zero;

        private static AttackableUnit _lastTarget;

        private static readonly AIHeroClient Player;

        private static int _delay;

        private static float _minDistance = 400;

        private static bool _missileLaunched;

        private static readonly string _CharacterName;

        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        private static int _autoattackCounter;

        static Orbwalking()
        {
            Player = ObjectManager.Player;
            _CharacterName = Player.CharacterName;
            AIBaseClient.OnProcessSpellCast += OnProcessSpell;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Spellbook.OnStopCast += SpellbookOnStopCast;
            //AttackableUnit.OnDamage += AIBaseClient_OnDamage;
            AttackableUnit.OnDelete += AIBaseClient_OnDelete;
            AIBaseClient.OnCreate += AIBaseClient_OnCreate;
        }

        private static void AIBaseClient_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null)
            {
                if (missile.SpellCaster.IsMe)
                {
                    //Console.WriteLine(Player.BoundingRadius + " dis " + (missile.Position.Distance(Player.Position)));
                }
            }
        }

        private static void AIBaseClient_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (DelayOnFire != 0 && missile != null && Player.AttackDelay > 1 / 2f)
            {
                if (missile.SpellCaster.IsMe && EnsoulSharp.SDK.Orbwalker.IsAutoAttack(missile.SData.Name) && (DelayOnFireId == (int)missile.NetworkId || DelayOnFireId == (int)missile.Target.NetworkId))
                {
                    var x = Variables.TickCount - DelayOnFire;

                    if (x < 110 - Game.Ping / 2)
                    {
                        BrainFarmInt -= 2;
                    }
                    else if (x > 130 - Game.Ping / 2)
                    {
                        BrainFarmInt += 2;
                    }
                    //Console.WriteLine(BrainFarmInt + " ADJ " + (Variables.TickCount - DelayOnFire));
                    //Console.WriteLine(missile.Target.BoundingRadius + " dis2 " + (missile.Position.Distance(missile.Target.Position)));
                }
            }
        }

        /*private static void AIBaseClient_OnDamage(AIBaseClient sender, AttackableUnitDamageEventArgs args)
        {
            if (args.SourceNetworkId == Player.NetworkId)
            {
                //Console.WriteLine("OD4 "+ (Variables.TickCount - DelayOnFire) );
            }
        }*/

        public static event BeforeAttackEvenH BeforeAttack;

        public static event OnAttackEvenH OnAttack;

        public static event AfterAttackEvenH AfterAttack;

        public static event OnTargetChangeH OnTargetChange;

        public static event OnNonKillableMinionH OnNonKillableMinion;

        private static void FireBeforeAttack(AttackableUnit target)
        {
            if (BeforeAttack != null)
            {
                BeforeAttack(new BeforeAttackEventArgs { Target = target });
            }
            else
            {
                DisableNextAttack = false;
            }
        }

        private static void FireOnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (OnAttack != null)
            {
                OnAttack(unit, target);
            }
        }

        private static void FireAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (AfterAttack != null && target.IsValidTarget())
            {
                AfterAttack(unit, target);
            }
        }

        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null && (!_lastTarget.IsValidTarget() || _lastTarget != newTarget))
            {
                OnTargetChange(_lastTarget, newTarget);
            }
        }

        private static void FireOnNonKillableMinion(AttackableUnit minion)
        {
            if (OnNonKillableMinion != null)
            {
                OnNonKillableMinion(minion);
            }
        }

        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static bool IsMelee(this AIBaseClient unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) ||
                   Attacks.Contains(name.ToLower());
        }

        public static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = Player.AttackRange + Player.BoundingRadius;
            if (target.IsValidTarget())
            {
                var aiBase = target as AIBaseClient;
                if (aiBase != null && Player.CharacterName == "Caitlyn")
                {
                    if (aiBase.HasBuff("caitlynyordletrapinternal"))
                    {
                        result += 650;
                    }
                }

                return result + target.BoundingRadius;
            }

            return result;
        }

        public static float GetAttackRange(AIHeroClient target)
        {
            var result = target.AttackRange + target.BoundingRadius;
            return result;
        }

        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.IsValidTarget())
            {
                return false;
            }

            var myRange = GetRealAutoAttackRange(target);
            var hero = target as AIHeroClient;
            if (hero != null)
            {
                return
                Vector2.DistanceSquared(
                   Prediction.Prediction.GetPrediction(hero, 0).CastPosition.ToVector2(), Player.Position.ToVector2()) <= myRange * myRange;
            }


            return
                Vector2.DistanceSquared(
                    target is AIBaseClient ? ((AIBaseClient)target).Position.ToVector2() : target.Position.ToVector2(),
                    Player.Position.ToVector2()) <= myRange * myRange;
        }

        public static float GetMyProjectileSpeed()
        {
            return IsMelee(Player) || _CharacterName == "Azir" || _CharacterName == "Thresh" || _CharacterName == "Velkoz" ||
                   _CharacterName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn")
                ? float.MaxValue
                : Player.BasicAttack.MissileSpeed;
        }

        public static bool CanAttack()
        {
            if (Player.CharacterName == "Graves")
            {
                var attackDelay = 1.0740296828d * 1000 * Player.AttackDelay - 716.2381256175d;
                if (Variables.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + attackDelay &&
                    Player.HasBuff("GravesBasicAttackAmmo1"))
                {
                    return true;
                }

                return false;
            }

            if (Player.CharacterName == "Jhin")
            {
                if (Player.HasBuff("JhinPassiveReload"))
                {
                    return false;
                }
            }

            return Variables.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + Player.AttackDelay * 1000;
        }

        public static bool CanMove(float extraWindup, bool disableMissileCheck = false)
        {
            if (_missileLaunched && Orbwalker.MissileCheck && !disableMissileCheck)
            {
                return true;
            }

            var localExtraWindup = 0;
            if (_CharacterName == "Rengar" && (Player.HasBuff("rengarqbase") || Player.HasBuff("rengarqemp")))
            {
                localExtraWindup = 200;
            }

            return NoCancelChamps.Contains(_CharacterName) ||
                   (Variables.GameTimeTickCount + Game.Ping / 2 >=
                    LastAATick + Player.AttackCastDelay * 1000 + extraWindup + localExtraWindup);
        }

        public static void SetMovementDelay(int delay)
        {
            _delay = delay;
        }

        public static void SetMinimumOrbwalkDistance(float d)
        {
            _minDistance = d;
        }

        public static float GetLastMoveTime()
        {
            return LastMoveCommandT;
        }

        public static Vector3 GetLastMovePosition()
        {
            return LastMoveCommandPosition;
        }

        public static void MoveTo(Vector3 position,
            float holdAreaRadius = 0,
            bool overrideTimer = false,
            bool useFixedDistance = true,
            bool randomizeMinDistance = true)
        {
            var playerPosition = Player.Position;

            if (playerPosition.DistanceSquared(position) < holdAreaRadius * holdAreaRadius)
            {
                if (Player.Path.Length > 0)
                {
                    Player.IssueOrder(GameObjectOrder.Stop, playerPosition);
                    LastMoveCommandPosition = playerPosition;
                    LastMoveCommandT = Variables.GameTimeTickCount - 70;
                }
                return;
            }

            var point = position;

            if (Player.DistanceSquared(point) < 150 * 150)
            {
                point = playerPosition.Extend(
                    position, randomizeMinDistance ? (_random.NextFloat(0.6f, 1) + 0.2f) * _minDistance : _minDistance);
            }
            var angle = 0f;
            var currentPath = Player.GetWaypoints();
            if (currentPath.Count > 1 && currentPath.PathLength() > 100)
            {
                var movePath = Player.GetPath(point);

                if (movePath.Length > 1)
                {
                    var v1 = currentPath[1] - currentPath[0];
                    var v2 = movePath[1] - movePath[0];
                    angle = v1.AngleBetween(v2.ToVector2());
                    var distance = movePath.Last().ToVector2().DistanceSquared(currentPath.Last());

                    if ((angle < 10 && distance < 500 * 500) || distance < 50 * 50)
                    {
                        return;
                    }
                }
            }

            if (Variables.GameTimeTickCount - LastMoveCommandT < 70 + Math.Min(60, Game.Ping) && !overrideTimer &&
                angle < 60)
            {
                return;
            }

            if (angle >= 60 && Variables.GameTimeTickCount - LastMoveCommandT < 60)
            {
                return;
            }

            Player.IssueOrder(GameObjectOrder.MoveTo, point);
            LastMoveCommandPosition = point;
            LastMoveCommandT = Variables.GameTimeTickCount;
        }

        public static void Orbwalk(AttackableUnit target,
            Vector3 position,
            float extraWindup = 90,
            float holdAreaRadius = 0,
            bool useFixedDistance = true,
            bool randomizeMinDistance = true)
        {
            if (Variables.GameTimeTickCount - LastAttackCommandT < 70 + Math.Min(60, Game.Ping))
            {
                return;
            }

            try
            {
                if (target.IsValidTarget() && CanAttack() && Attack)
                {
                    DisableNextAttack = false;
                    FireBeforeAttack(target);

                    if (!DisableNextAttack)
                    {
                        if (!NoCancelChamps.Contains(_CharacterName))
                        {
                            _missileLaunched = false;
                        }

                        if (Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                        {
                            LastAttackCommandT = Variables.GameTimeTickCount;
                            _lastTarget = target;
                        }

                        return;
                    }
                }

                if (CanMove(extraWindup) && Move)
                {
                    if (Orbwalker.LimitAttackSpeed && (Player.AttackDelay < 1 / 2.6f) && _autoattackCounter % 3 != 0 &&
                        !CanMove(500, true))
                    {
                        return;
                    }

                    MoveTo(position, Math.Max(holdAreaRadius, 30), false, useFixedDistance, randomizeMinDistance);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ResetAutoAttackTimer()
        {
            LastAATick = 0;
        }

        private static void SpellbookOnStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.Owner.IsValid && spellbook.Owner.IsMe && args.DestroyMissile && args.SpellStopCancelled)
            {
                ResetAutoAttackTimer();
            }
        }

        private static void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                var ping = Game.Ping;
                if (ping <= 30) //First world problems kappa
                {
                    DelayAction.Add(30 - ping, () => AIBaseClient_OnDoCast_Delayed(sender, args));
                    return;
                }

                AIBaseClient_OnDoCast_Delayed(sender, args);
            }
        }

        private static void AIBaseClient_OnDoCast_Delayed(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (IsAutoAttackReset(args.SData.Name))
            {
                ResetAutoAttackTimer();
            }

            if (IsAutoAttack(args.SData.Name))
            {
                FireAfterAttack(sender, args.Target as AttackableUnit);
                _missileLaunched = true;
            }
        }

        private static void OnProcessSpell(AIBaseClient unit, AIBaseClientProcessSpellCastEventArgs Spell)
        {
            try
            {
                var spellName = Spell.SData.Name;

                if (unit.IsMe && IsAutoAttackReset(spellName))
                {
                    ResetAutoAttackTimer();
                }

                if (!IsAutoAttack(spellName))
                {
                    return;
                }

                if (unit.IsMe &&
                    (Spell.Target is AIBaseClient || Spell.Target is Barracks || Spell.Target is HQClient))
                {
                    LastAATick = Variables.GameTimeTickCount - Game.Ping / 2;
                    _missileLaunched = false;
                    LastMoveCommandT = 0;
                    _autoattackCounter++;

                    if (Spell.Target is AIBaseClient)
                    {
                        var target = (AIBaseClient)Spell.Target;
                        if (target.IsValid)
                        {
                            FireOnTargetSwitch(target);
                            _lastTarget = target;
                        }
                    }
                }

                FireOnAttack(unit, _lastTarget);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public class BeforeAttackEventArgs : EventArgs
        {
            private bool _process = true;

            public AttackableUnit Target;

            public AIBaseClient Unit = ObjectManager.Player;

            public bool Process
            {
                get { return _process; }
                set
                {
                    DisableNextAttack = !value;
                    _process = value;
                }
            }
        }

        public class Orbwalker
        {

            private const float LaneClearWaitTimeMod = 2f;

            /// <summary>
            ///     The configuration
            /// </summary>
            private static Menu _config;

            /// <summary>
            ///     The instances of the orbwalker.
            /// </summary>
            public static List<Orbwalker> Instances = new List<Orbwalker>();

            /// <summary>
            ///     The player
            /// </summary>
            private readonly AIHeroClient Player;

            /// <summary>
            ///     The forced target
            /// </summary>
            private AIBaseClient _forcedTarget;

            /// <summary>
            ///     The orbalker mode
            /// </summary>
            private static OrbwalkingMode _mode = OrbwalkingMode.None;

            /// <summary>
            ///     The orbwalking point
            /// </summary>
            private Vector3 _orbwalkingPoint;

            /// <summary>
            ///     The previous minion the orbwalker was targeting.
            /// </summary>
            private AIMinionClient _prevMinion;

            /// <summary>
            ///     The name of the CustomMode if it is set.
            /// </summary>
            private string CustomModeName;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Orbwalker" /> class.
            /// </summary>
            /// <param name="attachToMenu">The menu the orbwalker should attach to.</param>
            public Orbwalker(Menu attachToMenu)
            {
                _config = attachToMenu;
                /* Drawings submenu */
                var drawings = new Menu("_Drawings", "drawings");
                drawings.Add(
                    new MenuColor("_AACircle", "AACircle", SharpDX.Color.Blue));
                drawings.Add(
                    new MenuColor("_AACircle2", "Enemy AA circle", SharpDX.Color.Blue));
                drawings.Add(
                    new MenuColor("_HoldZone", "HoldZone", SharpDX.Color.Blue));
                drawings.Add(new MenuSlider("_AALineWidth", "Line Width")).SetValue(new Slider(2, 1, 6));
                drawings.Add(new MenuBool("_LastHitHelper", "Last Hit Helper").SetValue(false));
                _config.Add(drawings);

                /* Misc options */
                var misc = new Menu("_Misc", "Misc");
                misc.Add(
                    new MenuSlider("_HoldPosRadius", "Hold Position Radius").SetValue(new Slider(0, 0, 250)));
                misc.Add(new MenuBool("_PriorizeFarm", "Priorize farm over harass").SetValue(true));
                misc.Add(new MenuBool("_AttackWards", "Auto attack wards").SetValue(false));
                misc.Add(new MenuBool("_AttackPetsnTraps", "Auto attack pets & traps").SetValue(true));
                misc.Add(new MenuBool("_AttackBarrel", "Auto attack gangplank barrel").SetValue(true));
                misc.Add(new MenuBool("_Smallminionsprio", "Jungle clear small first").SetValue(false));
                misc.Add(
                    new MenuBool("_LimitAttackSpeed", "Don't kite if Attack Speed > 2.5").SetValue(false));
                misc.Add(
                    new MenuKeyBind("_FocusMinionsOverTurrets", "Focus minions over objectives", Keys.M, KeyBindType.Toggle));

                _config.Add(misc);


                var sebbyFix = new Menu("_Sebby FIX", "Sebby FIX");

                sebbyFix.Add(new MenuSlider("_DamageAdjust", "Adjust last hit auto attack damage").SetValue(new Slider(0, -100, 100)));
                sebbyFix.Add(new MenuBool("_PassiveDmg", "Last hit include passive damage", true).SetValue(true));

                _config.Add(sebbyFix);
                /* Missile check */
                _config.Add(new MenuBool("_MissileCheck", "Use Missile Check").SetValue(true));

                /* Delay sliders */
                _config.Add(
                    new MenuSlider("_ExtraWindup", "Extra windup time").SetValue(new Slider(80, 0, 200)));
                _config.Add(new MenuSlider("_FarmDelay", "Farm delay").SetValue(new Slider(0, 0, 200)));

                /*Load the menu*/
                _config.Add(
                    new MenuKeyBind("_LastHit", "Last hit", Keys.X, KeyBindType.Press));

                _config.Add(new MenuKeyBind("_Farm", "Mixed", Keys.C, KeyBindType.Press));

                _config.Add(
                    new MenuKeyBind("_Freeze", "Freeze", Keys.N, KeyBindType.Press));

                _config.Add(
                    new MenuKeyBind("_LaneClear", "LaneClear", Keys.V, KeyBindType.Press));

                _config.Add(
                    new MenuKeyBind("_Orbwalk", "Combo", Keys.Space, KeyBindType.Press));

                _config.Add(
                    new MenuKeyBind("_StillCombo", "Combo without moving", Keys.N, KeyBindType.Press));

                _config.Item("_StillCombo").GetValue<MenuKeyBind>().ValueChanged +=
                    (sender, args) => { Move = !_config.GetValue<MenuKeyBind>("_StillCombo").Active; };

                _config.Item("_Orbwalk").GetValue<MenuKeyBind>().ValueChanged +=
                    (sender, args) =>
                    {
                        if (_config.Item("_Orbwalk").GetValue<MenuKeyBind>().Active == true)
                        {
                            EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.Combo;
                        }
                        else
                        {
                            if (_config.Item("_Farm").GetValue<MenuKeyBind>().Active)
                            {
                                EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.Harass;
                            }
                            else
                            {
                                if (_config.Item("_LaneClear").GetValue<MenuKeyBind>().Active)
                                {
                                    EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.LaneClear;
                                }
                                else
                                {
                                    if (_config.Item("_LastHit").GetValue<MenuKeyBind>().Active)
                                    {
                                        EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.LastHit;
                                    }
                                    else
                                    {
                                        EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.None;
                                    }
                                }
                            }
                        }
                    };
                _config.Item("_Farm").GetValue<MenuKeyBind>().ValueChanged +=
                    (sender, args) =>
                    {
                        if (_config.Item("_Farm").GetValue<MenuKeyBind>().Active == true)
                        {
                            EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.Harass;
                        }
                        else
                        {
                            if (_config.Item("_Orbwalk").GetValue<MenuKeyBind>().Active)
                            {
                                EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.Combo;
                            }
                            else
                            {
                                if (_config.Item("_LaneClear").GetValue<MenuKeyBind>().Active)
                                {
                                    EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.LaneClear;
                                }
                                else
                                {
                                    if (_config.Item("_LastHit").GetValue<MenuKeyBind>().Active)
                                    {
                                        EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.LastHit;
                                    }
                                    else
                                    {
                                        EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.None;
                                    }
                                }
                            }
                        }
                    };
                _config.Item("_LaneClear").GetValue<MenuKeyBind>().ValueChanged +=
                    (sender, args) =>
                    {
                        if (_config.Item("_LaneClear").GetValue<MenuKeyBind>().Active == true)
                        {
                            EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.LaneClear;
                        }
                        else
                        {
                            if (_config.Item("_Orbwalk").GetValue<MenuKeyBind>().Active)
                            {
                                EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.Combo;
                            }
                            else
                            {
                                if (_config.Item("_Farm").GetValue<MenuKeyBind>().Active)
                                {
                                    EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.Harass;
                                }
                                else
                                {
                                    if (_config.Item("_LastHit").GetValue<MenuKeyBind>().Active)
                                    {
                                        EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.LastHit;
                                    }
                                    else
                                    {
                                        EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.None;
                                    }
                                }
                            }
                        }
                    };
                _config.Item("_LastHit").GetValue<MenuKeyBind>().ValueChanged +=
                    (sender, args) =>
                    {
                        if (_config.Item("_LastHit").GetValue<MenuKeyBind>().Active == true)
                        {
                            EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.LastHit;
                        }
                        else
                        {
                            if (_config.Item("_Orbwalk").GetValue<MenuKeyBind>().Active)
                            {
                                EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.Combo;
                            }
                            else
                            {
                                if (_config.Item("_LaneClear").GetValue<MenuKeyBind>().Active)
                                {
                                    EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.LaneClear;
                                }
                                else
                                {
                                    if (_config.Item("_LaneClear").GetValue<MenuKeyBind>().Active)
                                    {
                                        EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.LastHit;
                                    }
                                    else
                                    {
                                        EnsoulSharp.SDK.Orbwalker.ActiveMode = OrbwalkerMode.None;
                                    }
                                }
                            }
                        }
                    };

                Player = ObjectManager.Player;
                Game.OnUpdate += GameOnOnGameUpdate;
                Game.OnUpdate += Game_OnUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Instances.Add(this);
            }

            private void Game_OnUpdate(EventArgs args)
            {
                if (ObjectManager.Player == null || Player.IsDead)
                    return;
                /*if (_config.Item("AACircle").GetValue<MenuColor>().Active)
                {
                    
                }*/
                Render.Circle.DrawCircle(
                        ObjectManager.Player.Position, ObjectManager.Player.GetRealAutoAttackRange(),
                        //System.Drawing.Color.Blue,
                        _config.Item("_AACircle").GetValue<MenuColor>().Color.ToSystemColor(),
                        _config.Item("_AALineWidth").GetValue<MenuSlider>().Value);
                /*if (_config.Item("_AACircle2").GetValue<MenuColor>().Active)
                {
                    
                }*/
                foreach (var target in
                        GameObjects.EnemyHeroes.Where(target => target.IsValidTarget(1175)))
                {
                    Render.Circle.DrawCircle(
                        target.Position, target.GetRealAutoAttackRange(),
                        //System.Drawing.Color.Blue,
                        _config.Item("_AACircle2").GetValue<MenuColor>().Color.ToSystemColor(),
                        _config.Item("_AALineWidth").GetValue<MenuSlider>().Value);
                }

                /*if (_config.Item("HoldZone").GetValue<MenuColor>().Active)
                {
                    
                }*/
                Render.Circle.DrawCircle(
                        ObjectManager.Player.Position, _config.Item("_HoldPosRadius").GetValue<MenuSlider>().Value,
                        //System.Drawing.Color.Blue,
                        _config.Item("_HoldZone").GetValue<MenuColor>().Color.ToSystemColor(),
                        _config.Item("_AALineWidth").GetValue<MenuSlider>().Value, true);
                /*_config.Item("FocusMinionsOverTurrets")
                    .PermaShowText(_config.Item("FocusMinionsOverTurrets").GetValue<MenuKeyBind>().Active);*/

                if (_config.Item("_LastHitHelper").GetValue<MenuBool>().Enabled)
                {
                    foreach (var minion in
                        Cache.MinionsListEnemy
                            .Where(
                                x => x.Name.ToLower().Contains("minion") && x.IsValidTarget() && x.IsValidTarget(1000)))
                    {
                        if (minion.Health < ObjectManager.Player.GetAutoAttackDamage(minion))
                        {
                            Render.Circle.DrawCircle(minion.Position, 50, Color.LimeGreen);
                        }
                    }
                }
            }

            private int FarmDelay
            {
                get { return _config.Item("_FarmDelay").GetValue<MenuSlider>().Value; }
            }

            public static bool MissileCheck
            {
                get { return _config.Item("_MissileCheck").GetValue<MenuBool>().Enabled; }
            }

            public static bool LimitAttackSpeed
            {
                get { return _config.Item("_LimitAttackSpeed").GetValue<MenuBool>().Enabled; }
            }

            public static OrbwalkingMode ActiveMode
            {
                get
                {
                    /*if (_mode != OrbwalkingMode.None)
                    {
                        return _mode;
                    }

                    if (_config.Item("_Orbwalk").GetValue<MenuKeyBind>().Active)
                    {
                        return OrbwalkingMode.Combo;
                    }

                    if (_config.Item("_StillCombo").GetValue<MenuKeyBind>().Active)
                    {
                        return OrbwalkingMode.Combo;
                    }

                    if (_config.Item("_LaneClear").GetValue<MenuKeyBind>().Active)
                    {
                        return OrbwalkingMode.LaneClear;
                    }

                    if (_config.Item("_Farm").GetValue<MenuKeyBind>().Active)
                    {
                        return OrbwalkingMode.Mixed;
                    }

                    if (_config.Item("_Freeze").GetValue<MenuKeyBind>().Active)
                    {
                        return OrbwalkingMode.Freeze;
                    }

                    if (_config.Item("_LastHit").GetValue<MenuKeyBind>().Active)
                    {
                        return OrbwalkingMode.LastHit;
                    }

                    if (_config.Item(CustomModeName) != null && _config.Item(CustomModeName).GetValue<MenuKeyBind>().Active)
                    {
                        return OrbwalkingMode.CustomMode;
                    }*/

                    return OrbwalkingMode.None;
                }
                set { _mode = value; }
            }

            public virtual bool InAutoAttackRange(AttackableUnit target)
            {
                return Orbwalking.InAutoAttackRange(target);
            }

            public virtual void RegisterCustomMode(string name, string displayname, uint key)
            {
                CustomModeName = name;
                if (_config.Item(name) == null)
                {
                    _config.AddItem(
                        new MenuKeyBind(name, displayname, (Keys)key, KeyBindType.Press));
                }
            }

            public void SetAttack(bool b)
            {
                Attack = b;
            }

            public void SetMovement(bool b)
            {
                Move = b;
            }

            public void ForceTarget(AIBaseClient target)
            {
                _forcedTarget = target;
            }

            public void SetOrbwalkingPoint(Vector3 point)
            {
                _orbwalkingPoint = point;
            }

            public bool ShouldWait()
            {
                var attackCalc = (int)(Player.AttackDelay * 1000 * 1.6) + Game.Ping / 2 + 1000 * 500 / (int)GetMyProjectileSpeed();
                return
                    MinionListAA.Any(
                        minion => HealthPrediction.LaneClearHealthPrediction(minion, attackCalc, FarmDelay) <= Player.GetAutoAttackDamage(minion));
            }

            private bool ShouldWaitUnderTurret(AIMinionClient noneKillableMinion)
            {
                var attackCalc = (int)(Player.AttackDelay * 1000 + (Player.IsMelee ? Player.AttackCastDelay * 1000 : Player.AttackCastDelay * 1000 +
                                               1000 * (Player.AttackRange + 2 * Player.BoundingRadius) / Player.BasicAttack.MissileSpeed));
                return
                    MinionListAA.Any(minion =>
                               (noneKillableMinion != null ? noneKillableMinion.NetworkId != minion.NetworkId : true) &&
                               HealthPrediction.LaneClearHealthPrediction(minion, attackCalc, FarmDelay) <= Player.GetAutoAttackDamage(minion));
            }

            public virtual AttackableUnit GetTarget()
            {
                AttackableUnit result = null;
                var mode = ActiveMode;
                //Forced target
                if (_forcedTarget.IsValidTarget() && _forcedTarget.InAutoAttackRange())
                {
                    return _forcedTarget;
                }

                if ((mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LaneClear) &&
                    !_config.Item("_PriorizeFarm").GetValue<MenuBool>().Enabled)
                {
                    var target = TargetSelector.GetTarget(-1, DamageType.Physical);
                    if (target != null && target.InAutoAttackRange())
                    {
                        return target;
                    }
                }

                if (_config.Item("_AttackBarrel").GetValue<MenuBool>().Enabled && ((mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)))
                {
                    var enemyGangPlank = GameObjects.EnemyHeroes.FirstOrDefault(e => e.CharacterName.Equals("gangplank", StringComparison.InvariantCultureIgnoreCase));

                    if (enemyGangPlank != null)
                    {
                        var barrels = Cache.GetMinions(Player.Position, 0, MinionTeam.Enemy).Where(minion => minion.Team == GameObjectTeam.Neutral && minion.SkinName == "gangplankbarrel" && minion.IsValidTarget() && minion.IsValidTarget() && minion.InAutoAttackRange());

                        foreach (var barrel in barrels)
                        {
                            if (barrel.Health <= 1f)
                                return barrel;

                            var t = (int)(Player.AttackCastDelay * 1000) + Game.Ping / 2 + 1000 * (int)Math.Max(0, Player.Distance(barrel) - Player.BoundingRadius) / (int)GetMyProjectileSpeed();

                            var barrelBuff = barrel.Buffs.FirstOrDefault(b => b.Name.Equals("gangplankebarrelactive", StringComparison.InvariantCultureIgnoreCase));

                            if (barrelBuff != null && barrel.Health <= 2f)
                            {
                                var healthDecayRate = enemyGangPlank.Level >= 13 ? 0.5f : (enemyGangPlank.Level >= 7 ? 1f : 2f);
                                var nextHealthDecayTime = Game.Time < barrelBuff.StartTime + healthDecayRate ? barrelBuff.StartTime + healthDecayRate : barrelBuff.StartTime + healthDecayRate * 2;

                                if (nextHealthDecayTime <= Game.Time + t / 1000f && ObjectManager.Get<EffectEmitter>().Any(x => x.Name == "Gangplank_Base_E_AoE_Red.troy" && barrel.Distance(x.Position) < 10))
                                    return barrel;
                            }
                        }

                        if (barrels.Any())
                            return null;

                    }
                }

                /*Killable Minion*/
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)
                {

                    var MinionList = Cache.GetMinions(Player.Position, 0, MinionTeam.Enemy).OrderBy(minion => HealthPrediction.GetHealthPrediction(minion, 1200));

                    foreach (var minion in MinionList)
                    {
                        if (minion.Team != GameObjectTeam.Neutral)
                        {
                            if (!ShouldAttackMinion(minion))
                                continue;

                            var t = (int)(Player.AttackCastDelay * 1000) + BrainFarmInt + Game.Ping / 2 + 1000 * (int)Math.Max(0, Player.Position.Distance(minion.Position) - Player.BoundingRadius) / (int)GetMyProjectileSpeed();

                            if (mode == OrbwalkingMode.Freeze)
                            {
                                t += 200 + Game.Ping / 2;
                            }

                            var predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay);


                            var damage = Player.CalculatePhysicalDamage(minion, 0) + _config.Item("DamageAdjust").GetValue<MenuSlider>().Value;


                            var killable = predHealth <= damage;

                            if (mode == OrbwalkingMode.Freeze)
                            {
                                if (minion.Health < 50 || predHealth <= 50)
                                {
                                    return minion;
                                }
                            }
                            else
                            {
                                if (CanAttack())
                                {

                                    DelayOnFire = t + Variables.TickCount;
                                    DelayOnFireId = (int)minion.NetworkId;
                                }

                                if (predHealth <= 0)
                                {
                                    if (HealthPrediction.GetHealthPrediction(minion, t - 50, FarmDelay) > 0)
                                    {
                                        FireOnNonKillableMinion(minion);
                                        return minion;
                                    }
                                }

                                else if (killable)
                                {
                                    return minion;
                                }
                            }
                        }
                    }
                }
                if (CanAttack())
                {
                    DelayOnFire = 0;
                }


                /* turrets / inhibitors / nexus */
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed)
                {
                    /* turrets */
                    foreach (var turret in
                        Cache.TurretList.Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    /* inhibitor */
                    foreach (var turret in
                        Cache.InhiList.Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    /* nexus */
                    foreach (var nexus in
                        Cache.NexusList.Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return nexus;
                    }
                }

                /*Champions*/
                if (mode != OrbwalkingMode.LastHit)
                {
                    var target = TargetSelector.GetTarget(-1, DamageType.Physical);
                    if (target.IsValidTarget() && target.InAutoAttackRange())
                    {
                        if (!ObjectManager.Player.IsUnderEnemyTurret() || mode == OrbwalkingMode.Combo)
                            return target;
                    }
                }

                /*Jungle minions*/
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed)
                {
                    var jminions = Cache.GetMinions(Player.Position, 0, MinionTeam.All);

                    result = _config.Item("_Smallminionsprio").GetValue<MenuBool>().Enabled
                        ? jminions.MinOrDefault(mob => mob.MaxHealth)
                        : jminions.MaxOrDefault(mob => mob.MaxHealth);

                    if (result != null)
                    {
                        return result;
                    }
                }

                /* UnderTurret Farming */
                if ((mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit ||
                    mode == OrbwalkingMode.Freeze) && CanAttack())
                {
                    var closestTower =
                        ObjectManager.Get<AITurretClient>().MinOrDefault(t => t.IsAlly &&
                        (t.Name.Contains("L_03_A") || t.Name.Contains("L_02_A") || t.Name.Contains("C_04_A") || t.Name.Contains("C_05_A") || t.Name.Contains("R_02_A") || t.Name.Contains("R_03_A"))
                        && !t.IsDead ? Player.DistanceSquared(t) : float.MaxValue);

                    if (closestTower != null && Player.DistanceSquared(closestTower) < 1500 * 1500)
                    {
                        AIMinionClient farmUnderTurretMinion = null;
                        AIMinionClient noneKillableMinion = null;
                        // return all the minions underturret in auto attack range
                        var minions = MinionListAA.Where(minion =>
                            closestTower.DistanceSquared(minion) < 900 * 900)
                            .OrderByDescending(minion => minion.SkinName.Contains("Siege"))
                            .ThenBy(minion => minion.SkinName.Contains("Super"))
                            .ThenByDescending(minion => minion.MaxHealth)
                            .ThenByDescending(minion => minion.Health);

                        if (minions.Any())
                        {
                            // get the turret aggro minion
                            var turretMinion =
                                minions.FirstOrDefault(
                                    minion =>
                                        minion is AIMinionClient &&
                                        HealthPrediction.HasTurretAggro(minion as AIMinionClient));

                            if (turretMinion != null)
                            {
                                var hpLeftBeforeDie = 0;
                                var hpLeft = 0;
                                var turretAttackCount = 0;
                                var turretStarTick = HealthPrediction.TurretAggroStartTick(
                                    turretMinion as AIMinionClient);
                                // from healthprediction (don't blame me :S)
                                var turretLandTick = turretStarTick + (int)(closestTower.AttackCastDelay * 1000) +
                                                     1000 *
                                                     Math.Max(
                                                         0,
                                                         (int)
                                                             (turretMinion.Distance(closestTower) -
                                                              closestTower.BoundingRadius)) /
                                                     (int)(closestTower.BasicAttack.MissileSpeed + 70);
                                // calculate the HP before try to balance it
                                for (float i = turretLandTick + 50;
                                    i < turretLandTick + 10 * closestTower.AttackDelay * 1000 + 50;
                                    i = i + closestTower.AttackDelay * 1000)
                                {
                                    var time = (int)i - Variables.GameTimeTickCount + Game.Ping / 2;
                                    var predHP =
                                        (int)
                                            HealthPrediction.LaneClearHealthPrediction(
                                                turretMinion, time > 0 ? time : 0);
                                    if (predHP > 0)
                                    {
                                        hpLeft = predHP;
                                        turretAttackCount += 1;
                                        continue;
                                    }
                                    hpLeftBeforeDie = hpLeft;
                                    hpLeft = 0;
                                    break;
                                }
                                // calculate the hits is needed and possibilty to balance
                                if (hpLeft == 0 && turretAttackCount != 0 && hpLeftBeforeDie != 0)
                                {
                                    var damage = (int)Player.GetAutoAttackDamage(turretMinion);
                                    var hits = hpLeftBeforeDie / damage;
                                    var timeBeforeDie = turretLandTick +
                                                        (turretAttackCount + 1) *
                                                        (int)(closestTower.AttackDelay * 1000) -
                                                        Variables.GameTimeTickCount;
                                    var timeUntilAttackReady = LastAATick + (int)(Player.AttackDelay * 1000) >
                                                               Variables.GameTimeTickCount + Game.Ping / 2 + 25
                                        ? LastAATick + (int)(Player.AttackDelay * 1000) -
                                          (Variables.GameTimeTickCount + Game.Ping / 2 + 25)
                                        : 0;
                                    var timeToLandAttack = Player.IsMelee
                                        ? Player.AttackCastDelay * 1000
                                        : Player.AttackCastDelay * 1000 +
                                          1000 * Math.Max(0, turretMinion.Distance(Player) - Player.BoundingRadius) /
                                          Player.BasicAttack.MissileSpeed;
                                    if (hits >= 1 &&
                                        hits * Player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack <
                                        timeBeforeDie)
                                    {
                                        farmUnderTurretMinion = turretMinion as AIMinionClient;
                                    }
                                    else if (hits >= 1 &&
                                             hits * Player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack >
                                             timeBeforeDie)
                                    {
                                        noneKillableMinion = turretMinion as AIMinionClient;
                                    }
                                }
                                else if (hpLeft == 0 && turretAttackCount == 0 && hpLeftBeforeDie == 0)
                                {
                                    noneKillableMinion = turretMinion as AIMinionClient;
                                }
                                // should wait before attacking a minion.
                                if (ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }
                                if (farmUnderTurretMinion != null)
                                {
                                    return farmUnderTurretMinion;
                                }
                                // balance other minions
                                foreach (var minion in
                                    minions.Where(
                                        x =>
                                            x.NetworkId != turretMinion.NetworkId && x is AIMinionClient &&
                                            !HealthPrediction.HasMinionAggro(x as AIMinionClient)))
                                {
                                    var playerDamage = (int)Player.GetAutoAttackDamage(minion);
                                    var turretDamage = (int)closestTower.GetAutoAttackDamage(minion);
                                    var leftHP = (int)minion.Health % turretDamage;
                                    if (leftHP > playerDamage)
                                    {
                                        return minion;
                                    }
                                }
                                // late game
                                var lastminion =
                                    minions.LastOrDefault(x => x.NetworkId != turretMinion.NetworkId && x is AIMinionClient &&
                                            !HealthPrediction.HasMinionAggro(x as AIMinionClient));
                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (1f / Player.AttackDelay >= 1f &&
                                        (int)(turretAttackCount * closestTower.AttackDelay / Player.AttackDelay) *
                                        Player.GetAutoAttackDamage(lastminion) > lastminion.Health)
                                    {
                                        return lastminion;
                                    }
                                    if (minions.Count() >= 5 && 1f / Player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            else
                            {
                                if (ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }
                                // balance other minions
                                foreach (var minion in
                                    minions.Where(
                                        x => x is AIMinionClient && !HealthPrediction.HasMinionAggro(x as AIMinionClient))
                                    )
                                {
                                    if (closestTower != null)
                                    {
                                        var playerDamage = (int)Player.GetAutoAttackDamage(minion);
                                        var turretDamage = (int)closestTower.GetAutoAttackDamage(minion);
                                        var leftHP = (int)minion.Health % turretDamage;
                                        if (leftHP > playerDamage)
                                        {
                                            return minion;
                                        }
                                    }
                                }
                                //late game
                                var lastminion =
                                    minions
                                        .LastOrDefault(x => x is AIMinionClient && !HealthPrediction.HasMinionAggro(x as AIMinionClient));
                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (minions.Count() >= 5 && 1f / Player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            return null;
                        }
                    }
                }

                /*Lane Clear minions*/
                if (mode == OrbwalkingMode.LaneClear)
                {
                    if (!ShouldWait())
                    {
                        if (_prevMinion.IsValidTarget() && InAutoAttackRange(_prevMinion))
                        {
                            var predHealth = HealthPrediction.LaneClearHealthPrediction(
                                _prevMinion, (int)(Player.AttackDelay * 1000 * LaneClearWaitTimeMod), FarmDelay);
                            if (predHealth >= 2 * Player.GetAutoAttackDamage(_prevMinion) ||
                                Math.Abs(predHealth - _prevMinion.Health) < float.Epsilon)
                            {
                                return _prevMinion;
                            }
                        }

                        result = (from minion in
                            MinionListAA.Where(
                                    minion => ShouldAttackMinion(minion, false))
                                  let predHealth =
                                      HealthPrediction.LaneClearHealthPrediction(
                                          minion, (int)(Player.AttackDelay * 1000 * LaneClearWaitTimeMod), FarmDelay)
                                  where
                                      predHealth >= 2 * Player.GetAutoAttackDamage(minion) ||
                                      Math.Abs(predHealth - minion.Health) < float.Epsilon
                                  select minion).MaxOrDefault(
                                m => m.Health);

                        if (result != null)
                        {
                            _prevMinion = (AIMinionClient)result;
                        }
                    }
                }

                return result;
            }

            private bool ShouldAttackMinion(AIBaseClient minion, bool includeBarrel = false)
            {
                if (minion.Name == "WardCorpse" || minion.SkinName == "jarvanivstandard")
                {
                    return false;
                }

                if (minion.Team == GameObjectTeam.Neutral && includeBarrel)
                {
                    return _config.Item("_AttackBarrel").GetValue<MenuBool>().Enabled &&
                           minion.SkinName == "gangplankbarrel" && minion.IsValidTarget();
                }
                var minion2 = minion as AIMinionClient;
                /*if ()
                {
                    return _config.Item("AttackWards").GetValue<MenuBool>().Enabled;
                }*/

                return (_config.Item("_AttackPetsnTraps").GetValue<MenuBool>().Enabled || SPredictionMash1.MinionManager.IsMinion(minion2)) &&
                       minion.SkinName != "gangplankbarrel";
            }

            /// <summary>
            ///     Fired when the game is updated.
            /// </summary>
            /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
            private void GameOnOnGameUpdate(EventArgs args)
            {
                try
                {
                    if (ActiveMode == OrbwalkingMode.None)
                    {
                        return;
                    }

                    //Prevent canceling important spells
                    if (Player.IsCastingInterruptableSpell(true))
                    {
                        return;
                    }
                    MinionListAA = Cache.GetMinions(Player.Position, 0);
                    var target = GetTarget();
                    if(target != null)

                    Orbwalk(
                        target, _orbwalkingPoint.ToVector2().IsValid() ? _orbwalkingPoint : Game.CursorPos,
                        _config.Item("_ExtraWindup").GetValue<MenuSlider>().Value,
                        Math.Max(_config.Item("_HoldPosRadius").GetValue<MenuSlider>().Value, 30));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            private void DrawingOnOnDraw(EventArgs args)
            {

            }
        }
    }
}