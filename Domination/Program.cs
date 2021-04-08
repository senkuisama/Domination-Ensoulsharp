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
        private static void OnLoadingComplete()
        {            
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
                        case "Kayle":
                            new DaoHungAIO.Champions.Kayle();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Sylas":
                            MySylas.LoadSylas();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "LeeSin":
                            MyLee.MyLeeLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Aphelios":
                            Champions.Aphelios.loaded.OnLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Velkoz":
                            NewPlugins.MyVelKoz.VelkozLoad();

                            break;
                        case "Viego":
                            NewPlugins.MyViego.ViegoLoad();
                            break;
                        case "Jinx":
                            MyJinx.LoadJinx();
                            break;
                        case "Fiora":
                            new DaoHungAIO.Champions.Fiora();
                            break;
                        case "Xerath":
                            NewPlugins.MyXerath.XerathLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");
                            break;
                        case "Yasuo":
                            NewPlugins.Yasuo.MyYS.YasuoLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Katarina":
                            NewPlugins.Katarina.MyKatarina.LoadKata();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Lucian":
                            //URF_Lucian.LoadLucian();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        /*case "TahmKench":
                            TahmKench.Load();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.BuildDate + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;*/
                        case "Qiyana":
                            //Qiyana.Load();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Blitzcrank":
                            Blit.Load();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Zoe":
                            Zoe.Load();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Samira":
                            Samira.SamiraLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "MasterYi":
                            MasterYi.YiLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Brand":
                            Champions.Brand.BrandLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        /*case "Yasuo":
                            //ProdragonYasuo.loaded();
                            //Yasuo.Yasuo.OnLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;*/
                        case "Irelia":
                            NewPlugins.Irelia.NewIre();
                            Game.Print(Game.Version);

                            break;
                        case "Riven":
                            //Rupdate.OnLoaded();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Vayne":
                            //PRADA_Vayne.Program.VayneMain();
                            NewPlugins.MyVayne.MyVayneLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Kaisa":
                            Kaisa.ongameload();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Gangplank":
                            DaoHungAIO.Champions.Gangplank.BadaoGangplank.BadaoActivate();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Sion":
                            Sion.SionLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Akali":
                            Akali.OnLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Ezreal":
                            Ezreal.Ezreal_Load();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Pyke":
                            Pyke_Ryū.Program.GameEvent_OnGameLoad();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        case "Rengar":
                            //Rengar.RengarLoader();
                            Game.Print("<font color='#b756c5' size='25'>" + Game.Version + "</font>: DominationAIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00' size='25'>by ProDragon</font>");

                            break;
                        default:
                            Game.Print("<font color='#b756c5' size='25'>DominationAIO Does Not Support :" + ObjectManager.Player.CharacterName + "</font>");
                            Console.WriteLine("DominationAIO Does Not Support " + ObjectManager.Player.CharacterName);
                            break;
                    }
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
                        "Jinx"
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


            Drawing.OnDraw += Drawing_OnEndScene;
        }

        private static Menu programmenu = null;
        private static MenuBool LoadTracker = new MenuBool("Tracker", "Tracker Load");
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


            programmenu.Attach();
        }

        private static Dictionary<int, Vector3> SaveLastPoint = new Dictionary<int, Vector3>();

        private static void Drawing_OnEndScene(EventArgs args)
        {

            if (!LoadTracker.Enabled)
                return;
            /*foreach (var target in ObjectManager.Get<AIHeroClient>().Where(i => !i.IsAlly && i.IsValidTarget()))
            {
                if (SaveLastPoint.ContainsKey(target.NetworkId))
                {
                    SaveLastPoint[target.NetworkId] = target.Position;
                }
                else
                {
                    SaveLastPoint.Add(target.NetworkId, target.Position);
                }
            }*/
            foreach(var target in ObjectManager.Get<AIHeroClient>().Where(i => !i.IsAlly))
            {
                //ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);

                if (!target.Position.IsZero && !target.CharacterName.ToString().Contains("PracticeTool") && target.Position.DistanceToPlayer() <= 20000)
                {
                    if (target.Position.IsOnScreen())
                    {
                        var pos = Drawing.WorldToScreen(target.Position);
                        if (!pos.IsZero && pos.DistanceToPlayer() <= 20000)
                        {
                            Drawing.DrawText(pos.X, pos.Y, System.Drawing.Color.Yellow, target.CharacterName);
                            Render.Circle.DrawCircle(target.Position, 50, System.Drawing.Color.Red);
                        }
                    }

                    //Drawing.DrawLine(ObjectManager.Player.Position.ToVector2(), target.Position.ToVector2(), 1, System.Drawing.Color.Red);

                    if(target.Position.DistanceToPlayer() <= 300)
                    {
                        var line = new Geometry.Line(ObjectManager.Player.Position, target.Position);
                        line.Draw(System.Drawing.Color.Red);
                    }
                    
                    /*Geometry.Line getline;
                    if (SaveLastPoint.ContainsKey(target.NetworkId) && !SaveLastPoint[target.NetworkId].IsZero)
                        if (SaveLastPoint[target.NetworkId] != Vector3.Zero && SaveLastPoint[target.NetworkId] != target.Position)
                        {
                            getline = new Geometry.Line(target.Position, SaveLastPoint[target.NetworkId]);
                            Render.Circle.DrawCircle(target.Position, 50, System.Drawing.Color.Green);
                        }

                    */
                }                                              
            }
        }
    }   
}
