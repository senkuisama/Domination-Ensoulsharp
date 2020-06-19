using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using Troll_Chat_xD.Event;
using Troll_Chat_xD.Menu;

namespace Troll_Chat_xD.Core
{
    public class Troll
    {
        public static void OnLoad()
        {
            MenuManager.LoadMenu();
            EventManager.LoadEvents();
        }
    }
}
