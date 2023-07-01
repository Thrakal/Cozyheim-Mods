using BepInEx.Configuration;
using ServerSync;
using System.Collections.Generic;
using static ItemDrop.ItemData;

namespace Cozyheim.AdjustUpgradeStats {
    internal class ConfigSettings {
        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        internal static List<Stats> gearStats = new List<Stats>() {
            new Stats() {
                name = "One-Handed Weapons",
                category = "01 - One-Handed Weapons",
                types = new List<ItemType>() {
                    ItemType.OneHandedWeapon
                }
            },
            new Stats() {
                name = "Two-Handed Weapons",
                category = "02 - Two-Handed Weapons",
                types = new List<ItemType>() {
                    ItemType.TwoHandedWeapon,
                    ItemType.TwoHandedWeaponLeft
                }
            },
            new Stats() {
                name = "Bows & Crossbows",
                category = "03 - Bows & Crossbows",
                types = new List<ItemType>() {
                    ItemType.Bow
                }
            },
            new Stats() {
                name = "Arrows & Bolts",
                category = "04 - Arrows & Bolts",
                types = new List<ItemType>() {
                    ItemType.Ammo
                }
            },
            new Stats() {
                name = "Hands",
                category = "05 - Hands",
                types = new List<ItemType>() {
                    ItemType.Hands
                }
            }
        };

        public static void Init() {
            // Assigning config entries
            modEnabled = CreateConfigEntry("00 - General", "ModEnabled", true, "Enable this mod", true);
            debugEnabled = CreateConfigEntry("00 - General", "DebugEnabled", false, "Display debug messages in the console", false);

            foreach(Stats stats in gearStats) {
                stats.Init();
            }
        }

        #region CreateConfigEntry Wrapper
        public static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true) {
            ConfigEntry<T> configEntry = Main.configFile.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = Main.configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        public static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => CreateConfigEntry(group, name, value, new ConfigDescription(description), synchronizedSetting);
        #endregion
    }

    public enum UpgradeType {
        None,
        Percentage,
        PercentageOfBase
    }
}
