using DominationAIO.Champions.Helper;
using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyLogic.Q;
using PRADA_Vayne.MyUtils;
using PRADA_Vayne.Utils;

namespace PRADA_Vayne.MyLogic.Others
{
    public static partial class Events
    {
        public static void OnProcessSpellcast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (args.Target == null) return;

            #region ward brush after condemn

            if (sender.IsMe && args.SData.Name.ToLower().Contains("condemn") && args.Target.IsValid)
            {
                var target = (AIBaseClient)args.Target;
                if(target != null)
                {
                    if (Program.MainMenu.GetMenuBool("Combo Settings", "EQ") && target.IsVisible &&
                    !target.HasBuffOfType(BuffType.Stun) && Program.Q.IsReady()) //#TODO: fix
                    {
                        var tumblePos = target.GetTumblePos();
                        Tumble.Cast(tumblePos);
                    }
                }              
                /*if (NavMesh.IsWallOfType(args.End, 100))
                {
                    var blueTrinket = ItemId.Farsight_Alteration;
                    if (Items.HasItem((int)ItemId.Farsight_Alteration, Heroes.Player) &&
                        Items.CanUseItem((int)ItemId.Farsight_Alteration))
                        blueTrinket = ItemId.Farsight_Alteration;

                    var yellowTrinket = ItemId.Warding_Totem;
                    if (Items.HasItem((int)ItemId.Greater_Stealth_Totem_Trinket, Heroes.Player))
                        yellowTrinket = ItemId.Greater_Stealth_Totem_Trinket;

                    if (Items.CanUseItem((int)blueTrinket))
                        Items.UseItem((int)blueTrinket, args.End.Randomize(0, 100));
                    if (Items.CanUseItem((int)yellowTrinket))
                        Items.UseItem((int)yellowTrinket, args.End.Randomize(0, 100));
                }*/
            }

            #endregion ward brush after condemn

            #region Anti-Stealth

            if (args.SData.Name.ToLower().Contains("talonshadow")) //#TODO get the actual buff name
            {
                if (ObjectManager.Player.HasItem((int)ItemId.Oracle_Alteration) &&
                    ObjectManager.Player.CanUseItem((int)ItemId.Oracle_Alteration))
                    ObjectManager.Player.UseItem((int)ItemId.Oracle_Alteration, Heroes.Player.Position);
                else if (ObjectManager.Player.HasItem((int)ItemId.Control_Ward))
                    ObjectManager.Player.UseItem((int)ItemId.Control_Ward, Heroes.Player.Position.Randomize());

            }

            #endregion Anti-Stealth

            if (MyWizard.ShouldSaveCondemn()) return;
            if (sender.Distance(Heroes.Player) > 1000 || !args.Target.IsMe || args.SData == null)
                return;
            //how to milk alistar/thresh/everytoplaner
            var spellData = SpellDb.GetByName(args.SData.Name);
            if (spellData != null  &&
                !Lists.UselessChamps.Contains(sender.CharacterName))
                if (spellData.CcType == CcType.Knockup || spellData.CcType == CcType.Stun ||
                    spellData.CcType == CcType.Knockback || spellData.CcType == CcType.Suppression)
                    Program.E.Cast(sender);
        }
    }
}