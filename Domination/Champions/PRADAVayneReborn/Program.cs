using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using PRADA_Vayne.MyInitializer;

namespace PRADA_Vayne
{
    public class Program
    {
        public static void VayneMain()
        {
            GameEvent.OnGameLoad += GameEvent_OnGameEnd;
            //GameEvent.OnGameLoad += arg => PRADALoader.Init();
        }

        private static void GameEvent_OnGameEnd()
        {
            PRADALoader.Init();
        }

        #region Menu

        public static Menu MainMenu;
        public static Menu ComboMenu;
        public static Menu LaneClearMenu;
        public static Menu EscapeMenu;
        public static Menu DrawingsMenu;
        public static Menu SkinhackMenu;
        public static Menu OrbwalkerMenu;

        #endregion Menu

        #region Spells

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        #endregion Spells
    }
}