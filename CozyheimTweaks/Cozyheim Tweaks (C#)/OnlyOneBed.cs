using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace CozyheimTweaks
{
    [HarmonyPatch]
    internal class OnlyOneBed
    {
        internal static ConfigFile patchConfigFile;

        internal static ConfigEntry<bool> enable;
        internal static ConfigEntry<float> noBedRadius;

        internal const string patchName = "OnlyOneBed";

        internal static void Init()
        {
            patchConfigFile = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/" + patchName + ".cfg", true);
            patchConfigFile.SaveOnConfigSet = true;

            enable = Main.CreateConfigEntry("General", "enableMod", true, "Allows this mod to be used", patchConfigFile);
            noBedRadius = Main.CreateConfigEntry("General", "noBedRadius", 300f, "Distance from a current claimed bed, before you can claim another", patchConfigFile);
        }

        // ref bool __result = returned value of the original method
        // ref <T> __instance = reference to original class (this)
        // object[] __args = original parameters
        // -> Example: Player player = (Player) __args[0];

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Bed), "Interact")]
        static bool Bed_Interact_Prefix(ref bool __result, Bed __instance)
        {
            if (enable.Value) {
                bool foundOwnBedNearby = false;
                Collider[] colls = Physics.OverlapSphere(__instance.transform.position, noBedRadius.Value);
                foreach(Collider coll in colls)
                {
                    Bed bed = coll.GetComponent<Bed>();
                    if (bed != null && bed != __instance) {
                        // Check if the bed found is the current spawnpoint for the user
                        if(bed.IsCurrent())
                        {
                            float distanceToBed = Vector3.Distance(bed.GetSpawnPoint(), __instance.GetSpawnPoint());
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You already got a bed nearby\n(" + distanceToBed.ToString("N0") + " of " + noBedRadius.Value.ToString("N0") + " meters away)");
                            foundOwnBedNearby = true;
                            break;
                        }
                    }
                }

                if(foundOwnBedNearby)
                {
                    __result = false;
                    return false;
                }
            }

            return true;
        }
    }
}
