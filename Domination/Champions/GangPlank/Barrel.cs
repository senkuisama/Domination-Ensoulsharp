using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
namespace e.Motion_Gangplank
{
    internal class Barrel
    {
        
        public int BarrelAttackTick;
        public int BarrelObjectNetworkID;
        public int GetNetworkID()
        {
            return BarrelObjectNetworkID;
        }
        public AIMinionClient GetBarrel()
        {
            return ObjectManager.GetUnitByNetworkId<AIMinionClient>(BarrelObjectNetworkID);
        }
        public bool CanAANow()
        {
            //Console.WriteLine();
            return Environment.TickCount >= BarrelAttackTick - Program.Player.AttackCastDelay * 1000;
        }

        public bool CanQNow(int delay = 0)
        {
            if (Program.Player.Distance(GetBarrel().Position)<=625 && Helper.GetQTime(GetBarrel().Position) + delay + Environment.TickCount >= BarrelAttackTick + Config.Menu["Miscellanious"]["misc.additionalServerTick"].GetValue<MenuSlider>().Value)
            {
                return true;
            }
            return false;
        }

        public Barrel(AIMinionClient barrel)
        {
            BarrelObjectNetworkID = (int)barrel.NetworkId;
            BarrelAttackTick = GetBarrelAttackTick();
        }

        public void ReduceBarrelAttackTick()
        {
            if (Program.Player.Level < 7) BarrelAttackTick -= 2000;
            else if (Program.Player.Level < 13) BarrelAttackTick -= 1000;
            else BarrelAttackTick -= 500;
        }
        private static int GetBarrelAttackTick()
        {
            if (Program.Player.Level < 7) return Environment.TickCount + 4000;
            if (Program.Player.Level < 13) return Environment.TickCount + 2000;
            return Environment.TickCount + 1000;
        }      
    }
}
