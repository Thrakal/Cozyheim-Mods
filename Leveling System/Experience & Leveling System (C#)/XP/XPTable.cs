using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class XPTable
    {
        public static Dictionary<string, int> monsterXPTable = new Dictionary<string, int>()
        {
            // Tier 1 Mobs: Meadows (Lv. 1-10)
            // (80xp -> 895xp,  Total: 4.817xp)
            {"Gull", 3},                  // Difficulty: 0  
            {"Deer", 5},                  // Difficulty: 0   
            {"Neck", 5},                  // Difficulty: 1   
            {"Boar", 8},                  // Difficulty: 1  
            {"Greyling", 8},              // Difficulty: 1   
            {"Eikthyr", 96},              // Eikthyr        


            // Tier 2 Mobs: BlackForest (Lv. 11-30)
            // (991xp -> 3.420xp,  Total: 41.654xp)
            {"Greydwarf", 22},            // Difficulty: 2   
            {"Skeleton", 30},             // Difficulty: 2  
            {"Ghost", 36},                // Difficulty: 2   
            {"Greydwarf_Shaman", 40},     // Difficulty: 2  
            {"Skeleton_Poison", 53},      // Difficulty: 3   
            {"Greydwarf_Elite", 69},      // Difficulty: 4   
            {"Troll", 243},               // Difficulty: 7   
            {"gd_king", 833},             // The Elder      


            // Tier 3 Mobs: Swamp (Lv. 31-50)
            // (3.610xp -> 11.697xp,  Total: 134.247xp)
            {"Surtling", 85},             // Difficulty: 2   
            {"Blob", 99},                 // Difficulty: 3   
            {"Leech", 120},               // Difficulty: 3   
            {"Draugr", 139},              // Difficulty: 3   
            {"Wraith", 156},              // Difficulty: 4  
            {"BlobElite", 187},           // Difficulty: 4  
            {"Draugr_Elite", 224},        // Difficulty: 5   
            {"Abomination", 783},         // Difficulty: 8   
            {"Bonemass", 2685},           // Bonemass       


            // Tier 4 Mobs: Mountain + Ocean (Lv. 51-70)
            // (12.603xp -> 67.445xp,  Total: 639.932xp)
            {"Bat", 109},                 // Difficulty: 1   
            {"Ulv", 457},                 // Difficulty: 4      
            {"Wolf", 479},                // Difficulty: 4 
            {"Hatchling", 609},           // Difficulty: 4  
            {"Cultist", 762},             // Difficulty: 5 
            {"Fenring", 1067},            // Difficulty: 5 
            {"Serpent", 3200},            // Difficulty: 6 
            {"StoneGolem", 1600},         // Difficulty: 9  
            {"Dragon", 12800},            // Moder          


            // Tier 5 Mobs: Plains (Lv. 71-80)
            // (74.508xp -> 189.009xp,  Total: 1.234.664xp)
            {"Deathsquito", 310},         // Difficulty: 3  
            {"BlobTar", 392},             // Difficulty: 4  
            {"GoblinShaman", 490},        // Difficulty: 4
            {"Goblin", 563},              // Difficulty: 5   
            {"GoblinBrute", 1731},        // Difficulty: 9   
            {"Lox", 2058},                // Difficulty: 10   
            {"GoblinKing", 24693},        // Yagluth        


            // Tier 6 Mobs: Mistlands (Lv. 81-90)
            // (210.294xp -> 562.259xp,  Total: 3.600.378xp)
            {"Hare", 34},                 // Difficulty: 1  - Faction: AnimalVeg
            {"SeekerBrood", 483},         // Difficulty: 3  
            {"Tick", 504},                // Difficulty: 3 
            {"DvergerMageSupport", 1207}, // Difficulty: 5  - Faction: Dverger
            {"Seeker", 1517},             // Difficulty: 5 
            {"DvergerMageFire", 1897},    // Difficulty: 6  - Faction: Dverger
            {"Dverger", 1966 },           // Difficulty: 6  - Faction: Dverger
            {"DvergerMage", 1966},        // Difficulty: 6  - Faction: Dverger
            {"DvergerMageIce", 2586},     // Difficulty: 7  - Faction: Dverger
            {"SeekerBrute", 6001},        // Difficulty: 11
            {"Gjall", 18002},             // Difficulty: 11
            {"SeekerQueen", 72008}        // The Queen      
        };

        public static int[] playerXPTable;
        /*
        Lv1:80,       Lv2:167,      Lv3:255,      Lv4:343,      Lv5:433,      Lv6:523,      Lv7:614,      Lv8:707,      Lv9:800,      Lv10:895,         (Total: 4.817xp       Diff: 4.817xp)
        Lv11:991,     Lv12:1089,    Lv13:1189,    Lv14:1291,    Lv15:1394,    Lv16:1500,    Lv17:1609,    Lv18:1720,    Lv19:1835,    Lv20:1953,        (Total: 19.388xp      Diff: 14.571xp)
        Lv21:2074,    Lv22:2200,    Lv23:2331,    Lv24:2466,    Lv25:2607,    Lv26:2755,    Lv27:2909,    Lv28:3071,    Lv29:3241,    Lv30:3420,        (Total: 46.462xp      Diff: 27.074xp)
        Lv31:3610,    Lv32:3811,    Lv33:4024,    Lv34:4251,    Lv35:4494,    Lv36:4754,    Lv37:5032,    Lv38:5332,    Lv39:5654,    Lv40:6002,        (Total: 93.426xp      Diff: 46.964xp)
        Lv41:6379,    Lv42:6787,    Lv43:7230,    Lv44:7712,    Lv45:8237,    Lv46:8810,    Lv47:9436,    Lv48:10122,   Lv49:10873,   Lv50:11697,       (Total: 180.709xp     Diff: 87.283xp)
        Lv51:12603,   Lv52:13599,   Lv53:14696,   Lv54:15906,   Lv55:17240,   Lv56:18713,   Lv57:20341,   Lv58:22141,   Lv59:24134,   Lv60:26342,       (Total: 366.424xp     Diff: 185.715xp)
        Lv61:28789,   Lv62:31503,   Lv63:34515,   Lv64:37860,   Lv65:41576,   Lv66:45707,   Lv67:50302,   Lv68:55414,   Lv69:61106,   Lv70:67445,       (Total: 820.641xp     Diff: 454.217xp)
        Lv71:74508,   Lv72:82382,   Lv73:91162,   Lv74:100958,  Lv75:111890,  Lv76:124095,  Lv77:137726,  Lv78:152956,  Lv79:169978,  Lv80:189009,      (Total: 2.055.305xp   Diff: 1.234.664xp)
        Lv81:210294,  Lv82:234108,  Lv83:260759,  Lv84:290595,  Lv85:324009,  Lv86:361440,  Lv87:403385,  Lv88:450404,  Lv89:503125,  Lv90:562259       (Total: 5.655.683xp   Diff: 3.600.378xp)
        */


        public static void GenerateDefaultPlayerXPTable()
        {
            int totalLevels = 90;
            float baseXP = 80;
            float fixedXPAddPrLv = 80f;
            float lvMultiplier = 0.09f;
            float lvMultiplierAdd = 0.00033f;

            int totalXpNeeded = 0;
            int lastTotal = 0;

            List<int> levels = new List<int>();

            for(int i = 1; i <= totalLevels; i++)
            {
                int xpForThisLevel = (int)(baseXP + fixedXPAddPrLv * (i - 1));
                baseXP *= 1f + lvMultiplier;
                lvMultiplier += lvMultiplierAdd;

                levels.Add(xpForThisLevel);
                totalXpNeeded += xpForThisLevel;

                if(i % 10 == 0)
                {
                    ConsoleLog.Print("Lv. " + i + ": " + totalXpNeeded + "xp total, (diff: " + (totalXpNeeded - lastTotal) + "xp)");
                    lastTotal = totalXpNeeded;
                }
            }
            ConsoleLog.Print("Lv. " + totalLevels + " (max): " + totalXpNeeded + "xp total, (diff: " + (totalXpNeeded - lastTotal) + "xp)");

            playerXPTable = levels.ToArray(); 
        }

        public static void UpdateMonsterXPTable()
        {
            ConsoleLog.Print("Level System: Setting monster base XP", LogType.Message);
            monsterXPTable.Clear();
            string[] monsterXPstring = Main.monsterXpTable.Value.Split(',');
            foreach(string s in monsterXPstring)
            {
                int value;
                string[] data = s.Split(':');
                string key = data[0].Replace("\n", "").Replace(" ", "");
                int.TryParse(data[1], out value);
                monsterXPTable[key] = value;
                ConsoleLog.Print("-> " + key + ": " + value + "xp");
            }
        }

        public static void UpdatePlayerXPTable()
        {
            ConsoleLog.Print("Level System: Setting player level XP requirements", LogType.Message);
            List<int> playerLevels = new List<int>();

            string[] playerXPstring = Main.playerXpTable.Value.Split(',');
            int counter = 1;
            foreach (string s in playerXPstring)
            {
                int value;
                string[] data = s.Split(':');
                int.TryParse(data[1], out value);
                playerLevels.Add(value);
                ConsoleLog.Print("-> Level " + counter + ": " + value + "xp");
                counter++;
            }
            playerXPTable = playerLevels.ToArray();
        }

        public static int GetMonsterXP(string name)
        {
            name = name.Replace("(Clone)", "");
            return monsterXPTable.ContainsKey(name) ? monsterXPTable[name] : 0;
        }
    }
}
