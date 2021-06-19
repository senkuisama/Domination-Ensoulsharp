using System;
using System.Net;
using System.Diagnostics;
using EnsoulSharp;
using EnsoulSharp.SDK;
using DominationAIO.Champions;
using System.Threading.Tasks;
using EnsoulSharp.SDK.MenuUI;
using FunnySlayerCommon;
using DominationAIO.NewPlugins;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Resources;
using EnsoulSharp.SDK.Utility;

namespace DominationAIO
{
    public class Program
    {
        /*public async Task Updater()
        {
            var client = new WebClient();
            try
            {
                await client.DownloadFileTaskAsync("", "");
                Console.WriteLine("Downloaded");                
            }
            catch (Exception)
            {
                Console.WriteLine("Error When Downloading");
            }
        }*/

        public static void Main(string[] args)
        {            
            GameEvent.OnGameLoad += OnLoadingComplete;
        }

        private static List<TrackerHelper> TrackerHelp = new List<TrackerHelper>();

        public static bool ChampLoaded = false;
        public static string ImagesFolderPath = "";
        public static Bitmap myimages = null;
        public static string pathfound = "";
        private static void OnLoadingComplete()
        {                      
            try
            {
                var process = Process.GetProcessesByName("Loader").FirstOrDefault();

                if (process != null)
                {
                    Console.WriteLine("________");
                    Console.WriteLine(process.MainModule.FileName);
                    Console.WriteLine("________");
                    pathfound = process.MainModule.FileName;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            ImagesFolderPath = Path.GetDirectoryName(pathfound) + @"\Cache\Images\Champions";

            Console.WriteLine("________");
            Console.WriteLine(ImagesFolderPath);
            Console.WriteLine(EnsoulSharp.SDK.Core.Config.ImageFolder.FullName);
            Console.WriteLine("________");

            if (ObjectManager.Player.CharacterName != "Yasuo")
                myimages = TrackerHelper.Load(ObjectManager.Player.NetworkId);
            else
                myimages = TrackerHelper.resizeImage(new Bitmap(Properties.Resources.LoadBitmap), false, 100);
            Drawing.OnEndScene += Drawing_OnEndScene1;
            DelayAction.Add(7000, () => {
                Drawing.OnEndScene -= Drawing_OnEndScene1;
            });

            if (ObjectManager.Player == null)
                return;
            FSpred.Prediction.Prediction.Initialize();
            FunnySlayerCommon.OnAction.CheckOnAction();

            ProgramLoad();

            /*try
            {
                new Program().Updater().Wait();
            }
            catch (Exception)
            {
                Console.WriteLine("Error When Updating");
            }

            try
            {
                Process.Start("");
            }
            catch (Exception)
            {
                Console.WriteLine("Error When Starting");
            }*/

            try
            {   
                //ObjectManager.Player.Name = "Riot Games";
                /*Hacks.DisableAntiDisconnect = false;
                if (Hacks.DisableAntiDisconnect == true) Hacks.DisableAntiDisconnect = false;*/
                if(LoadChamps.Enabled)
                {
                    switch (GameObjects.Player.CharacterName)
                    {
                        case "Anivia":
                            new xSaliceResurrected_Rework.Pluging.Anivia();
                            SendLoadChamp();
                            break;
                        case "Jayce":
                            DaoHungAIO.Champions.Jayce.JayceLoad();
                            SendLoadChamp();
                            break;
                        case "Lucian":
                            Luian.URF_Lucian.LoadLucian();
                            SendLoadChamp();
                            break;
                        case "Viktor":
                            DaoHungAIO.Champions.Viktor.LoadViktor();
                            SendLoadChamp();
                            break;
                        case "Gwen":
                            MyGwen.GwenLoad();
                            SendLoadChamp();
                            break;
                        case "Yone":
                            MyYone.YoneLoad();
                            SendLoadChamp();
                            break;
                        case "Kayle":
                            new DaoHungAIO.Champions.Kayle();
                            SendLoadChamp();
                            break;
                        case "Sylas":
                            MySylas.LoadSylas();
                            SendLoadChamp();
                            break;
                        case "LeeSin":
                            MyLee.MyLeeLoad();
                            SendLoadChamp();
                            break;
                        case "Aphelios":
                            Champions.Aphelios.loaded.OnLoad();
                            SendLoadChamp();
                            break;
                        case "Velkoz":
                            NewPlugins.MyVelKoz.VelkozLoad();
                            SendLoadChamp();
                            break;
                        case "Viego":
                            NewPlugins.MyViego.ViegoLoad();
                            SendLoadChamp();
                            break;
                        case "Jinx":
                            MyJinx.LoadJinx();
                            SendLoadChamp();
                            break;
                        case "Fiora":
                            new DaoHungAIO.Champions.Fiora();
                            SendLoadChamp();
                            break;
                        case "Xerath":
                            NewPlugins.MyXerath.XerathLoad();
                            SendLoadChamp();
                            break;
                        case "Yasuo":
                            NewPlugins.Yasuo.MyYS.YasuoLoad();
                            SendLoadChamp();
                            break;
                        case "Katarina":
                            NewPlugins.Katarina.MyKatarina.LoadKata();
                            SendLoadChamp();
                            break;
                        case "Blitzcrank":
                            Blit.Load();
                            SendLoadChamp();
                            break;
                        case "Zoe":
                            Zoe.Load();
                            SendLoadChamp();
                            break;
                        case "Samira":
                            Samira.SamiraLoad();
                            SendLoadChamp();
                            break;
                        case "MasterYi":
                            MasterYi.YiLoad();
                            SendLoadChamp();
                            break;
                        case "Brand":
                            Champions.Brand.BrandLoad();
                            SendLoadChamp();
                            break;
                        case "Irelia":
                            NewPlugins.Irelia.NewIre();
                            SendLoadChamp();
                            break;
                        case "Riven":
                            MyRiven.LoadRiven();
                            SendLoadChamp();
                            break;
                        case "Vayne":
                            NewPlugins.MyVayne.MyVayneLoad();
                            SendLoadChamp();
                            break;
                        case "Kaisa":
                            Kaisa.ongameload();
                            SendLoadChamp();
                            break;
                        case "Gangplank":
                            DaoHungAIO.Champions.Gangplank.BadaoGangplank.BadaoActivate();
                            SendLoadChamp();
                            break;
                        case "Sion":
                            Sion.SionLoad();
                            SendLoadChamp();
                            break;
                        case "Akali":
                            Akali.OnLoad();
                            SendLoadChamp();
                            break;
                        case "Ezreal":
                            Ezreal.Ezreal_Load();
                            SendLoadChamp();
                            break;
                        case "Pyke":
                            Pyke_Ryū.Program.GameEvent_OnGameLoad();
                            SendLoadChamp();
                            break;
                        case "Rengar":
                            SendLoadChamp();
                            break;
                        default:
                            Game.Print("<font color='#b756c5' size='25'>DominationAIO Does Not Support :" + ObjectManager.Player.CharacterName + "</font>");
                            Console.WriteLine("DominationAIO Does Not Support " + ObjectManager.Player.CharacterName);
                            break;
                    }

                    ChampLoaded = true;
                }

                if (LoadSkin.Enabled)
                {
                    skinhack.OnLoad();
                }

                if (LoadBaseUlt.Enabled)
                {
                    var listbaseultsupported = new List<string>()
                    {
                        "Draven",
                        "Ezreal",
                        "Ashe",
                        "Jinx",
                        "Senna"
                    };

                    if (listbaseultsupported.Contains(ObjectManager.Player.CharacterName))
                        MyBaseUlt.LoadBaseUlt();
                }               
            }
            catch (Exception ex)
            {
                Game.Print("Error in loading");
                Console.WriteLine("Error in loading :");
                Console.WriteLine(ex);
            }

           
            //Load Tracker
            if (LoadTracker.Enabled)
            {
                foreach (var item in GameObjects.EnemyHeroes.Where(i => i.CharacterName != "PracticeTool_TargetDummy"))
                {
                    var target = item;
                    new TrackerHelper(target);
                }
            }
        }

        private static void SendLoadChamp()
        {
            Game.Print("<font color='#b756c5' size='35'>DominationAIO " + Game.Version + "</font> :" + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by FunnySlayer</font>");
        }

        private static void Drawing_OnEndScene1(EventArgs args)
        {
            if(ObjectManager.Player.CharacterName == "Yasuo")
            {
                var pos = new Vector2((Drawing.Width / 2) - 256, (Drawing.Height / 2) - 256);
                using (var r = new Render.Sprite(myimages, pos))
                {
                    r.Draw();
                }
            }
            else
            {
                var pos = new Vector2((Drawing.Width / 2) - 270, (Drawing.Height / 2) - 270);
                using (var r = new Render.Sprite(myimages, pos))
                {
                    r.Draw();
                }

                var newpos = new Vector2((Drawing.Width / 2) - 300, (Drawing.Height / 2) - 300 - 65);
                var Qspell = TrackerHelper.SpellLoad(ObjectManager.Player.GetSpell(SpellSlot.Q).Name, false, 100);
                var Wspell = TrackerHelper.SpellLoad(ObjectManager.Player.GetSpell(SpellSlot.W).Name, false, 100);
                var Espell = TrackerHelper.SpellLoad(ObjectManager.Player.GetSpell(SpellSlot.E).Name, false, 100);
                var Rspell = TrackerHelper.SpellLoad(ObjectManager.Player.GetSpell(SpellSlot.R).Name, false, 100);
                newpos.X += 68.8f;
                using (var r = new Render.Sprite(Qspell, newpos))
                {
                    r.Draw();
                }
                newpos.X += 68.8f + 64f;
                using (var r = new Render.Sprite(Wspell, newpos))
                {
                    r.Draw();
                }
                newpos.X += 68.8f + 64f;
                using (var r = new Render.Sprite(Espell, newpos))
                {
                    r.Draw();
                }
                newpos.X += 68.8f + 64f;
                using (var r = new Render.Sprite(Rspell, newpos))
                {
                    r.Draw();
                }
            }
        }

        private static Menu programmenu = null;
        public static MenuBool LoadTracker = new MenuBool("Tracker", "Tracker Load");
        private static MenuBool LoadBaseUlt = new MenuBool("BaseUlt", "Base Ult Load");
        private static MenuBool LoadSkin = new MenuBool("Skin", "Skin Load");
        private static MenuBool LoadChamps = new MenuBool("Champs", "Champions Load");
        private static MenuSeparator F5 = new MenuSeparator("F5", "F5 if change");
        private static void ProgramLoad()
        {
            programmenu = new Menu("Domination", "DominationAIO", true);
            programmenu.Add(LoadTracker);
            programmenu.Add(LoadBaseUlt);
            programmenu.Add(LoadChamps);
            programmenu.Add(F5);


            programmenu.AddTargetSelectorMenu();

            programmenu.Attach();

        }

        private static Dictionary<int, Vector3> SaveLastPoint = new Dictionary<int, Vector3>();
    }   
}
