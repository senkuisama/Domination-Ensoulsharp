using EnsoulSharp;
using PRADA_Vayne.MyLogic.Others;
using PRADA_Vayne.MyUtils;

namespace PRADA_Vayne.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void Init()
        {
            Game.Print("<font color = '#b756c5' size = '25' >VayBu Loaded Good Luck</font>");
            Cache.Load();
            LoadMenu();
            LoadSpells();
            LoadLogic();
            ShowNotifications();
            SkinHack.Load();
        }
    }
}