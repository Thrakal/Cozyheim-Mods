using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Jotunn.Utils;
using System.Collections.Generic;

namespace CozyheimModDependencies
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]

    // Mods that are NOT allowed                                    // --- Not Allowed Mods ---
    //                                                                 ________________________
    [BepInIncompatibility("NoSleepingLoadingScreen")]               // -> No Sleeping Loading Screen


    // Required mods                                                // --- Required Mods ---
    //                                                                 _____________________
    [BepInDependency("randyknapp.mods.epicloot")]                   // -> Epic Loot
    [BepInDependency("randyknapp.mods.equipmentandquickslots")]     // -> Equipment And Quick Slots
    [BepInDependency("aedenthorn.CraftFromContainers")]             // -> Craft From Containers
    [BepInDependency("aedenthorn.DiscardInventoryItem")]            // -> Discard Inventory Items
    [BepInDependency("aedenthorn.Regeneration")]                    // -> Regeneration
    [BepInDependency("aedenthorn.InstantMonsterDrop")]              // -> Instant Monster Drop
    [BepInDependency("uk.co.oliapps.valheim.noraindamage")]         // -> No Rain Damage
    [BepInDependency("BepIn.Sarcen.FarmGrid")]                      // -> Farm Grid
    [BepInDependency("SKK.DisableRandomEvents")]                    // -> Disable Random Events
    [BepInDependency("yardik.SnapPointsMadeEasy")]                  // -> Snap Points Made Easy
    [BepInDependency("advize.PlantEverything")]                     // -> Plant Everything
    [BepInDependency("goldenrevolver.quick_stack_store")]           // -> Quick Stack Store
    [BepInDependency("smallo.mods.fermenterutilities")]             // -> Fermenter Utilities
    [BepInDependency("OhhLoz-HoneyPlus")]                           // -> Honey Plus
    [BepInDependency("com.Bento.MissingPieces")]                    // -> Missing Pieces
    [BepInDependency("ishid4.mods.betterarchery")]                  // -> Better Archery
    [BepInDependency("com.undeadbits.valheimmods.portalstation")]   // -> Portal Station
    [BepInDependency("dk.thrakal.CozyheimTweaks")]                  // -> Cozyheim Tweaks
    [BepInDependency("dk.thrakal.BurnBabyBurn")]                    // -> Cozyheim
    [BepInDependency("dk.thrakal.AddingNPCs")]                      // -> Cozyheim
    [BepInDependency("dk.thrakal.DifficultyScaler")]                // -> Cozyheim
    [BepInDependency("dk.thrakal.DeathChange")]                     // -> Cozyheim

    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "CozyheimModDependencies";
        internal const string version = "0.0.5";
        internal const string GUID = "dk.thrakal." + modName;

        void Awake()
        {
            // Dictionary<string, PluginInfo> plugins = Chainloader.PluginInfos;
            // Jotunn.Logger.LogMessage("Number of plugins loaded: " + plugins.Count);
        }
    }
}
