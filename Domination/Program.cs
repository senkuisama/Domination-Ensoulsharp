using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using DominationAIO.Champions;

namespace DominationAIO
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnLoadingComplete;
        }
        private static void OnLoadingComplete()
        {
            try
            {
                Hacks.DisableAntiDisconnect = true;
                if (Hacks.DisableAntiDisconnect == false) Hacks.DisableAntiDisconnect = true;
                switch (GameObjects.Player.CharacterName)
                {
                    case "Aphelios":
                        Champions.Aphelios.loaded.OnLoad();
                        Game.Print("<font color='#b756c5' size='25'>" + Game.BuildDate + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                        break;
                    case "Yasuo":
                        //ProdragonYasuo.loaded();
                        Yasuo.Yasuo.OnLoad();
                        Game.Print("<font color='#b756c5' size='25'>" + Game.BuildDate + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                        break;
                    case "Irelia":
                        Template.loaded.OnLoad();
                        Game.Print(Game.BuildDate);

                        break;                                 
                    case "Riven":
                        Rupdate.OnLoaded();
                        Game.Print("<font color='#b756c5' size='25'>" + Game.BuildDate + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                        break;
                    case "Vayne":
                        PRADA_Vayne.Program.VayneMain();
                        Game.Print("<font color='#b756c5' size='25'>" + Game.BuildDate + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                        break;
                    case "Kaisa":
                        Kaisa.ongameload();
                        Game.Print("<font color='#b756c5' size='25'>" + Game.BuildDate + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                        break;
                    case "Gangplank":
                        e.Motion_Gangplank.Program.Game_OnGameLoad();
                        Game.Print("<font color='#b756c5' size='25'>" + Game.BuildDate + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                        break;
                    case "Sion":
                        Sion.SionLoad();
                        Game.Print("<font color='#b756c5' size='25'>" + Game.BuildDate + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                        break;
                    default:
                        Game.Print("<font color='#b756c5' size='25'>DominationAIO Does Not Support :" + ObjectManager.Player.CharacterName+ "</font>");
                        Console.WriteLine("DominationAIO Does Not Support " + ObjectManager.Player.CharacterName);
                        break;                   
                }
                skinhack.OnLoad(); 
                Troll_Chat_xD.Program.OnLoad();
            }
            catch (Exception ex)
            {
                Game.Print("Error in loading");
                Console.WriteLine("Error in loading :");
                Console.WriteLine(ex);
            }
        }
    }
}
