using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;

namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        public static void OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!Program.E.IsReady() || !(sender is AIHeroClient)) return;

            var casterName = sender.CharacterName;
            var slot = args.Slot;
            var nameToLower = casterName.ToLower();
            var spellName = args.SData.Name;
            var spellNameToLower = spellName.ToLower();

            if (args.Target != null && args.Target.IsMe || ObjectManager.Player.Position.Distance(args.End) < 350)
            {
                if (spellName == "RenektonDice") Program.E.Cast(sender);
                if ((casterName == "Leona" || casterName == "Graves") &&
                    slot == SpellSlot.E)
                    Program.E.Cast(sender);
                if (casterName == "Alistar" && slot == SpellSlot.W) Program.E.Cast(sender);
                if (casterName == "Camille" && slot == SpellSlot.E && spellName != "CamilleE") Program.E.Cast(sender);
                if (casterName == "Diana" && slot == SpellSlot.R) Program.E.Cast(sender);
                if (casterName == "Shyvana" && slot == SpellSlot.R) Program.E.Cast(sender);
                if (casterName == "Akali" && slot == SpellSlot.R && args.SData.CooldownTime > 2.5)
                    Program.E.Cast(sender);
                if (spellName.ToLower().Contains("flash") && sender.IsMelee) Program.E.Cast(sender);
                if (nameToLower.Contains("zhao") || nameToLower.Contains("zix") && slot == SpellSlot.E)
                    Program.E.Cast(sender);
                if (casterName == "Pantheon" && slot == SpellSlot.W) Program.E.Cast(sender);
                if ((casterName == "Aatrox" || casterName == "Fiora" ||
                     casterName == "Fizz" || casterName == "Irelia" ||
                     casterName == "Jax") && slot == SpellSlot.Q)
                    Program.E.Cast(sender);
                if ((casterName == "Ekko" || casterName == "Shen" ||
                     casterName == "Talon" || casterName == "Tryndamere" ||
                     casterName == "Zac") &&
                    slot == SpellSlot.E)
                    Program.E.Cast(sender);
                if (spellNameToLower == "elisespiderqcast" || spellNameToLower == "jaycetotheskies" ||
                    spellNameToLower == "riftwalk" || spellNameToLower == "rivenfeint" ||
                    spellNameToLower == "sejuaniarcticassault")
                    Program.E.Cast(sender);
                if (
                    casterName == "Yasuo" && args.Target != null && args.Target.IsMe && slot ==
                    SpellSlot.E)
                    Program.E.Cast(sender);
            }

            if (sender.Distance(ObjectManager.Player) < 550)
            {
                //INTERRUPTER
                if ((casterName == "Katarina" || casterName == "Caitlyn" ||
                     casterName == "Karthus" || casterName == "FiddleSticks" ||
                     casterName == "Galio" || casterName == "Jhin" ||
                     casterName == "Malzahar" || casterName == "MissFortune" ||
                     casterName == "Nunu" || casterName == "Pantheon" ||
                     casterName == "Sion" || casterName == "TwistedFate" ||
                     casterName == "TahmKench" || casterName == "Urgot" ||
                     casterName == "Velkoz" || casterName == "Warwick") &&
                    slot == SpellSlot.R)
                    Program.E.Cast(sender);
                if (casterName == "MasterYi" && slot == SpellSlot.W)
                    for (var i = 40; i < 425; i += 125)
                    {
                        var flags = NavMesh.GetCollisionFlags(sender.Position.ToVector2()
                            .Extend(Heroes.Player.Position.ToVector2(), -i).ToVector3());
                        if (flags != null && flags.HasFlag(CollisionFlags.Wall) ||
                            flags.HasFlag(CollisionFlags.Building))
                        {
                            Program.E.Cast(sender);
                            return;
                        }
                    }

                if (casterName == "Vi" && slot == SpellSlot.Q) Program.E.Cast(sender);
                if (casterName == "FiddleSticks" && slot == SpellSlot.W) Program.E.Cast(sender);
                if (casterName == "Vladimir" && slot == SpellSlot.E && sender.IsFacing(ObjectManager.Player))
                    Program.E.Cast(sender);
            }
        }
    }
}