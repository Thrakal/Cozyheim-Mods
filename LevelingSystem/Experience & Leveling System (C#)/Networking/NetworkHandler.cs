using Jotunn.Entities;
using Jotunn.Managers;
using System.Collections;

namespace Cozyheim.LevelingSystem
{
    internal class NetworkHandler
    {
        public static void Init()
        {
            RPC_LevelUpVFX.SetupRPC();
        }

        public static void LevelUpVFX()
        {
            ConsoleLog.Print("LevelUpVFX");
            ZPackage newPackage = new ZPackage();
            newPackage.Write(GetLocalPlayerID());
            RPC_LevelUpVFX.levelUpVFX.SendPackage(ZRoutedRpc.Everybody, newPackage);
        }


        public static bool IsServer()
        {
            return ZNet.instance.IsServer();
        }

        public static bool IsAdmin()
        {
            if (!SynchronizationManager.Instance.PlayerIsAdmin)
            {
                return false;
            }

            return true;
        }

        private static long GetLocalPlayerID()
        {
            return Player.m_localPlayer.GetPlayerID();
        }
    }
}
