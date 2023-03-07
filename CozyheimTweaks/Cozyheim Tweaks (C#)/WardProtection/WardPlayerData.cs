using Jotunn.Managers;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CozyheimTweaks
{
    public class WardPlayerData
    {
        public string username;
        public long userID;
        public int maxWards;
        public int currentWards;
        public List<WhitelistUser> whitelistUsers;

        public WardPlayerData(string username, long userID, int maxWards, int currentWards)
        {
            this.username = username;
            this.userID = userID;
            this.maxWards = maxWards;
            this.currentWards = currentWards;
            whitelistUsers = new List<WhitelistUser>();
        }

        public WardPlayerData(JSONObject jsonObject)
        {
            username = jsonObject["username"];
            userID = jsonObject["userID"].AsLong;
            maxWards = jsonObject["maxWards"].AsInt;
            currentWards = jsonObject["currentWards"].AsInt;

            whitelistUsers = new List<WhitelistUser>();
            JSONArray whitelistArray = jsonObject["whitelist"].AsArray;
            foreach (JSONObject user in whitelistArray)
            {
                whitelistUsers.Add(new WhitelistUser(user["userID"].AsLong, (WhitelistStatus)user["status"].AsInt));
            }
        }

        public JSONObject GetPlayerJSON()
        {
            JSONObject json = new JSONObject();
            json.Add("username", username);
            json.Add("userID", userID);
            json.Add("maxWards", maxWards);
            json.Add("currentWards", currentWards);

            JSONArray whitelistJson = new JSONArray();
            foreach (WhitelistUser user in whitelistUsers)
            {
                JSONObject whitelist = new JSONObject();
                whitelist["userID"] = user.userID;
                whitelist["status"] = (int) user.status;
                whitelistJson.Add(whitelist);
            }
            json.Add("whitelist", whitelistJson);

            return json;
        }

        public void UpdatePlayerData(JSONObject jsonObject)
        {
            username = jsonObject["username"];
            userID = jsonObject["userID"].AsLong;
            maxWards = jsonObject["maxWards"].AsInt;
            currentWards = jsonObject["currentWards"].AsInt;

            whitelistUsers = new List<WhitelistUser>();
            JSONArray whitelistArray = jsonObject["whitelist"].AsArray;
            foreach (JSONObject user in whitelistArray)
            {
                whitelistUsers.Add(new WhitelistUser(user["userID"].AsLong, (WhitelistStatus)user["status"].AsInt));
            }
        }

        public bool WhitelistAddUser(long userid, WhitelistStatus status = WhitelistStatus.Everything)
        {
            foreach(WhitelistUser user in whitelistUsers)
            {
                if(user.userID == userid)
                {
                    return false;
                }
            }

            whitelistUsers.Add(new WhitelistUser(userid, status));
            return true;
        }

        public bool WhitelistUpdateStatus(long userid, WhitelistStatus status)
        {
            foreach (WhitelistUser user in whitelistUsers)
            {
                if (user.userID == userid)
                {
                    user.status = status;
                    return true;
                }
            }
        
            return false;
        }

        public bool WhitelistRemoveUser(long userid)
        {
            foreach (WhitelistUser user in whitelistUsers)
            {
                if (user.userID == userid)
                {
                    whitelistUsers.Remove(user);
                    return true;
                }
            }

            return false;
        }

        public bool WardBuild()
        {
            if(SynchronizationManager.Instance.PlayerIsAdmin && WardProtection.adminOverrules.Value)
            {
                currentWards++;
                return true;
            }

            if (currentWards >= maxWards)
            {
                return false;
            }

            currentWards++;
            return true;
        }

        public bool WardDestroy()
        {
            if (SynchronizationManager.Instance.PlayerIsAdmin && WardProtection.adminOverrules.Value)
            {
                currentWards--;
                if(currentWards < 0)
                {
                    currentWards = 0;
                }
                return true;
            }

            if (currentWards <= 0)
            {
                return false;
            }

            currentWards--;
            return true;
        }

        public WhitelistStatus GetWhitelistStatus(long userid)
        {
            foreach(WhitelistUser user in whitelistUsers)
            {
                if(user.userID == userid)
                {
                    return user.status;
                }
            }

            return WhitelistStatus.None;
        }

        public class WhitelistUser
        {
            public long userID;
            public WhitelistStatus status;

            public WhitelistUser(long userID, WhitelistStatus status)
            {
                this.userID = userID;
                this.status = status;
            }
        }
    }
}

public enum WhitelistStatus
{
    None = 0,
    InteractOnly = 1,
    BuildOnly = 2,
    Everything = 3
}