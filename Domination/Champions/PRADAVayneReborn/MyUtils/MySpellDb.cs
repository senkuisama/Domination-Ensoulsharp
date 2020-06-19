/*
*     ___ _____ _   _   ___ ______   _  _
*    |_  |_   _| | | | / _ \|  _  \_| || |_
*      | | | | | |_| |/ /_\ \ | | |_  __  _|
*      | | | | |  _  ||  _  | | | |_| || |_
*  /\__/ /_| |_| | | || | | | |/ /|_  __  _|
*  \____/ \___/\_| |_/\_| |_/___/   |_||_|
*
*  "Very sharp, this assembly is the bomb!"
*                                     -ISIS
*     Greetings to our brothers in arms:
*
*                  Gucci
*   (without him there would be no jihad.)
*                Joduskame
*   (without him there would be no sharp.)
*
*  h3h3, Trees, L33T, blacky, zezzy, xQx,
*  iJava, ChewyMoon, Kurisu, Asuna, iMeh,
*  Sebby, Beaving, xcsoft, Sida, jQuery,
*  legacy, xQx, Kortatu, OutrageousMe
*        and everyone using this
*
*/

using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

/* This database of targeted spells is actually c+p from h3h3
 * and is used under the terms of GNU's GPL.
 * You can grab a copy of GPL here: http://www.gnu.org/licenses/
 */

namespace PRADA_Vayne.Utils
{
    public static class SpellDb
    {
        public static List<TargetSpellData> Spells;

        static SpellDb()
        {
            Spells = new List<TargetSpellData>
            {
                #region Aatrox

                new TargetSpellData("aatrox", "aatroxq", SpellSlot.Q, SpellType.Skillshot, CcType.Knockup, 650, 600,
                    0, true),
                new TargetSpellData("aatrox", "aatroxq2", SpellSlot.Q, SpellType.Skillshot, CcType.Knockup, 525, 600,
                    0, true),
                new TargetSpellData("aatrox", "aatroxq3", SpellSlot.Q, SpellType.Skillshot, CcType.Knockup, 200, 600,
                    0, true),
                new TargetSpellData("aatrox", "aatroxw", SpellSlot.W, SpellType.Skillshot, CcType.Knockup, 825, 250,
                    1800),
                new TargetSpellData("aatrox", "aatroxe", SpellSlot.E, SpellType.Skillshot, CcType.No, 300, 250,
                    800),
                new TargetSpellData("aatrox", "aatroxr", SpellSlot.R, SpellType.Self, CcType.No, 600, 250,
                    0, false, false, true),

                #endregion Aatrox

                #region Ahri

                new TargetSpellData("ahri", "ahriorbofdeception", SpellSlot.Q, SpellType.Skillshot, CcType.No, 100, 250,
                    1450),
                new TargetSpellData("ahri", "ahrifoxfire", SpellSlot.W, SpellType.Self, CcType.No, 800, 0, 1800),
                new TargetSpellData("ahri", "ahriseduce", SpellSlot.E, SpellType.Skillshot, CcType.Charm, 1000, 250,
                    1550, true),
                new TargetSpellData("ahri", "ahritumble", SpellSlot.R, SpellType.Skillshot, CcType.No, 450, 250, 2200,
                    false, false, true),

                #endregion Ahri

                #region Akali

                new TargetSpellData("akali", "akaliq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 500, 0, 0,
                    true),
                new TargetSpellData("akali", "akaliw", SpellSlot.W, SpellType.Skillshot, CcType.No, 250, 0,
                    int.MaxValue, false, true),
                new TargetSpellData("akali", "akalie", SpellSlot.E, SpellType.Skillshot, CcType.No, 650, 0,
                    0),
                new TargetSpellData("akali", "akalie2", SpellSlot.E, SpellType.Skillshot, CcType.No, int.MaxValue, 0,
                    0),
                new TargetSpellData("akali", "akalir", SpellSlot.R, SpellType.Skillshot, CcType.No, 575, 0,
                    0),
                new TargetSpellData("akali", "akalir2", SpellSlot.R, SpellType.Skillshot, CcType.No, 750, 0,
                    0),

                #endregion Akali

                #region Alistar

                new TargetSpellData("alistar", "pulverize", SpellSlot.Q, SpellType.Self, CcType.Knockup, 365, 0, 20,
                    true),
                new TargetSpellData("alistar", "headbutt", SpellSlot.W, SpellType.Targeted, CcType.Knockback, 650, 200,
                    0),
                new TargetSpellData("alistar", "triumphantroar", SpellSlot.E, SpellType.Self, CcType.No, 575, 0, 0),
                new TargetSpellData("alistar", "feroucioushowl", SpellSlot.R, SpellType.Self, CcType.No, 0, 0, 828,
                    false, false, true),

                #endregion Alistar

                #region Amumu

                new TargetSpellData("amumu", "bandagetoss", SpellSlot.Q, SpellType.Skillshot, CcType.Stun, 1100, 250,
                    2000, true),
                new TargetSpellData("amumu", "auraofdespair", SpellSlot.W, SpellType.Self, CcType.No, 300, 470,
                    int.MaxValue),
                new TargetSpellData("amumu", "tantrum", SpellSlot.E, SpellType.Self, CcType.No, 350, 150, int.MaxValue),
                new TargetSpellData("amumu", "curseofthesadmummy", SpellSlot.R, SpellType.Self, CcType.Stun, 550, 150,
                    int.MaxValue, true),

                #endregion Amumu

                #region Anivia

                new TargetSpellData("anivia", "flashfrost", SpellSlot.Q, SpellType.Skillshot, CcType.Stun, 1200, 500,
                    850),
                new TargetSpellData("anivia", "crystalize", SpellSlot.W, SpellType.Skillshot, CcType.No, 1000, 500,
                    1600),
                new TargetSpellData("anivia", "frostbite", SpellSlot.E, SpellType.Targeted, CcType.No, 650, 250, 1450),
                new TargetSpellData("anivia", "glacialstorm", SpellSlot.R, SpellType.Skillshot, CcType.Slow, 675, 300,
                    250, false, false, true),

                #endregion Anivia

                #region Annie

                new TargetSpellData("annie", "disintegrate", SpellSlot.Q, SpellType.Targeted, CcType.No, 623, 500,
                    1400),
                new TargetSpellData("annie", "incinerate", SpellSlot.W, SpellType.Targeted, CcType.No, 825, 250,
                    int.MaxValue),
                new TargetSpellData("annie", "moltenshield", SpellSlot.E, SpellType.Self, CcType.No, 100, 0,
                    int.MaxValue),
                new TargetSpellData("annie", "infernalguardian", SpellSlot.R, SpellType.Skillshot, CcType.No, 600, 150,
                    int.MaxValue, true),

                #endregion Annie

                #region Ashe

                new TargetSpellData("ashe", "frostshot", SpellSlot.Q, SpellType.Self, CcType.No, 0, 0, int.MaxValue),
                new TargetSpellData("ashe", "frostarrow", SpellSlot.Q, SpellType.Targeted, CcType.Slow, 0, 0,
                    int.MaxValue),
                new TargetSpellData("ashe", "volley", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 1200, 250, 1500),
                new TargetSpellData("ashe", "ashespiritofthehawk", SpellSlot.E, SpellType.Skillshot, CcType.No, 2500,
                    500, 1400),
                new TargetSpellData("ashe", "enchantedcrystalarrow", SpellSlot.R, SpellType.Skillshot, CcType.Stun,
                    20000, 250, 1600, true),

                #endregion Ashe

                #region Bard

                new TargetSpellData("bard", "bardq", SpellSlot.Q, SpellType.Skillshot, CcType.Stun, 950, 250, 1600),
                new TargetSpellData("bard", "bardw", SpellSlot.W, SpellType.Skillshot, CcType.No, 1000, 250, 1450),
                new TargetSpellData("bard", "barde", SpellSlot.E, SpellType.Skillshot, CcType.No, 900, 350, 1600),
                new TargetSpellData("bard", "bardr", SpellSlot.R, SpellType.Skillshot, CcType.Stun, 3400, 300, 2100),

                #endregion Bard

                #region Blitzcrank

                new TargetSpellData("blitzcrank", "rocketgrabmissile", SpellSlot.Q, SpellType.Skillshot, CcType.Pull,
                    1150, 250, 1800),
                new TargetSpellData("blitzcrank", "overdrive", SpellSlot.W, SpellType.Self, CcType.No, 0, 0, 0),
                new TargetSpellData("blitzcrank", "powerfist", SpellSlot.E, SpellType.Self, CcType.Knockup, 100, 150,
                    int.MaxValue),
                new TargetSpellData("blitzcrank", "staticfield", SpellSlot.R, SpellType.Self, CcType.Silence, 600, 150,
                    int.MaxValue, true),

                #endregion Blitzcrank

                #region Brand

                new TargetSpellData("brand", "brandblaze", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1150, 500, 1200,
                    true),
                new TargetSpellData("brand", "brandfissure", SpellSlot.W, SpellType.Skillshot, CcType.No, 240, 550, 20,
                    true),
                new TargetSpellData("brand", "brandconflagration", SpellSlot.E, SpellType.Targeted, CcType.No, 625, 0,
                    1800),
                new TargetSpellData("brand", "brandwildfire", SpellSlot.R, SpellType.Targeted, CcType.No, 0, 0, 1000,
                    true),

                #endregion Brand

                #region Braum

                new TargetSpellData("braum", "braumq", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 1100, 500, 1200,
                    true),
                new TargetSpellData("braum", "braumqmissle", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 1100, 500,
                    1200),
                new TargetSpellData("braum", "braumw", SpellSlot.W, SpellType.Targeted, CcType.No, 650, 500, 1500),
                new TargetSpellData("braum", "braume", SpellSlot.E, SpellType.Skillshot, CcType.No, 250, 0,
                    int.MaxValue),
                new TargetSpellData("braum", "braumrwrapper", SpellSlot.R, SpellType.Skillshot, CcType.Knockup, 1250, 0,
                    1200, true),

                #endregion Braum

                #region Caitlyn

                new TargetSpellData("caitlyn", "caitlynpiltoverpeacemaker", SpellSlot.Q, SpellType.Skillshot, CcType.No,
                    2000, 250, 2200),
                new TargetSpellData("caitlyn", "caitlynyordletrap", SpellSlot.W, SpellType.Skillshot, CcType.Snare, 800,
                    0, 1400),
                new TargetSpellData("caitlyn", "caitlynentrapment", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 950,
                    250, 2000),
                new TargetSpellData("caitlyn", "caitlynaceinthehole", SpellSlot.R, SpellType.Targeted, CcType.No, 2500,
                    0, 1500, true, false, true),

                #endregion Caitlyn

                #region Cassiopeia

                new TargetSpellData("cassiopeia", "cassiopeianoxiousblast", SpellSlot.Q, SpellType.Skillshot, CcType.No,
                    925, 250, int.MaxValue),
                new TargetSpellData("cassiopeia", "cassiopeiamiasma", SpellSlot.W, SpellType.Skillshot, CcType.Slow,
                    925, 500, 2500),
                new TargetSpellData("cassiopeia", "cassiopeiatwinfang", SpellSlot.E, SpellType.Targeted, CcType.No, 700,
                    0, 1900),
                new TargetSpellData("cassiopeia", "cassiopeiapetrifyinggaze", SpellSlot.R, SpellType.Skillshot,
                    CcType.Stun, 875, 250, int.MaxValue, true),

                #endregion Cassiopeia

                #region Chogath

                new TargetSpellData("chogath", "rupture", SpellSlot.Q, SpellType.Skillshot, CcType.Knockup, 950, 1000,
                    int.MaxValue, true),
                new TargetSpellData("chogath", "feralscream", SpellSlot.W, SpellType.Skillshot, CcType.Silence, 675,
                    250, int.MaxValue),
                new TargetSpellData("chogath", "vorpalspikes", SpellSlot.E, SpellType.Self, CcType.No, 0, 0, 347),
                new TargetSpellData("chogath", "feast", SpellSlot.R, SpellType.Targeted, CcType.No, 230, 0,
                    int.MaxValue, true),

                #endregion Chogath

                #region Corki

                new TargetSpellData("corki", "phosphorusbomb", SpellSlot.Q, SpellType.Skillshot, CcType.No, 875, 500,
                    1000, true),
                new TargetSpellData("corki", "carpetbomb", SpellSlot.W, SpellType.Skillshot, CcType.No, 875, 0, 700),
                new TargetSpellData("corki", "ggun", SpellSlot.E, SpellType.Skillshot, CcType.No, 750, 250,
                    int.MaxValue),
                new TargetSpellData("corki", "missilebarrage", SpellSlot.R, SpellType.Skillshot, CcType.No, 1225, 250,
                    828, false, false, true),

                #endregion Corki

                #region Darius

                new TargetSpellData("darius", "dariuscleave", SpellSlot.Q, SpellType.Self, CcType.No, 425, 0,
                    int.MaxValue),
                new TargetSpellData("darius", "dariusnoxiantacticsonh", SpellSlot.W, SpellType.Self, CcType.Slow, 210,
                    0, int.MaxValue),
                new TargetSpellData("darius", "dariusaxegrabcone", SpellSlot.E, SpellType.Skillshot, CcType.Pull, 550,
                    150, int.MaxValue),
                new TargetSpellData("darius", "dariusexecute", SpellSlot.R, SpellType.Targeted, CcType.No, 460, 200,
                    int.MaxValue, true),

                #endregion Darius

                #region Diana

                new TargetSpellData("diana", "dianaarc", SpellSlot.Q, SpellType.Skillshot, CcType.No, 900, 300, 1500),
                new TargetSpellData("diana", "dianaorbs", SpellSlot.W, SpellType.Self, CcType.No, 0, 0, 0),
                new TargetSpellData("diana", "dianavortex", SpellSlot.E, SpellType.Self, CcType.Pull, 300, 250, 1500),
                new TargetSpellData("diana", "dianateleport", SpellSlot.R, SpellType.Targeted, CcType.No, 800, 250,
                    1500, true),

                #endregion Diana

                #region Draven

                new TargetSpellData("draven", "dravenspinning", SpellSlot.Q, SpellType.Self, CcType.No, 0, int.MaxValue,
                    int.MaxValue),
                new TargetSpellData("draven", "dravenfury", SpellSlot.W, SpellType.Self, CcType.No, 0, int.MaxValue,
                    int.MaxValue),
                new TargetSpellData("draven", "dravendoubleshot", SpellSlot.E, SpellType.Skillshot, CcType.Knockback,
                    1050, 500, 1600),
                new TargetSpellData("draven", "dravenrcast", SpellSlot.R, SpellType.Skillshot, CcType.No, 20000, 500,
                    2000, true),

                #endregion Draven

                #region DrMundo

                new TargetSpellData("drmundo", "infectedcleavermissilecast", SpellSlot.Q, SpellType.Skillshot,
                    CcType.Slow, 1000, 500, 1500, true),
                new TargetSpellData("drmundo", "burningagony", SpellSlot.W, SpellType.Self, CcType.No, 225,
                    int.MaxValue, int.MaxValue),
                new TargetSpellData("drmundo", "masochism", SpellSlot.E, SpellType.Self, CcType.No, 0, int.MaxValue,
                    int.MaxValue),
                new TargetSpellData("drmundo", "sadism", SpellSlot.R, SpellType.Self, CcType.No, 0, int.MaxValue,
                    int.MaxValue, false, false, true),

                #endregion DrMundo

                #region Elise

                new TargetSpellData("elise", "elisehumanq", SpellSlot.Q, SpellType.Targeted, CcType.No, 625, 550, 2200),
                new TargetSpellData("elise", "elisespiderqcast", SpellSlot.Q, SpellType.Targeted, CcType.No, 375, 500,
                    int.MaxValue),
                new TargetSpellData("elise", "elisehumanw", SpellSlot.W, SpellType.Skillshot, CcType.No, 950, 750,
                    5000),
                new TargetSpellData("elise", "elisespiderw", SpellSlot.W, SpellType.Self, CcType.No, 0, int.MaxValue,
                    int.MaxValue),
                new TargetSpellData("elise", "elisehumane", SpellSlot.E, SpellType.Skillshot, CcType.Stun, 1075, 500,
                    1600),
                new TargetSpellData("elise", "elisespidereinitial", SpellSlot.E, SpellType.Targeted, CcType.No, 975,
                    int.MaxValue, int.MaxValue),
                new TargetSpellData("elise", "elisespideredescent", SpellSlot.E, SpellType.Targeted, CcType.No, 975,
                    int.MaxValue, int.MaxValue),
                new TargetSpellData("elise", "eliser", SpellSlot.R, SpellType.Self, CcType.No, 0, int.MaxValue,
                    int.MaxValue, false, false, true),
                new TargetSpellData("elise", "elisespiderr", SpellSlot.R, SpellType.Self, CcType.No, 0, int.MaxValue,
                    int.MaxValue, false, false, true),

                #endregion Elise

                #region Evelynn

                new TargetSpellData("evelynn", "evelynnq", SpellSlot.Q, SpellType.Self, CcType.No, 500, 500,
                    int.MaxValue),
                new TargetSpellData("evelynn", "evelynnw", SpellSlot.W, SpellType.Self, CcType.No, 0, int.MaxValue,
                    int.MaxValue),
                new TargetSpellData("evelynn", "evelynne", SpellSlot.E, SpellType.Targeted, CcType.No, 290, 500, 900),
                new TargetSpellData("evelynn", "evelynnr", SpellSlot.R, SpellType.Skillshot, CcType.Slow, 650, 0, 1300,
                    true),

                #endregion Evelynn

                #region Ezreal

                new TargetSpellData("ezreal", "ezrealmysticshot", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1200,
                    250, 2000),
                new TargetSpellData("ezreal", "ezrealessenceflux", SpellSlot.W, SpellType.Skillshot, CcType.No, 1050,
                    250, 1600),
                new TargetSpellData("ezreal", "ezrealessencemissle", SpellSlot.W, SpellType.Skillshot, CcType.No, 1050,
                    250, 1600),
                new TargetSpellData("ezreal", "ezrealarcaneshift", SpellSlot.E, SpellType.Targeted, CcType.No, 475, 500,
                    int.MaxValue),
                new TargetSpellData("ezreal", "ezrealtruehotbarrage", SpellSlot.R, SpellType.Skillshot, CcType.No,
                    20000, 1000, 2000, true),

                #endregion Ezreal

                #region FiddleSticks

                new TargetSpellData("fiddlesticks", "terrify", SpellSlot.Q, SpellType.Targeted, CcType.Fear, 575, 500,
                    int.MaxValue, true),
                new TargetSpellData("fiddlesticks", "drain", SpellSlot.W, SpellType.Targeted, CcType.No, 575, 500,
                    int.MaxValue),
                new TargetSpellData("fiddlesticks", "fiddlesticksdarkwind", SpellSlot.E, SpellType.Skillshot,
                    CcType.Silence, 750, 500, 1100),
                new TargetSpellData("fiddlesticks", "crowstorm", SpellSlot.R, SpellType.Targeted, CcType.No, 800, 500,
                    int.MaxValue, true, false, true),

                #endregion FiddleSticks

                #region Fiora

                new TargetSpellData("fiora", "fioraq", SpellSlot.Q, SpellType.Targeted, CcType.No, 300, 500, 2200),
                new TargetSpellData("fiora", "fiorariposte", SpellSlot.W, SpellType.Self, CcType.No, 100, 0, 0),
                new TargetSpellData("fiora", "fioraflurry", SpellSlot.E, SpellType.Self, CcType.No, 210, 0, 0),
                new TargetSpellData("fiora", "fioradance", SpellSlot.R, SpellType.Targeted, CcType.No, 210, 500, 0,
                    true),

                #endregion Fiora

                #region Fizz

                new TargetSpellData("fizz", "fizzpiercingstrike", SpellSlot.Q, SpellType.Targeted, CcType.No, 550, 500,
                    int.MaxValue),
                new TargetSpellData("fizz", "fizzseastonepassive", SpellSlot.W, SpellType.Self, CcType.No, 0, 500, 0),
                new TargetSpellData("fizz", "fizzjump", SpellSlot.E, SpellType.Self, CcType.No, 400, 500, 1300),
                new TargetSpellData("fizz", "fizzjumptwo", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 400, 500,
                    1300),
                new TargetSpellData("fizz", "fizzmarinerdoom", SpellSlot.R, SpellType.Skillshot, CcType.Knockup, 1275,
                    500, 1200, false, false, true),

                #endregion Fizz

                #region Galio

                new TargetSpellData("galio", "galioresolutesmite", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 940,
                    500, 1300),
                new TargetSpellData("galio", "galiobulwark", SpellSlot.W, SpellType.Targeted, CcType.No, 800, 500,
                    int.MaxValue),
                new TargetSpellData("galio", "galiorighteousgust", SpellSlot.E, SpellType.Skillshot, CcType.No, 1180,
                    500, 1200),
                new TargetSpellData("galio", "galioidolofdurand", SpellSlot.R, SpellType.Self, CcType.Taunt, 560, 150,
                    int.MaxValue, true),

                #endregion Galio

                #region Gangplank

                new TargetSpellData("gangplank", "parley", SpellSlot.Q, SpellType.Targeted, CcType.No, 625, 500, 2000),
                new TargetSpellData("gangplank", "removescurvy", SpellSlot.W, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("gangplank", "raisemorale", SpellSlot.E, SpellType.Self, CcType.No, 1300, 500,
                    int.MaxValue),
                new TargetSpellData("gangplank", "cannonbarrage", SpellSlot.R, SpellType.Skillshot, CcType.Slow, 20000,
                    500, 500, false, false, true),

                #endregion Gangplank

                #region Garen

                new TargetSpellData("garen", "garenq", SpellSlot.Q, SpellType.Self, CcType.No, 0, 200, int.MaxValue),
                new TargetSpellData("garen", "garenw", SpellSlot.W, SpellType.Self, CcType.No, 0, 500, int.MaxValue),
                new TargetSpellData("garen", "garene", SpellSlot.E, SpellType.Self, CcType.No, 325, 0, 700),
                new TargetSpellData("garen", "garenr", SpellSlot.R, SpellType.Targeted, CcType.No, 400, 120,
                    int.MaxValue, false, false, true),

                #endregion Garen

                #region Gragas

                new TargetSpellData("gragas", "gragasq", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 1100, 250,
                    1300),
                new TargetSpellData("gragas", "gragasqtoggle", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1100, 300,
                    1000),
                new TargetSpellData("gragas", "gragasw", SpellSlot.W, SpellType.Self, CcType.No, 0, 0, 0),
                new TargetSpellData("gragas", "gragase", SpellSlot.E, SpellType.Skillshot, CcType.Knockback, 1100, 300,
                    1000, true),
                new TargetSpellData("gragas", "gragasr", SpellSlot.R, SpellType.Skillshot, CcType.Knockback, 1100, 300,
                    1000, true),

                #endregion Gragas

                #region Graves

                new TargetSpellData("graves", "gravesclustershot", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1100,
                    300, 902),
                new TargetSpellData("graves", "gravessmokegrenade", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 1100,
                    300, 1650),
                new TargetSpellData("graves", "gravessmokegrenadeboom", SpellSlot.W, SpellType.Skillshot, CcType.Slow,
                    1100, 300, 1650),
                new TargetSpellData("graves", "gravesmove", SpellSlot.E, SpellType.Skillshot, CcType.No, 425, 300,
                    1000),
                new TargetSpellData("graves", "graveschargeshot", SpellSlot.R, SpellType.Skillshot, CcType.No, 1100,
                    250, 2100, true),

                #endregion Graves

                #region Hecarim

                new TargetSpellData("hecarim", "hecarimrapidslash", SpellSlot.Q, SpellType.Self, CcType.No, 350, 300,
                    1450),
                new TargetSpellData("hecarim", "hecarimw", SpellSlot.W, SpellType.Self, CcType.No, 525, 120, 828),
                new TargetSpellData("hecarim", "hecarimramp", SpellSlot.E, SpellType.Self, CcType.No, 0, int.MaxValue,
                    int.MaxValue),
                new TargetSpellData("hecarim", "hecarimult", SpellSlot.R, SpellType.Skillshot, CcType.Fear, 1350, 250,
                    1800, true),

                #endregion Hecarim

                #region Heimerdinger

                new TargetSpellData("heimerdinger", "heimerdingerq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 350,
                    500, int.MaxValue),
                new TargetSpellData("heimerdinger", "heimerdingerw", SpellSlot.W, SpellType.Skillshot, CcType.No, 1525,
                    500, 902),
                new TargetSpellData("heimerdinger", "heimerdingere", SpellSlot.E, SpellType.Skillshot, CcType.Stun, 970,
                    500, 2500, true),
                new TargetSpellData("heimerdinger", "heimerdingerr", SpellSlot.R, SpellType.Self, CcType.No, 0, 230,
                    int.MaxValue, false, false, true),
                new TargetSpellData("heimerdinger", "heimerdingereult", SpellSlot.E, SpellType.Skillshot, CcType.Stun,
                    970, 250, 1200),

                #endregion Heimerdinger

                #region Irelia

                new TargetSpellData("irelia", "ireliagatotsu", SpellSlot.Q, SpellType.Targeted, CcType.No, 650, 150,
                    2200),
                new TargetSpellData("irelia", "ireliahitenstyle", SpellSlot.W, SpellType.Self, CcType.No, 0, 230, 347),
                new TargetSpellData("irelia", "ireliaequilibriumstrike", SpellSlot.E, SpellType.Targeted, CcType.Stun,
                    325, 500, int.MaxValue, true),
                new TargetSpellData("irelia", "ireliatranscendentblades", SpellSlot.R, SpellType.Skillshot, CcType.No,
                    1200, 500, 779, false, false, true),

                #endregion Irelia

                #region Janna

                new TargetSpellData("janna", "howlinggale", SpellSlot.Q, SpellType.Skillshot, CcType.Knockup, 1800, 250,
                    int.MaxValue),
                new TargetSpellData("janna", "sowthewind", SpellSlot.W, SpellType.Targeted, CcType.Slow, 600, 500,
                    1600),
                new TargetSpellData("janna", "eyeofthestorm", SpellSlot.E, SpellType.Targeted, CcType.No, 800, 500,
                    int.MaxValue),
                new TargetSpellData("janna", "reapthewhirlwind", SpellSlot.R, SpellType.Self, CcType.Knockback, 725, 0,
                    828, true),

                #endregion Janna

                #region JarvanIV

                new TargetSpellData("jarvaniv", "jarvanivdragonstrike", SpellSlot.Q, SpellType.Skillshot, CcType.No,
                    700, 500, int.MaxValue),
                new TargetSpellData("jarvaniv", "jarvanivgoldenaegis", SpellSlot.W, SpellType.Self, CcType.Slow, 300,
                    500, 0),
                new TargetSpellData("jarvaniv", "jarvanivdemacianstandard", SpellSlot.E, SpellType.Skillshot, CcType.No,
                    830, 500, int.MaxValue),
                new TargetSpellData("jarvaniv", "jarvanivcataclysm", SpellSlot.R, SpellType.Skillshot, CcType.No, 650,
                    250, 0, true),

                #endregion JarvanIV

                #region Jax

                new TargetSpellData("jax", "jaxleapstrike", SpellSlot.Q, SpellType.Targeted, CcType.No, 210, 500, 0),
                new TargetSpellData("jax", "jaxempowertwo", SpellSlot.W, SpellType.Targeted, CcType.No, 0, 500, 0),
                new TargetSpellData("jax", "jaxcounterstrike", SpellSlot.E, SpellType.Self, CcType.Stun, 425, 500, 1450,
                    true),
                new TargetSpellData("jax", "jaxrelentlessasssault", SpellSlot.R, SpellType.Self, CcType.No, 0, 0, 0,
                    false, false, true),

                #endregion Jax

                #region Jayce

                new TargetSpellData("jayce", "jaycetotheskies", SpellSlot.Q, SpellType.Targeted, CcType.Slow, 600, 500,
                    int.MaxValue),
                new TargetSpellData("jayce", "jayceshockblast", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1050, 500,
                    1200),
                new TargetSpellData("jayce", "jaycestaticfield", SpellSlot.W, SpellType.Self, CcType.No, 285, 500,
                    1500),
                new TargetSpellData("jayce", "jaycehypercharge", SpellSlot.W, SpellType.Self, CcType.No, 0, 750,
                    int.MaxValue),
                new TargetSpellData("jayce", "jaycethunderingblow", SpellSlot.E, SpellType.Targeted, CcType.Knockback,
                    300, 0, int.MaxValue),
                new TargetSpellData("jayce", "jayceaccelerationgate", SpellSlot.E, SpellType.Skillshot, CcType.No, 685,
                    500, 1600),
                new TargetSpellData("jayce", "jaycestancehtg", SpellSlot.R, SpellType.Self, CcType.No, 0, 750,
                    int.MaxValue, false, false, true),
                new TargetSpellData("jayce", "jaycestancegth", SpellSlot.R, SpellType.Self, CcType.No, 0, 750,
                    int.MaxValue, false, false, true),

                #endregion Jayce

                #region Jinx

                new TargetSpellData("jinx", "jinxq", SpellSlot.Q, SpellType.Self, CcType.No, 0, 0, int.MaxValue),
                new TargetSpellData("jinx", "jinxw", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 1550, 500, 1200),
                new TargetSpellData("jinx", "jinxwmissle", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 1550, 500,
                    1200),
                new TargetSpellData("jinx", "jinxe", SpellSlot.E, SpellType.Skillshot, CcType.Snare, 900, 500, 1000),
                new TargetSpellData("jinx", "jinxr", SpellSlot.R, SpellType.Skillshot, CcType.No, 25000, 0,
                    int.MaxValue),
                new TargetSpellData("jinx", "jinxrwrapper", SpellSlot.R, SpellType.Skillshot, CcType.No, 25000, 0,
                    int.MaxValue, true),

                #endregion Jinx

                #region Karma

                new TargetSpellData("karma", "karmaq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 950, 500, 902),
                new TargetSpellData("karma", "karmaspiritbind", SpellSlot.W, SpellType.Targeted, CcType.Snare, 700, 500,
                    2000),
                new TargetSpellData("karma", "karmasolkimshield", SpellSlot.E, SpellType.Targeted, CcType.No, 800, 500,
                    int.MaxValue),
                new TargetSpellData("karma", "karmamantra", SpellSlot.R, SpellType.Self, CcType.No, 0, 500, 1300, false,
                    false, true),

                #endregion Karma

                #region Karthus

                new TargetSpellData("karthus", "laywaste", SpellSlot.Q, SpellType.Skillshot, CcType.No, 875, 500,
                    int.MaxValue),
                new TargetSpellData("karthus", "wallofpain", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 1090, 500,
                    1600),
                new TargetSpellData("karthus", "defile", SpellSlot.E, SpellType.Self, CcType.No, 550, 500, 1000),
                new TargetSpellData("karthus", "fallenone", SpellSlot.R, SpellType.Self, CcType.No, int.MaxValue, 2800,
                    int.MaxValue, true, false, true),

                #endregion Karthus

                #region Kassadin

                new TargetSpellData("kassadin", "nulllance", SpellSlot.Q, SpellType.Targeted, CcType.Silence, 650, 500,
                    1400),
                new TargetSpellData("kassadin", "netherblade", SpellSlot.W, SpellType.Self, CcType.No, 0, 0, 0),
                new TargetSpellData("kassadin", "forcepulse", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 700, 500,
                    int.MaxValue),
                new TargetSpellData("kassadin", "riftwalk", SpellSlot.R, SpellType.Skillshot, CcType.No, 675, 500,
                    int.MaxValue, false, false, true),

                #endregion Kassadin

                #region Katarina

                new TargetSpellData("katarina", "katarinaq", SpellSlot.Q, SpellType.Targeted, CcType.No, 675, 500,
                    1800),
                new TargetSpellData("katarina", "katarinaw", SpellSlot.W, SpellType.Self, CcType.No, 400, 500, 1800),
                new TargetSpellData("katarina", "katarinae", SpellSlot.E, SpellType.Targeted, CcType.No, 700, 500, 0),
                new TargetSpellData("katarina", "katarinar", SpellSlot.R, SpellType.Self, CcType.No, 550, 500, 1450,
                    true),

                #endregion Katarina

                #region Kayle

                new TargetSpellData("kayle", "kayleq", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 900,
                    250, 1600),
                new TargetSpellData("kayle", "kaylew", SpellSlot.W, SpellType.Self, CcType.No, 900,
                    0, 0),
                new TargetSpellData("kayle", "kaylee", SpellSlot.E, SpellType.Self, CcType.No, 0, 0,
                    0),
                new TargetSpellData("kayle", "kayler", SpellSlot.R, SpellType.Targeted, CcType.No, 900,
                    0, 0, false, false, true),

                #endregion Kayle

                #region Kennen

                new TargetSpellData("kennen", "kennenshurikenhurlmissile1", SpellSlot.Q, SpellType.Skillshot, CcType.No,
                    1000, 690, 1700),
                new TargetSpellData("kennen", "kennenbringthelight", SpellSlot.W, SpellType.Self, CcType.No, 900, 500,
                    int.MaxValue),
                new TargetSpellData("kennen", "kennenlightningrush", SpellSlot.E, SpellType.Self, CcType.No, 0, 0,
                    int.MaxValue),
                new TargetSpellData("kennen", "kennenshurikenstorm", SpellSlot.R, SpellType.Self, CcType.No, 550, 500,
                    779, true),

                #endregion Kennen

                #region Khazix

                new TargetSpellData("khazix", "khazixq", SpellSlot.Q, SpellType.Targeted, CcType.No, 325, 500,
                    int.MaxValue),
                new TargetSpellData("khazix", "khazixqlong", SpellSlot.Q, SpellType.Targeted, CcType.No, 375, 500,
                    int.MaxValue),
                new TargetSpellData("khazix", "khazixw", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 1000, 500, 828),
                new TargetSpellData("khazix", "khazixwlong", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 1000, 500,
                    828),
                new TargetSpellData("khazix", "khazixe", SpellSlot.E, SpellType.Skillshot, CcType.No, 600, 500,
                    int.MaxValue),
                new TargetSpellData("khazix", "khazixelong", SpellSlot.E, SpellType.Skillshot, CcType.No, 900, 500,
                    int.MaxValue),
                new TargetSpellData("khazix", "khazixr", SpellSlot.R, SpellType.Self, CcType.No, 0, 0, int.MaxValue,
                    false, true, true),
                new TargetSpellData("khazix", "khazixrlong", SpellSlot.R, SpellType.Self, CcType.No, 0, 0, int.MaxValue,
                    false, true, true),

                #endregion Khazix

                #region KogMaw

                new TargetSpellData("kogmaw", "kogmawcausticspittle", SpellSlot.Q, SpellType.Targeted, CcType.No, 625,
                    500, int.MaxValue),
                new TargetSpellData("kogmaw", "kogmawbioarcanbarrage", SpellSlot.W, SpellType.Self, CcType.No, 130, 500,
                    2000),
                new TargetSpellData("kogmaw", "kogmawvoidooze", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 1000,
                    500, 1200),
                new TargetSpellData("kogmaw", "kogmawlivingartillery", SpellSlot.R, SpellType.Skillshot, CcType.No,
                    1400, 600, 2000),

                #endregion KogMaw

                #region Leblanc

                new TargetSpellData("leblanc", "leblancchaosorb", SpellSlot.Q, SpellType.Targeted, CcType.No, 700, 500,
                    2000),
                new TargetSpellData("leblanc", "leblancslide", SpellSlot.W, SpellType.Skillshot, CcType.No, 600, 500,
                    int.MaxValue),
                new TargetSpellData("leblanc", "leblacslidereturn", SpellSlot.W, SpellType.Skillshot, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("leblanc", "leblancsoulshackle", SpellSlot.E, SpellType.Skillshot, CcType.Snare,
                    925, 500, 1600),
                new TargetSpellData("leblanc", "leblancchaosorbm", SpellSlot.R, SpellType.Targeted, CcType.No, 700, 500,
                    2000, true),
                new TargetSpellData("leblanc", "leblancslidem", SpellSlot.R, SpellType.Skillshot, CcType.No, 600, 500,
                    int.MaxValue, true),
                new TargetSpellData("leblanc", "leblancslidereturnm", SpellSlot.R, SpellType.Skillshot, CcType.No, 0,
                    500, int.MaxValue),
                new TargetSpellData("leblanc", "leblancsoulshacklem", SpellSlot.R, SpellType.Skillshot, CcType.No, 925,
                    500, 1600, true, false, true),

                #endregion Leblanc

                #region LeeSin

                new TargetSpellData("leesin", "blindmonkqone", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1000, 500,
                    1800, true),
                new TargetSpellData("leesin", "blindmonkqtwo", SpellSlot.Q, SpellType.Targeted, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("leesin", "blindmonkwone", SpellSlot.W, SpellType.Targeted, CcType.No, 700, 0,
                    1500),
                new TargetSpellData("leesin", "blindmonkwtwo", SpellSlot.W, SpellType.Self, CcType.No, 700, 0,
                    int.MaxValue),
                new TargetSpellData("leesin", "blindmonkeone", SpellSlot.E, SpellType.Self, CcType.No, 425, 500,
                    int.MaxValue),
                new TargetSpellData("leesin", "blindmonketwo", SpellSlot.E, SpellType.Self, CcType.Slow, 350, 0,
                    int.MaxValue),
                new TargetSpellData("leesin", "blindmonkrkick", SpellSlot.R, SpellType.Targeted, CcType.Knockback, 375,
                    150, 1500, true),

                #endregion LeeSin

                #region Leona

                new TargetSpellData("leona", "leonashieldofdaybreak", SpellSlot.Q, SpellType.Self, CcType.Stun, 215, 0,
                    0, true),
                new TargetSpellData("leona", "leonasolarbarrier", SpellSlot.W, SpellType.Self, CcType.No, 500, 3000, 0),
                new TargetSpellData("leona", "leonazenithblade", SpellSlot.E, SpellType.Skillshot, CcType.Stun, 900, 0,
                    2000),
                new TargetSpellData("leona", "leonazenithblademissle", SpellSlot.E, SpellType.Skillshot, CcType.Stun,
                    900, 0, 2000),
                new TargetSpellData("leona", "leonasolarflare", SpellSlot.R, SpellType.Skillshot, CcType.Stun, 1200,
                    450, int.MaxValue, true),

                #endregion Leona

                #region Lissandra

                new TargetSpellData("lissandra", "lissandraq", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 725, 500,
                    1200),
                new TargetSpellData("lissandra", "lissandraw", SpellSlot.W, SpellType.Self, CcType.Snare, 450, 500,
                    int.MaxValue, true),
                new TargetSpellData("lissandra", "lissandrae", SpellSlot.E, SpellType.Skillshot, CcType.No, 1050, 500,
                    850),
                new TargetSpellData("lissandra", "lissandrar", SpellSlot.R, SpellType.Targeted, CcType.Stun, 550, 0,
                    int.MaxValue, true),

                #endregion Lissandra

                #region Lucian

                new TargetSpellData("lucian", "lucianq", SpellSlot.Q, SpellType.Targeted, CcType.No, 550, 500, 500),
                new TargetSpellData("lucian", "lucianw", SpellSlot.W, SpellType.Skillshot, CcType.No, 1000, 500, 500),
                new TargetSpellData("lucian", "luciane", SpellSlot.E, SpellType.Skillshot, CcType.No, 650, 500,
                    int.MaxValue),
                new TargetSpellData("lucian", "lucianr", SpellSlot.R, SpellType.Skillshot, CcType.No, 1400, 500, 0,
                    false, false, true),

                #endregion Lucian

                #region Lulu

                new TargetSpellData("lulu", "luluq", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 925, 500, 1400),
                new TargetSpellData("lulu", "luluqmissle", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 925, 500,
                    1400),
                new TargetSpellData("lulu", "luluw", SpellSlot.W, SpellType.Targeted, CcType.Polymorph, 650, 640, 2000),
                new TargetSpellData("lulu", "lulue", SpellSlot.E, SpellType.Targeted, CcType.No, 650, 640,
                    int.MaxValue),
                new TargetSpellData("lulu", "lulur", SpellSlot.R, SpellType.Skillshot, CcType.Knockup, 900, 500,
                    int.MaxValue, true),

                #endregion Lulu

                #region Lux

                new TargetSpellData("lux", "luxlightbinding", SpellSlot.Q, SpellType.Skillshot, CcType.Snare, 1300, 500,
                    1200, true),
                new TargetSpellData("lux", "luxprismaticwave", SpellSlot.W, SpellType.Skillshot, CcType.No, 1075, 500,
                    1200),
                new TargetSpellData("lux", "luxlightstrikekugel", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 1100,
                    500, 1300),
                new TargetSpellData("lux", "luxlightstriketoggle", SpellSlot.E, SpellType.Skillshot, CcType.No, 1100,
                    500, 1300),
                new TargetSpellData("lux", "luxmalicecannon", SpellSlot.R, SpellType.Skillshot, CcType.No, 3340, 1750,
                    3000, true),
                new TargetSpellData("lux", "luxmalicecannonmis", SpellSlot.R, SpellType.Skillshot, CcType.No, 3340,
                    1750, 3000, true),

                #endregion Lux

                #region Kalista

                new TargetSpellData("kalista", "kalistamysticshot", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1150,
                    250, 1200),
                new TargetSpellData("kalista", "kalistamysticshotmis", SpellSlot.Q, SpellType.Skillshot, CcType.No,
                    1150, 250, 1200),
                new TargetSpellData("kalista", "kalistaw", SpellSlot.W, SpellType.Skillshot, CcType.No, 5000, 800, 200),
                new TargetSpellData("kalista", "kalistaexpungewrapper", SpellSlot.E, SpellType.Self, CcType.Slow, 1000,
                    0, 0, true),

                #endregion Kalista

                #region Malphite

                new TargetSpellData("malphite", "seismicshard", SpellSlot.Q, SpellType.Targeted, CcType.Slow, 625, 500,
                    1200),
                new TargetSpellData("malphite", "obduracy", SpellSlot.W, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("malphite", "landslide", SpellSlot.E, SpellType.Self, CcType.No, 400, 500,
                    int.MaxValue),
                new TargetSpellData("malphite", "ufslash", SpellSlot.R, SpellType.Skillshot, CcType.Knockup, 1000, 250,
                    700, true),

                #endregion Malphite

                #region Malzahar

                new TargetSpellData("malzahar", "alzaharcallofthevoid", SpellSlot.Q, SpellType.Skillshot,
                    CcType.Silence, 900, 500, int.MaxValue),
                new TargetSpellData("malzahar", "alzaharnullzone", SpellSlot.W, SpellType.Skillshot, CcType.No, 800,
                    500, int.MaxValue),
                new TargetSpellData("malzahar", "alzaharmaleficvisions", SpellSlot.E, SpellType.Targeted, CcType.No,
                    650, 500, int.MaxValue),
                new TargetSpellData("malzahar", "alzaharnethergrasp", SpellSlot.R, SpellType.Targeted,
                    CcType.Suppression, 700, 0, int.MaxValue, true),

                #endregion Malzahar

                #region Maokai

                new TargetSpellData("maokai", "maokaitrunkline", SpellSlot.Q, SpellType.Skillshot, CcType.Knockback,
                    600, 500, 1200),
                new TargetSpellData("maokai", "maokaiunstablegrowth", SpellSlot.W, SpellType.Targeted, CcType.Snare,
                    650, 500, int.MaxValue, true),
                new TargetSpellData("maokai", "maokaisapling2", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 1100,
                    500, 1750),
                new TargetSpellData("maokai", "maokaidrain3", SpellSlot.R, SpellType.Targeted, CcType.No, 625, 500,
                    int.MaxValue, false, false, true),

                #endregion Maokai

                #region MasterYi

                new TargetSpellData("masteryi", "alphastrike", SpellSlot.Q, SpellType.Targeted, CcType.No, 600, 500,
                    4000),
                new TargetSpellData("masteryi", "meditate", SpellSlot.W, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("masteryi", "wujustyle", SpellSlot.E, SpellType.Self, CcType.No, 0, 230,
                    int.MaxValue),
                new TargetSpellData("masteryi", "highlander", SpellSlot.R, SpellType.Self, CcType.No, 0, 370,
                    int.MaxValue, false, false, true),

                #endregion MasterYi

                #region MissFortune

                new TargetSpellData("missfortune", "missfortunericochetshot", SpellSlot.Q, SpellType.Targeted,
                    CcType.No, 650, 500, 1400),
                new TargetSpellData("missfortune", "missfortuneviciousstrikes", SpellSlot.W, SpellType.Self, CcType.No,
                    0, 0, int.MaxValue),
                new TargetSpellData("missfortune", "missfortunescattershot", SpellSlot.E, SpellType.Skillshot,
                    CcType.Slow, 1000, 500, 500),
                new TargetSpellData("missfortune", "missfortunebullettime", SpellSlot.R, SpellType.Skillshot, CcType.No,
                    1400, 500, 775, true),

                #endregion MissFortune

                #region MonkeyKing

                new TargetSpellData("monkeyking", "monkeykingdoubleattack", SpellSlot.Q, SpellType.Self, CcType.No, 300,
                    500, 20),
                new TargetSpellData("monkeyking", "monkeykingdecoy", SpellSlot.W, SpellType.Self, CcType.No, 0, 500, 0,
                    false, true),
                new TargetSpellData("monkeyking", "monkeykingdecoyswipe", SpellSlot.W, SpellType.Self, CcType.No, 325,
                    500, 0),
                new TargetSpellData("monkeyking", "monkeykingnimbus", SpellSlot.E, SpellType.Targeted, CcType.No, 625,
                    0, 2200),
                new TargetSpellData("monkeyking", "monkeykingspintowin", SpellSlot.R, SpellType.Self, CcType.Knockup,
                    315, 0, 700, true),
                new TargetSpellData("monkeyking", "monkeykingspintowinleave", SpellSlot.R, SpellType.Self, CcType.No, 0,
                    0, 700),

                #endregion MonkeyKing

                #region Mordekaiser

                new TargetSpellData("mordekaiser", "mordekaiserq", SpellSlot.Q, SpellType.Skillshot, CcType.No,
                    625, 500, 1500),
                new TargetSpellData("mordekaiser", "mordekaiserw", SpellSlot.W, SpellType.Self,
                    CcType.No, 625, 500, 0),
                new TargetSpellData("mordekaiser", "mordekaisere", SpellSlot.E, SpellType.Skillshot,
                    CcType.Pull, 900, 250, 3000),
                new TargetSpellData("mordekaiser", "mordekaiserr", SpellSlot.R, SpellType.Targeted,
                    CcType.No, 650, 0, 0, true),

                #endregion Mordekaiser

                #region Morgana

                new TargetSpellData("morgana", "darkbindingmissile", SpellSlot.Q, SpellType.Skillshot, CcType.Snare,
                    1300, 500, 1200, true),
                new TargetSpellData("morgana", "tormentedsoil", SpellSlot.W, SpellType.Skillshot, CcType.No, 1075, 500,
                    int.MaxValue),
                new TargetSpellData("morgana", "blackshield", SpellSlot.E, SpellType.Targeted, CcType.No, 750, 500,
                    int.MaxValue),
                new TargetSpellData("morgana", "soulshackles", SpellSlot.R, SpellType.Self, CcType.Stun, 600, 500,
                    int.MaxValue, true, false, true),

                #endregion Morgana

                #region Nami

                new TargetSpellData("nami", "namiq", SpellSlot.Q, SpellType.Skillshot, CcType.Knockup, 875, 500, 1750,
                    true),
                new TargetSpellData("nami", "namiw", SpellSlot.W, SpellType.Targeted, CcType.No, 725, 500, 1100),
                new TargetSpellData("nami", "namie", SpellSlot.E, SpellType.Targeted, CcType.Slow, 800, 500,
                    int.MaxValue),
                new TargetSpellData("nami", "namir", SpellSlot.R, SpellType.Skillshot, CcType.Knockup, 2550, 500, 1200,
                    true),

                #endregion Nami

                #region Nasus

                new TargetSpellData("nasus", "nasusq", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500, int.MaxValue),
                new TargetSpellData("nasus", "nasusw", SpellSlot.W, SpellType.Targeted, CcType.Slow, 600, 500,
                    int.MaxValue),
                new TargetSpellData("nasus", "nasuse", SpellSlot.E, SpellType.Skillshot, CcType.No, 850, 500,
                    int.MaxValue),
                new TargetSpellData("nasus", "nasusr", SpellSlot.R, SpellType.Skillshot, CcType.No, 1, 500,
                    int.MaxValue, false, false, true),

                #endregion Nasus

                #region Nautilus

                new TargetSpellData("nautilus", "nautilusanchordrag", SpellSlot.Q, SpellType.Skillshot, CcType.Pull,
                    950, 500, 1200),
                new TargetSpellData("nautilus", "nautiluspiercinggaze", SpellSlot.W, SpellType.Self, CcType.No, 0, 0,
                    0),
                new TargetSpellData("nautilus", "nautilussplashzone", SpellSlot.E, SpellType.Self, CcType.Slow, 600,
                    500, 1300),
                new TargetSpellData("nautilus", "nautilusgandline", SpellSlot.R, SpellType.Targeted, CcType.Knockup,
                    1500, 500, 1400, true, false, true),

                #endregion Nautilus

                #region Nidalee

                new TargetSpellData("nidalee", "javelintoss", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1500, 125,
                    1300),
                new TargetSpellData("nidalee", "takedown", SpellSlot.Q, SpellType.Self, CcType.No, 150, 0,
                    int.MaxValue),
                new TargetSpellData("nidalee", "bushwhack", SpellSlot.W, SpellType.Skillshot, CcType.No, 900, 500,
                    1450),
                new TargetSpellData("nidalee", "pounce", SpellSlot.W, SpellType.Skillshot, CcType.No, 375, 500, 1500),
                new TargetSpellData("nidalee", "primalsurge", SpellSlot.E, SpellType.Targeted, CcType.No, 600, 0,
                    int.MaxValue),
                new TargetSpellData("nidalee", "swipe", SpellSlot.E, SpellType.Skillshot, CcType.No, 300, 500,
                    int.MaxValue),
                new TargetSpellData("nidalee", "aspectofthecougar", SpellSlot.R, SpellType.Self, CcType.No, 0, 0,
                    int.MaxValue, false, false, true),

                #endregion Nidalee

                #region Nocturne

                new TargetSpellData("nocturne", "nocturneduskbringer", SpellSlot.Q, SpellType.Skillshot, CcType.No,
                    1125, 500, 1600),
                new TargetSpellData("nocturne", "nocturneshroudofdarkness", SpellSlot.W, SpellType.Self, CcType.No, 0,
                    500, 500),
                new TargetSpellData("nocturne", "nocturneunspeakablehorror", SpellSlot.E, SpellType.Targeted,
                    CcType.Fear, 500, 0, 0, true),
                new TargetSpellData("nocturne", "nocturneparanoia", SpellSlot.R, SpellType.Targeted, CcType.No, 2000,
                    500, 500, false, false, true),

                #endregion Nocturne

                #region Nunu

                new TargetSpellData("nunu", "nunuq", SpellSlot.Q, SpellType.Targeted, CcType.No, 125, 0, 0),
                new TargetSpellData("nunu", "nunuw", SpellSlot.W, SpellType.Skillshot, CcType.Knockup, 0, 0,
                    350),
                new TargetSpellData("nunu", "nunue", SpellSlot.E, SpellType.Skillshot, CcType.Stun, 690, 0, 0),
                new TargetSpellData("nunu", "nunur", SpellSlot.R, SpellType.Self, CcType.Slow, 650, 500,
                    int.MaxValue),

                #endregion Nunu

                #region Olaf

                new TargetSpellData("olaf", "olafaxethrowcast", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 1000,
                    500, 1600),
                new TargetSpellData("olaf", "olaffrenziedstrikes", SpellSlot.W, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("olaf", "olafrecklessstrike", SpellSlot.E, SpellType.Targeted, CcType.No, 325, 500,
                    int.MaxValue),
                new TargetSpellData("olaf", "olafragnarok", SpellSlot.R, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue, false, false, true),

                #endregion Olaf

                #region Orianna

                new TargetSpellData("orianna", "orianaizunacommand", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1100,
                    500, 1200),
                new TargetSpellData("orianna", "orianadissonancecommand", SpellSlot.W, SpellType.Skillshot, CcType.Slow,
                    0, 500, 1200),
                new TargetSpellData("orianna", "orianaredactcommand", SpellSlot.E, SpellType.Targeted, CcType.No, 1095,
                    500, 1200),
                new TargetSpellData("orianna", "orianadetonatecommand", SpellSlot.R, SpellType.Skillshot, CcType.Pull,
                    0, 500, 1200, true),

                #endregion Orianna

                #region Pantheon

                new TargetSpellData("pantheon", "pantheonq", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 1200, 0,
                    1500),
                new TargetSpellData("pantheon", "pantheonw", SpellSlot.W, SpellType.Targeted, CcType.Stun, 600, 500,
                    int.MaxValue),
                new TargetSpellData("pantheon", "pantheone", SpellSlot.E, SpellType.Self, CcType.No, 400, 0,
                    0),
                new TargetSpellData("pantheon", "pantheonrjump", SpellSlot.R, SpellType.Skillshot, CcType.No, 5500,
                    0, 0, false, false, true),
                new TargetSpellData("pantheon", "pantheonrfall", SpellSlot.R, SpellType.Skillshot, CcType.No, 2000,
                    0, 0, true),

                #endregion Pantheon

                #region Poppy

                new TargetSpellData("poppy", "poppydevastatingblow", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("poppy", "poppyparagonofdemacia", SpellSlot.W, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("poppy", "poppyheroiccharge", SpellSlot.E, SpellType.Targeted, CcType.Stun, 525,
                    500, 1450),
                new TargetSpellData("poppy", "poppydiplomaticimmunity", SpellSlot.R, SpellType.Targeted, CcType.No, 900,
                    500, int.MaxValue, false, false, true),

                #endregion Poppy

                #region Quinn

                new TargetSpellData("quinn", "quinnq", SpellSlot.Q, SpellType.Skillshot, CcType.Blind, 1025, 500, 1200),
                new TargetSpellData("quinn", "quinnw", SpellSlot.W, SpellType.Self, CcType.No, 2100, 0, 0),
                new TargetSpellData("quinn", "quinne", SpellSlot.E, SpellType.Targeted, CcType.Knockback, 700, 500,
                    775),
                new TargetSpellData("quinn", "quinnr", SpellSlot.R, SpellType.Self, CcType.No, 0, 0, 0),
                new TargetSpellData("quinn", "quinnrfinale", SpellSlot.R, SpellType.Self, CcType.No, 700, 0, 0, false,
                    false, true),

                #endregion Quinn

                #region Rammus

                new TargetSpellData("rammus", "powerball", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500, 775),
                new TargetSpellData("rammus", "defensiveballcurl", SpellSlot.W, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("rammus", "puncturingtaunt", SpellSlot.E, SpellType.Targeted, CcType.Taunt, 325,
                    500, int.MaxValue),
                new TargetSpellData("rammus", "tremors2", SpellSlot.R, SpellType.Self, CcType.No, 300, 500,
                    int.MaxValue, false, false, true),

                #endregion Rammus

                #region Renekton

                new TargetSpellData("renekton", "renektoncleave", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1, 500,
                    int.MaxValue),
                new TargetSpellData("renekton", "renektonpreexecute", SpellSlot.W, SpellType.Self, CcType.Stun, 0, 500,
                    int.MaxValue),
                new TargetSpellData("renekton", "renektonsliceanddice", SpellSlot.E, SpellType.Skillshot, CcType.No,
                    450, 500, 1400),
                new TargetSpellData("renekton", "renektonreignofthetyrant", SpellSlot.R, SpellType.Skillshot, CcType.No,
                    1, 500, 775, false, false, true),

                #endregion Renekton

                #region Rengar

                new TargetSpellData("rengar", "rengarq", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500, int.MaxValue),
                new TargetSpellData("rengar", "rengarw", SpellSlot.W, SpellType.Skillshot, CcType.No, 1, 500,
                    int.MaxValue),
                new TargetSpellData("rengar", "rengare", SpellSlot.E, SpellType.Targeted, CcType.Snare, 1000, 500,
                    1500),
                new TargetSpellData("rengar", "rengarr", SpellSlot.R, SpellType.Self, CcType.No, 0, 500, int.MaxValue,
                    false, false, true),

                #endregion Rengar

                #region Rek'Sai

                new TargetSpellData("reksai", "reksaiq", SpellSlot.Q, SpellType.Self, CcType.No, 0, 0, int.MaxValue),
                new TargetSpellData("reksai", "reksaiqburrowed", SpellSlot.W, SpellType.Self, CcType.No, 1650, 500,
                    1950),
                new TargetSpellData("reksai", "reksaiw", SpellSlot.W, SpellType.Self, CcType.No, 0, 350, int.MaxValue),
                new TargetSpellData("reksai", "reksaiwburrowed", SpellSlot.W, SpellType.Self, CcType.Knockup, 200, 300,
                    int.MaxValue),
                new TargetSpellData("reksai", "reksaie", SpellSlot.E, SpellType.Targeted, CcType.No, 250, 0,
                    int.MaxValue),
                new TargetSpellData("reksai", "reksaieburrowed", SpellSlot.E, SpellType.Skillshot, CcType.No, 350, 900,
                    1450),
                new TargetSpellData("reksai", "reksair", SpellSlot.R, SpellType.Targeted, CcType.No, int.MaxValue, 1000,
                    int.MaxValue),

                #endregion Rek'Sai

                #region Riven

                new TargetSpellData("riven", "riventricleave", SpellSlot.Q, SpellType.Skillshot, CcType.No, 250, 500,
                    0),
                new TargetSpellData("riven", "riventricleave_03", SpellSlot.Q, SpellType.Skillshot, CcType.Knockup, 250,
                    500, 0),
                new TargetSpellData("riven", "rivenmartyr", SpellSlot.W, SpellType.Self, CcType.Stun, 260, 0,
                    int.MaxValue, true),
                new TargetSpellData("riven", "rivenfeint", SpellSlot.E, SpellType.Skillshot, CcType.No, 325, 0, 1450),
                new TargetSpellData("riven", "rivenfengshuiengine", SpellSlot.R, SpellType.Self, CcType.No, 0, 500,
                    1200, false, false, true),
                new TargetSpellData("riven", "rivenizunablade", SpellSlot.R, SpellType.Skillshot, CcType.No, 900, 300,
                    2200, true),

                #endregion Riven

                #region Rumble

                new TargetSpellData("rumble", "rumbleflamethrower", SpellSlot.Q, SpellType.Skillshot, CcType.No, 600,
                    500, int.MaxValue),
                new TargetSpellData("rumble", "rumbleshield", SpellSlot.W, SpellType.Self, CcType.No, 0, 0, 0),
                new TargetSpellData("rumble", "rumbegrenade", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 850, 500,
                    1200),
                new TargetSpellData("rumble", "rumblecarpetbomb", SpellSlot.R, SpellType.Skillshot, CcType.Slow, 1700,
                    500, 1400, true),

                #endregion Rumble

                #region Ryze

                new TargetSpellData("ryze", "overload", SpellSlot.Q, SpellType.Targeted, CcType.No, 625, 500, 1400),
                new TargetSpellData("ryze", "runeprison", SpellSlot.W, SpellType.Targeted, CcType.Snare, 600, 500,
                    int.MaxValue),
                new TargetSpellData("ryze", "spellflux", SpellSlot.E, SpellType.Targeted, CcType.No, 600, 500, 1000),
                new TargetSpellData("ryze", "desperatepower", SpellSlot.R, SpellType.Targeted, CcType.No, 625, 500,
                    1400, false, false, true),

                #endregion Ryze

                #region Sejuani

                new TargetSpellData("sejuani", "sejuaniarcticassault", SpellSlot.Q, SpellType.Skillshot,
                    CcType.Knockback, 650, 500, 1450),
                new TargetSpellData("sejuani", "sejuaninorthernwinds", SpellSlot.W, SpellType.Self, CcType.No, 1, 1000,
                    1500),
                new TargetSpellData("sejuani", "sejuaniwintersclaw", SpellSlot.E, SpellType.Targeted, CcType.Slow, 1, 0,
                    1450),
                new TargetSpellData("sejuani", "sejuaniglacialprisonstart", SpellSlot.R, SpellType.Skillshot,
                    CcType.Stun, 1175, 500, 1400, true),

                #endregion Sejuani

                #region Shaco

                new TargetSpellData("shaco", "deceive", SpellSlot.Q, SpellType.Skillshot, CcType.No, 400, 500,
                    int.MaxValue, false, true),
                new TargetSpellData("shaco", "jackinthebox", SpellSlot.W, SpellType.Skillshot, CcType.Fear, 425, 500,
                    1450),
                new TargetSpellData("shaco", "twoshivpoison", SpellSlot.E, SpellType.Targeted, CcType.Slow, 625, 0,
                    1500),
                new TargetSpellData("shaco", "hallucinatefull", SpellSlot.R, SpellType.Skillshot, CcType.No, 1125, 500,
                    395, false, false, true),

                #endregion Shaco

                #region Shen

                new TargetSpellData("shen", "shenvorpalstar", SpellSlot.Q, SpellType.Targeted, CcType.No, 475, 500,
                    1500),
                new TargetSpellData("shen", "shenfeint", SpellSlot.W, SpellType.Self, CcType.No, 0, 500, int.MaxValue),
                new TargetSpellData("shen", "shenshadowdash", SpellSlot.E, SpellType.Skillshot, CcType.Taunt, 600, 500,
                    1000),
                new TargetSpellData("shen", "shenstandunited", SpellSlot.R, SpellType.Targeted, CcType.No, 25000, 500,
                    int.MaxValue, false, false, true),

                #endregion Shen

                #region Shyvana

                new TargetSpellData("shyvana", "shyvanadoubleattack", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("shyvana", "shyvanadoubleattackdragon", SpellSlot.Q, SpellType.Self, CcType.No, 0,
                    500, int.MaxValue),
                new TargetSpellData("shyvana", "shyvanaimmolationauraqw", SpellSlot.W, SpellType.Self, CcType.No, 0,
                    500, int.MaxValue),
                new TargetSpellData("shyvana", "shyvanaimmolateddragon", SpellSlot.W, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("shyvana", "shyvanafireball", SpellSlot.E, SpellType.Skillshot, CcType.No, 925, 500,
                    1200),
                new TargetSpellData("shyvana", "shyvanafireballdragon2", SpellSlot.E, SpellType.Skillshot, CcType.No,
                    925, 500, 1200),
                new TargetSpellData("shyvana", "shyvanatransformcast", SpellSlot.R, SpellType.Skillshot, CcType.No,
                    1000, 500, 700),
                new TargetSpellData("shyvana", "shyvanatransformleap", SpellSlot.R, SpellType.Skillshot,
                    CcType.Knockback, 1000, 500, 700),

                #endregion Shyvana

                #region Singed

                new TargetSpellData("singed", "poisentrail", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("singed", "megaadhesive", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 1175, 500,
                    700),
                new TargetSpellData("singed", "fling", SpellSlot.E, SpellType.Targeted, CcType.Pull, 125, 500,
                    int.MaxValue),
                new TargetSpellData("singed", "insanitypotion", SpellSlot.R, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue, false, false, true),

                #endregion Singed

                #region Sion

                new TargetSpellData("sion", "sionq", SpellSlot.Q, SpellType.Skillshot, CcType.Stun, 600, 2000, 1600),
                new TargetSpellData("sion", "sionw", SpellSlot.W, SpellType.Self, CcType.No, 550, 500, int.MaxValue),
                new TargetSpellData("sion", "sione", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 1000, 500,
                    int.MaxValue),
                new TargetSpellData("sion", "sionr", SpellSlot.R, SpellType.Self, CcType.Stun, 0, 500, 500, false,
                    false, true),

                #endregion Sion

                #region Sivir

                new TargetSpellData("sivir", "sivirq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1165, 500, 1350),
                new TargetSpellData("sivir", "sivirw", SpellSlot.W, SpellType.Targeted, CcType.No, 565, 500,
                    int.MaxValue),
                new TargetSpellData("sivir", "sivire", SpellSlot.E, SpellType.Self, CcType.No, 0, 500, int.MaxValue),
                new TargetSpellData("sivir", "sivirr", SpellSlot.R, SpellType.Self, CcType.No, 1000, 500, int.MaxValue,
                    false, false, true),

                #endregion Sivir

                #region Skarner

                new TargetSpellData("skarner", "skarnervirulentslash", SpellSlot.Q, SpellType.Self, CcType.No, 350, 0,
                    int.MaxValue),
                new TargetSpellData("skarner", "skarnerexoskeleton", SpellSlot.W, SpellType.Self, CcType.No, 0, 0,
                    int.MaxValue),
                new TargetSpellData("skarner", "skarnerfracture", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 1100,
                    500, 1200),
                new TargetSpellData("skarner", "skarnerfracturemissilespell", SpellSlot.E, SpellType.Skillshot,
                    CcType.Slow, 1100, 500, 1200),
                new TargetSpellData("skarner", "skarnerimpale", SpellSlot.R, SpellType.Targeted, CcType.Suppression,
                    350, 0, int.MaxValue, true),

                #endregion Skarner

                #region Sona

                new TargetSpellData("sona", "sonaq", SpellSlot.Q, SpellType.Self, CcType.No, 700, 500, 1500),
                new TargetSpellData("sona", "sonaw", SpellSlot.W, SpellType.Self, CcType.No, 1000, 500, 1500),
                new TargetSpellData("sona", "sonae", SpellSlot.E, SpellType.Self, CcType.Slow, 1000, 500, 1500),
                new TargetSpellData("sona", "sonar", SpellSlot.R, SpellType.Skillshot, CcType.Stun, 900, 500, 2400,
                    true),

                #endregion Sona

                #region Soraka

                new TargetSpellData("soraka", "sorakaq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 675, 500,
                    int.MaxValue),
                new TargetSpellData("soraka", "sorakaw", SpellSlot.W, SpellType.Targeted, CcType.No, 750, 500,
                    int.MaxValue),
                new TargetSpellData("soraka", "sorakae", SpellSlot.E, SpellType.Skillshot, CcType.Silence, 725, 500,
                    int.MaxValue),
                new TargetSpellData("soraka", "sorakar", SpellSlot.R, SpellType.Self, CcType.No, 25000, 500,
                    int.MaxValue, false, false, true),

                #endregion Soraka

                #region Swain

                new TargetSpellData("swain", "swaindecrepify", SpellSlot.Q, SpellType.Targeted, CcType.Slow, 625, 500,
                    int.MaxValue),
                new TargetSpellData("swain", "swainshadowgrasp", SpellSlot.W, SpellType.Skillshot, CcType.Snare, 1040,
                    500, 1250),
                new TargetSpellData("swain", "swaintorment", SpellSlot.E, SpellType.Targeted, CcType.No, 625, 500,
                    1400),
                new TargetSpellData("swain", "swainmetamorphism", SpellSlot.R, SpellType.Self, CcType.No, 700, 500, 950,
                    false, false, true),

                #endregion Swain

                #region Syndra

                new TargetSpellData("syndra", "syndraq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 800, 250, 1750),
                new TargetSpellData("syndra", "syndraw", SpellSlot.W, SpellType.Targeted, CcType.No, 925, 500, 1450),
                new TargetSpellData("syndra", "syndrawcast", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 950, 500,
                    1450),
                new TargetSpellData("syndra", "syndrae", SpellSlot.E, SpellType.Skillshot, CcType.Stun, 700, 500, 902),
                new TargetSpellData("syndra", "syndrar", SpellSlot.R, SpellType.Targeted, CcType.No, 675, 500, 1100,
                    true),

                #endregion Syndra

                #region Talon

                new TargetSpellData("talon", "talonnoxiandiplomacy", SpellSlot.Q, SpellType.Self, CcType.No, 0, 0, 0),
                new TargetSpellData("talon", "talonrake", SpellSlot.W, SpellType.Skillshot, CcType.No, 750, 500, 1200),
                new TargetSpellData("talon", "taloncutthroat", SpellSlot.E, SpellType.Targeted, CcType.Slow, 750, 0,
                    1200),
                new TargetSpellData("talon", "talonshadowassault", SpellSlot.R, SpellType.Self, CcType.No, 550, 0, 0,
                    true, true),

                #endregion Talon

                #region Taric

                new TargetSpellData("taric", "imbue", SpellSlot.Q, SpellType.Targeted, CcType.No, 750, 500, 1200),
                new TargetSpellData("taric", "shatter", SpellSlot.W, SpellType.Self, CcType.No, 400, 500, int.MaxValue),
                new TargetSpellData("taric", "dazzle", SpellSlot.E, SpellType.Targeted, CcType.Stun, 625, 500, 1400),
                new TargetSpellData("taric", "tarichammersmash", SpellSlot.R, SpellType.Self, CcType.No, 400, 500,
                    int.MaxValue, true),

                #endregion Taric

                #region Teemo

                new TargetSpellData("teemo", "blindingdart", SpellSlot.Q, SpellType.Targeted, CcType.Blind, 580, 500,
                    1500),
                new TargetSpellData("teemo", "movequick", SpellSlot.W, SpellType.Self, CcType.No, 0, 0, 943),
                new TargetSpellData("teemo", "toxicshot", SpellSlot.E, SpellType.Self, CcType.No, 0, 500, int.MaxValue),
                new TargetSpellData("teemo", "bantamtrap", SpellSlot.R, SpellType.Skillshot, CcType.Slow, 230, 0, 1500,
                    false, false, true),

                #endregion Teemo

                #region Thresh

                new TargetSpellData("thresh", "threshq", SpellSlot.Q, SpellType.Skillshot, CcType.Pull, 1175, 500,
                    1200),
                new TargetSpellData("thresh", "threshw", SpellSlot.W, SpellType.Skillshot, CcType.No, 950, 500,
                    int.MaxValue),
                new TargetSpellData("thresh", "threshe", SpellSlot.E, SpellType.Skillshot, CcType.Knockback, 515, 200,
                    int.MaxValue),
                new TargetSpellData("thresh", "threshrpenta", SpellSlot.R, SpellType.Skillshot, CcType.Slow, 420, 300,
                    int.MaxValue, true, false, true),

                #endregion Thresh

                #region Tristana

                new TargetSpellData("tristana", "tristanaq", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("tristana", "tristanaw", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 900, 500,
                    1150),
                new TargetSpellData("tristana", "tristanae", SpellSlot.E, SpellType.Targeted, CcType.No, 625, 500,
                    1400),
                new TargetSpellData("tristana", "tristanar", SpellSlot.R, SpellType.Targeted, CcType.Knockback, 700,
                    500, 1600, true),

                #endregion Tristana

                #region Trundle

                new TargetSpellData("trundle", "trundletrollsmash", SpellSlot.Q, SpellType.Targeted, CcType.Slow, 0,
                    500, int.MaxValue),
                new TargetSpellData("trundle", "trundledesecrate", SpellSlot.W, SpellType.Skillshot, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("trundle", "trundlecircle", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 1100,
                    500, 1600),
                new TargetSpellData("trundle", "trundlepain", SpellSlot.R, SpellType.Targeted, CcType.No, 700, 500,
                    1400, true, false, true),

                #endregion Trundle

                #region Tryndamere

                new TargetSpellData("tryndamere", "bloodlust", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("tryndamere", "mockingshout", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 400,
                    500, 500),
                new TargetSpellData("tryndamere", "slashcast", SpellSlot.E, SpellType.Skillshot, CcType.No, 660, 500,
                    700),
                new TargetSpellData("tryndamere", "undyingrage", SpellSlot.R, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue, false, false, true),

                #endregion Tryndamere

                #region Twitch

                new TargetSpellData("twich", "hideinshadows", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue, false, true),
                new TargetSpellData("twich", "twitchvenomcask", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 800, 500,
                    1750),
                new TargetSpellData("twich", "twitchvenomcaskmissle", SpellSlot.W, SpellType.Skillshot, CcType.Slow,
                    800, 500, 1750),
                new TargetSpellData("twich", "expunge", SpellSlot.E, SpellType.Targeted, CcType.No, 1200, 500,
                    int.MaxValue),
                new TargetSpellData("twich", "fullautomatic", SpellSlot.R, SpellType.Targeted, CcType.No, 850, 500, 500,
                    true, false, true),

                #endregion Twitch

                #region TwistedFate

                new TargetSpellData("twistedfate", "wildcards", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1450, 500,
                    1450),
                new TargetSpellData("twistedfate", "pickacard", SpellSlot.W, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue, false, false, true),
                new TargetSpellData("twistedfate", "goldcardpreattack", SpellSlot.W, SpellType.Targeted, CcType.Stun,
                    600, 0, int.MaxValue),
                new TargetSpellData("twistedfate", "redcardpreattack", SpellSlot.W, SpellType.Targeted, CcType.Slow,
                    600, 0, int.MaxValue),
                new TargetSpellData("twistedfate", "bluecardpreattack", SpellSlot.W, SpellType.Targeted, CcType.No, 600,
                    0, int.MaxValue),
                new TargetSpellData("twistedfate", "cardmasterstack", SpellSlot.E, SpellType.Self, CcType.No, 525, 500,
                    1200),
                new TargetSpellData("twistedfate", "destiny", SpellSlot.R, SpellType.Skillshot, CcType.No, 5500, 500,
                    int.MaxValue, false, false, true),

                #endregion TwistedFate

                #region Udyr

                new TargetSpellData("udyr", "udyrtigerstance", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("udyr", "udyrturtlestance", SpellSlot.W, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("udyr", "udyrbearstance", SpellSlot.E, SpellType.Self, CcType.Stun, 0, 500,
                    int.MaxValue),
                new TargetSpellData("udyr", "udyrphoenixstance", SpellSlot.R, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue, false, false, true),

                #endregion Udyr

                #region Urgot

                new TargetSpellData("urgot", "urgotheatseekinglineqqmissile", SpellSlot.Q, SpellType.Skillshot,
                    CcType.No, 1000, 500, 1600),
                new TargetSpellData("urgot", "urgotheatseekingmissile", SpellSlot.Q, SpellType.Skillshot, CcType.No,
                    1000, 500, 1600),
                new TargetSpellData("urgot", "urgotterrorcapacitoractive2", SpellSlot.W, SpellType.Self, CcType.No, 0,
                    500, int.MaxValue),
                new TargetSpellData("urgot", "urgotplasmagrenade", SpellSlot.E, SpellType.Skillshot, CcType.No, 950,
                    500, 1750),
                new TargetSpellData("urgot", "urgotplasmagrenadeboom", SpellSlot.E, SpellType.Skillshot, CcType.No, 950,
                    500, 1750),
                new TargetSpellData("urgot", "urgotswap2", SpellSlot.R, SpellType.Targeted, CcType.Suppression, 850,
                    500, 1800, true),

                #endregion Urgot

                #region Varus

                new TargetSpellData("varus", "varusq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 1500, 500, 1500),
                new TargetSpellData("varus", "varusw", SpellSlot.W, SpellType.Self, CcType.No, 0, 500, 0),
                new TargetSpellData("varus", "varuse", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 925, 500, 1500),
                new TargetSpellData("varus", "varusr", SpellSlot.R, SpellType.Skillshot, CcType.Snare, 1300, 500, 1500,
                    true),

                #endregion Varus

                #region Vayne

                new TargetSpellData("vayne", "vaynetumble", SpellSlot.Q, SpellType.Skillshot, CcType.No, 250, 500,
                    int.MaxValue),
                new TargetSpellData("vayne", "vaynesilverbolts", SpellSlot.W, SpellType.Self, CcType.No, 0, 0,
                    int.MaxValue),
                new TargetSpellData("vayne", "vaynecondemnmissile", SpellSlot.E, SpellType.Targeted, CcType.Stun, 450,
                    100, 1200),
                new TargetSpellData("vayne", "vayneinquisition", SpellSlot.R, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue, false, false, true),

                #endregion Vayne

                #region Veigar

                new TargetSpellData("veigar", "veigarbalefulstrike", SpellSlot.Q, SpellType.Skillshot, CcType.No, 850,
                    250, 1750),
                new TargetSpellData("veigar", "veigardarkmatter", SpellSlot.W, SpellType.Skillshot, CcType.No, 900,
                    1200, 1500),
                new TargetSpellData("veigar", "veigareventhorizon", SpellSlot.E, SpellType.Skillshot, CcType.Stun, 650,
                    0, 1500),
                new TargetSpellData("veigar", "veigarprimordialburst", SpellSlot.R, SpellType.Targeted, CcType.No, 650,
                    500, 1400, true),

                #endregion Veigar

                #region Velkoz

                new TargetSpellData("velkoz", "velkozq", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 1050, 300,
                    1200),
                new TargetSpellData("velkoz", "velkozqmissle", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 1050, 0,
                    1200),
                new TargetSpellData("velkoz", "velkozqplitactive", SpellSlot.Q, SpellType.Skillshot, CcType.Slow, 1050,
                    800, 1200),
                new TargetSpellData("velkoz", "velkozw", SpellSlot.W, SpellType.Skillshot, CcType.No, 1050, 0, 1200),
                new TargetSpellData("velkoz", "velkoze", SpellSlot.E, SpellType.Targeted, CcType.Knockup, 850, 0, 500),
                new TargetSpellData("velkoz", "velkozr", SpellSlot.R, SpellType.Skillshot, CcType.No, 1575, 0, 1500),

                #endregion Velkoz

                #region Vi

                new TargetSpellData("vi", "viq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 800, 500, 1500),
                new TargetSpellData("vi", "viw", SpellSlot.W, SpellType.Self, CcType.No, 0, 0, 0),
                new TargetSpellData("vi", "vie", SpellSlot.E, SpellType.Skillshot, CcType.No, 600, 0, 0),
                new TargetSpellData("vi", "vir", SpellSlot.R, SpellType.Targeted, CcType.Stun, 800, 500, 0, true),

                #endregion Vi

                #region Viktor

                new TargetSpellData("viktor", "viktorpowertransfer", SpellSlot.Q, SpellType.Targeted, CcType.No, 600,
                    500, 1400),
                new TargetSpellData("viktor", "viktorgravitonfield", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 815,
                    500, 1750),
                new TargetSpellData("viktor", "viktordeathray", SpellSlot.E, SpellType.Skillshot, CcType.No, 700, 500,
                    1210),
                new TargetSpellData("viktor", "viktorchaosstorm", SpellSlot.R, SpellType.Skillshot, CcType.Silence, 700,
                    500, 1210, true, false, true),

                #endregion Viktor

                #region Vladimir

                new TargetSpellData("vladimir", "vladimirtransfusion", SpellSlot.Q, SpellType.Targeted, CcType.No, 600,
                    500, 1400),
                new TargetSpellData("vladimir", "vladimirsanguinepool", SpellSlot.W, SpellType.Self, CcType.Slow, 350,
                    500, 1600),
                new TargetSpellData("vladimir", "vladimirtidesofblood", SpellSlot.E, SpellType.Self, CcType.No, 610,
                    500, 1100),
                new TargetSpellData("vladimir", "vladimirhemoplague", SpellSlot.R, SpellType.Skillshot, CcType.No, 875,
                    500, 1200, true, false, true),

                #endregion Vladimir

                #region Volibear

                new TargetSpellData("volibear", "volibearq", SpellSlot.Q, SpellType.Self, CcType.No, 300, 500,
                    int.MaxValue),
                new TargetSpellData("volibear", "volibearw", SpellSlot.W, SpellType.Targeted, CcType.No, 400, 500,
                    1450),
                new TargetSpellData("volibear", "volibeare", SpellSlot.E, SpellType.Self, CcType.Slow, 425, 500, 825),
                new TargetSpellData("volibear", "volibearr", SpellSlot.R, SpellType.Self, CcType.No, 425, 0, 825, false,
                    false, true),

                #endregion Volibear

                #region Warwick

                new TargetSpellData("warwick", "hungeringstrike", SpellSlot.Q, SpellType.Targeted, CcType.No, 400, 0,
                    int.MaxValue),
                new TargetSpellData("warwick", "hunterscall", SpellSlot.W, SpellType.Self, CcType.No, 1000, 0,
                    int.MaxValue),
                new TargetSpellData("warwick", "bloodscent", SpellSlot.E, SpellType.Self, CcType.No, 1500, 0,
                    int.MaxValue),
                new TargetSpellData("warwick", "infiniteduress", SpellSlot.R, SpellType.Targeted, CcType.Suppression,
                    700, 500, int.MaxValue, true),

                #endregion Warwick

                #region Xerath

                new TargetSpellData("xerath", "xeratharcanopulsechargeup", SpellSlot.Q, SpellType.Skillshot, CcType.No,
                    750, 750, 500),
                new TargetSpellData("xerath", "xeratharcanebarrage2", SpellSlot.W, SpellType.Skillshot, CcType.Slow,
                    1100, 500, 20),
                new TargetSpellData("xerath", "xerathmagespear", SpellSlot.E, SpellType.Skillshot, CcType.Stun, 1050,
                    500, 1600),
                new TargetSpellData("xerath", "xerathlocusofpower2", SpellSlot.R, SpellType.Skillshot, CcType.No, 5600,
                    750, 500),

                #endregion Xerath

                #region Xin Zhao

                new TargetSpellData("xinzhao", "xenzhaocombotarget", SpellSlot.Q, SpellType.Self, CcType.No, 200, 0,
                    2000),
                new TargetSpellData("xinzhao", "xenzhaobattlecry", SpellSlot.W, SpellType.Self, CcType.No, 0, 0, 2000),
                new TargetSpellData("xinzhao", "xenzhaosweep", SpellSlot.E, SpellType.Targeted, CcType.Slow, 600, 500,
                    1750),
                new TargetSpellData("xinzhao", "xenzhaoparry", SpellSlot.R, SpellType.Self, CcType.Knockback, 375, 0,
                    1750, true),

                #endregion Xin Zhao

                #region Yasuo

                new TargetSpellData("yasuo", "yasuoqw", SpellSlot.Q, SpellType.Skillshot, CcType.No, 475, 750, 1500),
                new TargetSpellData("yasuo", "yasuoq2w", SpellSlot.Q, SpellType.Skillshot, CcType.No, 475, 750, 1500),
                new TargetSpellData("yasuo", "yasuoq3w", SpellSlot.Q, SpellType.Skillshot, CcType.Knockup, 1000, 750,
                    1500),
                new TargetSpellData("yasuo", "yasuowmovingwall", SpellSlot.W, SpellType.Skillshot, CcType.No, 400, 500,
                    500),
                new TargetSpellData("yasuo", "yasuodashwrapper", SpellSlot.E, SpellType.Targeted, CcType.No, 475, 500,
                    20),
                new TargetSpellData("yasuo", "yasuorknockupcombow", SpellSlot.R, SpellType.Self, CcType.No, 1200, 500,
                    20, true, false, true),

                #endregion Yasuo

                #region Yorick

                new TargetSpellData("yorick", "yorickspectral", SpellSlot.Q, SpellType.Self, CcType.No, 0, 500,
                    int.MaxValue),
                new TargetSpellData("yorick", "yorickdecayed", SpellSlot.W, SpellType.Skillshot, CcType.Slow, 600, 500,
                    int.MaxValue),
                new TargetSpellData("yorick", "yorickravenous", SpellSlot.E, SpellType.Targeted, CcType.Slow, 550, 500,
                    int.MaxValue),
                new TargetSpellData("yorick", "yorickreviveally", SpellSlot.R, SpellType.Targeted, CcType.No, 900, 500,
                    1500, false, false, true),

                #endregion Yorick

                #region Zac

                new TargetSpellData("zac", "zacq", SpellSlot.Q, SpellType.Skillshot, CcType.Knockup, 550, 500, 902),
                new TargetSpellData("zac", "zacw", SpellSlot.W, SpellType.Self, CcType.No, 350, 500, 1600),
                new TargetSpellData("zac", "zace", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 1550, 500, 1500),
                new TargetSpellData("zac", "zacr", SpellSlot.R, SpellType.Self, CcType.Knockback, 850, 500, 1800, true),

                #endregion Zac

                #region Zed

                new TargetSpellData("zed", "zedshuriken", SpellSlot.Q, SpellType.Skillshot, CcType.No, 900, 500, 902),
                new TargetSpellData("zed", "zedshdaowdash", SpellSlot.W, SpellType.Skillshot, CcType.No, 550, 500,
                    1600),
                new TargetSpellData("zed", "zedpbaoedummy", SpellSlot.E, SpellType.Self, CcType.Slow, 300, 0, 0),
                new TargetSpellData("zed", "zedult", SpellSlot.R, SpellType.Targeted, CcType.No, 850, 500, 0, true,
                    false, true),

                #endregion Zed

                #region Ziggs

                new TargetSpellData("ziggs", "ziggsq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 850, 500, 1750),
                new TargetSpellData("ziggs", "ziggsqspell", SpellSlot.Q, SpellType.Skillshot, CcType.No, 850, 500,
                    1750),
                new TargetSpellData("ziggs", "ziggsw", SpellSlot.W, SpellType.Skillshot, CcType.Knockup, 850, 500,
                    1750),
                new TargetSpellData("ziggs", "ziggswtoggle", SpellSlot.W, SpellType.Self, CcType.Knockup, 850, 500,
                    1750),
                new TargetSpellData("ziggs", "ziggse", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 850, 500, 1750),
                new TargetSpellData("ziggs", "ziggse2", SpellSlot.E, SpellType.Skillshot, CcType.Slow, 850, 500, 1750),
                new TargetSpellData("ziggs", "ziggsr", SpellSlot.R, SpellType.Skillshot, CcType.No, 850, 500, 1750,
                    true),

                #endregion Ziggs

                #region Zilean

                new TargetSpellData("zilean", "zileanq", SpellSlot.Q, SpellType.Skillshot, CcType.No, 900, 300, 2000),
                new TargetSpellData("zilean", "zileanw", SpellSlot.W, SpellType.Self, CcType.No, 0, 500, int.MaxValue),
                new TargetSpellData("zilean", "zileane", SpellSlot.E, SpellType.Targeted, CcType.Slow, 700, 500, 1100),
                new TargetSpellData("zilean", "zileanr", SpellSlot.R, SpellType.Targeted, CcType.No, 780, 500,
                    int.MaxValue, false, false, true),

                #endregion Zilean

                #region Zyra

                new TargetSpellData("zyra", "zyraqfissure", SpellSlot.Q, SpellType.Skillshot, CcType.No, 800, 500,
                    1400),
                new TargetSpellData("zyra", "zyraseed", SpellSlot.W, SpellType.Skillshot, CcType.No, 800, 500, 2200),
                new TargetSpellData("zyra", "zyragraspingroots", SpellSlot.E, SpellType.Skillshot, CcType.Snare, 1100,
                    500, 1400),
                new TargetSpellData("zyra", "zyrabramblezone", SpellSlot.R, SpellType.Skillshot, CcType.Knockup, 700,
                    500, 20, true),

                #endregion Zyra
            };
        }

        public static TargetSpellData GetByName(string spellName)
        {
            spellName = spellName.ToLower();
            return Spells.FirstOrDefault(spell => spell.Name == spellName);
        }
    }

    public class TargetSpell
    {
        public AIHeroClient Caster { get; set; }
        public AIBaseClient Target { get; set; }
        public TargetSpellData Spell { get; set; }
        public int StartTick { get; set; }
        public Vector2 StartPosition { get; set; }

        public int EndTick =>
            (int)(StartTick + Spell.Delay + 1000 * (StartPosition.Distance(EndPosition) / Spell.Speed));

        public Vector2 EndPosition => Target.Position.ToVector2();

        public Vector2 Direction => (EndPosition - StartPosition).Normalized();

        public double Damage => Caster.GetSpellDamage(Target, (EnsoulSharp.SpellSlot)Spell.Spellslot);

        public bool IsKillable => Damage >= Target.Health;

        public CcType CrowdControl => Spell.CcType;

        public SpellType Type => Spell.Type;

        public bool IsActive => Environment.TickCount <= EndTick;

        public bool HasMissile => Spell.Speed == float.MaxValue || Spell.Speed == 0;
    }

    public class TargetSpellData
    {
        public TargetSpellData(string champion, string name, SpellSlot slot, SpellType type, CcType cc, float range,
            int delay, int speed, bool dangerous = false, bool stealth = false, bool wait = false)
        {
            ChampionName = champion;
            Name = name;
            Spellslot = slot;
            Type = type;
            CcType = cc;
            Range = range;
            Speed = speed;
            Delay = delay;
            Dangerous = dangerous;
            Stealth = stealth;
            Wait = wait;
        }

        public string ChampionName { get; set; }
        public SpellSlot Spellslot { get; set; }
        public SpellType Type { get; set; }
        public CcType CcType { get; set; }
        public string Name { get; set; }
        public float Range { get; set; }
        public int Delay { get; set; }
        public int Speed { get; set; }
        public bool Dangerous { get; set; }
        public bool Stealth { get; set; }
        public bool Wait { get; set; }
    }

    public enum SpellType
    {
        Skillshot,
        Targeted,
        Self,
        AutoAttack
    }

    public enum SpellSlot
    {
        Unknown = -1,
        Q = 0,
        W = 1,
        E = 2,
        R = 3,
        Summoner1 = 4,
        Summoner2 = 5,
        Recall = 13
    }

    public enum CcType
    {
        No,
        Stun,
        Silence,
        Taunt,
        Polymorph,
        Slow,
        Snare,
        Fear,
        Charm,
        Suppression,
        Blind,
        Flee,
        Knockup,
        Knockback,
        Pull
    }
}