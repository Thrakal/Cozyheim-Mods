using Jotunn.Entities;
using Jotunn.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cozyheim.LevelingSystem
{
    internal class UIManager : MonoBehaviour
    {
        // XP Bar
        private Text levelText, levelTextShadow;
        private Text levelUpText, levelUpTextShadow;
        private Text xpText, xpTextShadow;
        private Image xpFill;
        private CanvasGroup xpBarGroup, levelUpGroup;
        private RectTransform xpBarRect, levelTextRect, xpBarContainerRect;

        public int playerXP = 0;
        public int playerLevel = 1;

        private bool gainedNewLevel = false;
        private float smoothTime = 0.15f;
        private float smoothMaxSpeed = 10f;
        private float xpFillTarget = 0f;
        private float xpFillVel;

        // Floating text
        private static GameObject xpTextFloating;
        private static GameObject levelUpEffect;

        // Skills UI
        public CanvasGroup skillsUI;
        public Text remainingPoints;
        public Button closeButton;
        public Button resetPointsButton;
        public RectTransform viewportContent;
        public ScrollRect skillsScrollRect;
        public Scrollbar skillsScrollbar;

        public GameObject skillPrefab;

        public bool skillsUIVisible = false;

        public static CustomRPC rpc_AddExperienceMonster;
        public static CustomRPC rpc_AddExperience;
        public static CustomRPC rpc_ReloadConfig;
        public static CustomRPC rpc_LevelUpEffect;
        public static CustomRPC rpc_Test;

        public static UIManager Instance;

        private List<GameObject> skillGOsInUi = new List<GameObject>();

        public static void Init()
        {
            rpc_AddExperienceMonster = NetworkManager.Instance.AddRPC("AddExperienceMonster", RPC_AddExperienceMonster, RPC_AddExperienceMonster);
            rpc_AddExperience = NetworkManager.Instance.AddRPC("AddExperience", RPC_AddExperience, RPC_AddExperience);
            rpc_ReloadConfig = NetworkManager.Instance.AddRPC("ReloadConfig", RPC_ReloadConfig, RPC_ReloadConfig);
            rpc_LevelUpEffect = NetworkManager.Instance.AddRPC("LevelUpEffect", RPC_LevelUpEffect, RPC_LevelUpEffect);
            rpc_Test = NetworkManager.Instance.AddRPC("Test", RPC_Test, RPC_Test);
        }

        public void CallRPCTest()
        {
            rpc_Test.SendPackage(ZRoutedRpc.Everybody, new ZPackage());
        }

        private static IEnumerator RPC_Test(long sender, ZPackage package)
        {
            ConsoleLog.Print("RPC Test called!", LogType.Message);
            yield return null;
        }

        void Awake()
        {
            Instance = this;

            levelText = transform.Find("XP Bar/LevelText").GetComponent<Text>();
            levelTextShadow = transform.Find("XP Bar/LevelText/Shadow").GetComponent<Text>();
            xpText = transform.Find("XP Bar/XP Bar/XPText").GetComponent<Text>();
            xpTextShadow = transform.Find("XP Bar/XP Bar/XPText/Shadow").GetComponent<Text>();
            xpFill = transform.Find("XP Bar/XP Bar/XPFill").GetComponent<Image>();

            xpBarGroup = transform.Find("XP Bar").GetComponent<CanvasGroup>();
            xpBarContainerRect = transform.Find("XP Bar").GetComponent<RectTransform>();
            xpBarRect = transform.Find("XP Bar/XP Bar").GetComponent<RectTransform>();
            levelTextRect = transform.Find("XP Bar/LevelText").GetComponent<RectTransform>();

            levelUpText = transform.Find("LevelUp Pop-Up/LevelUpText").GetComponent<Text>();
            levelUpTextShadow = transform.Find("LevelUp Pop-Up/LevelUpText/Shadow").GetComponent<Text>();
            levelUpGroup = transform.Find("LevelUp Pop-Up").GetComponent<CanvasGroup>();

            // Skills UI
            skillsUI = transform.Find("Skills UI").GetComponent<CanvasGroup>();
            remainingPoints = transform.Find("Skills UI/Remaining Points").GetComponent<Text>();
            closeButton = transform.Find("Skills UI/Close Menu").GetComponent<Button>();
            resetPointsButton = transform.Find("Skills UI/Reset Skills Button").GetComponent<Button>();
            viewportContent = transform.Find("Skills UI/Scroll View/Viewport/Content").GetComponent<RectTransform>();
            skillsScrollRect = transform.Find("Skills UI").GetComponent<ScrollRect>();
            skillsScrollbar = transform.Find("Skills UI/Scrollbar").GetComponent<Scrollbar>();

            skillPrefab = PrefabManager.Instance.GetPrefab("SkillUI");
        }

        void Start()
        {
            ToggleSkillsUI(false);

            playerLevel = XPManager.Instance.GetPlayerLevel();
            playerXP = XPManager.Instance.GetPlayerXP();

            XPTable.UpdateMonsterXPTable();
            XPTable.UpdatePlayerXPTable();
            XPTable.UpdatePickableXPTable();
            XPTable.UpdateMiningXPTable();
            XPTable.UpdateWoodcuttingXPTable();

            levelUpGroup.alpha = 0f;

            xpTextFloating = PrefabManager.Instance.GetPrefab("XPText");
            levelUpEffect = PrefabManager.Instance.GetPrefab("LevelUpEffectNew");

            // Set the size and position of the xp bar / level text
            RepositionXPBar();

            UpdateUI(true);
            StartCoroutine(XPBarFadeIn(3f));
        }

        void RepositionXPBar()
        {
            Vector2 tempSize = xpBarRect.sizeDelta;
            tempSize.x *= Main.xpBarSize.Value / 100f;
            xpBarRect.sizeDelta = tempSize;

            if (Main.xpBarLevelTextPosition.Value == Main.Position.Below)
            {
                Vector2 tempPos = levelTextRect.anchoredPosition;
                tempPos.y *= -1f;
                levelTextRect.anchoredPosition = tempPos;
            }

            xpBarContainerRect.anchoredPosition = Main.xpBarPosition.Value;
        }

        void Update()
        {
            if(!IsPlayerMaxLevel()) {
                float tempFillTarget = gainedNewLevel ? 1f : xpFillTarget;
                xpFill.fillAmount = Mathf.SmoothDamp(xpFill.fillAmount, tempFillTarget, ref xpFillVel, smoothTime, smoothMaxSpeed);

                if (gainedNewLevel && xpFill.fillAmount > 0.99f)
                {
                    gainedNewLevel = false;
                    xpFill.fillAmount = 0f;
                }
            }

            if(skillsUIVisible)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ToggleSkillsUI(false);
                }
            }
        }

        void ClearSkillUIGameObjects()
        {
            foreach(GameObject go in skillGOsInUi)
            {
                Destroy(go);
            }

            skillGOsInUi.Clear();
        }

        public void ReloadSkillsUI()
        {
            ClearSkillUIGameObjects();
            CreateSkillUI();
            UpdateUIInformation();
        }

        void CreateSkillUI()
        {
            skillsScrollRect.verticalScrollbar = Main.showScrollbar.Value ? skillsScrollbar : null;
            skillsScrollbar.gameObject.SetActive(Main.showScrollbar.Value);

            closeButton.onClick.RemoveAllListeners();
            resetPointsButton.onClick.RemoveAllListeners();

            int skillsCount = SkillManager.Instance.GetTotalSkillsCount();

            for (int i = 0; i < (int)SkillType.EndOfEnum; i++)
            {
                SkillBase skill = SkillManager.Instance.GetSkillByIndex(i);
                if(skill != null) {
                    GameObject newSkill = Instantiate(skillPrefab, viewportContent);
                    skillGOsInUi.Add(newSkill);

                    SkillOption option = newSkill.gameObject.AddComponent<SkillOption>();
                    option.Setup();
                    skill.SetSkillUI(option);
                }
            }

            int skillsCeil = (int) Mathf.Ceil((float)skillsCount / 3f);

            Vector2 tempSize = viewportContent.sizeDelta;
            tempSize.y = 215f * skillsCeil;
            viewportContent.sizeDelta = tempSize;

            // Add listener for the buttons
            closeButton.onClick.AddListener(delegate () {
                ToggleSkillsUI(false);
            });

            resetPointsButton.onClick.AddListener(delegate ()
            {
                SkillManager.Instance.SkillResetAll();
            });
        }

        public void ToggleSkillsUI(bool value)
        {
            skillsUI.alpha = value ? 1f : 0f;
            skillsUI.interactable = value;
            skillsUI.blocksRaycasts = value;

            skillsUIVisible = value;
            viewportContent.anchoredPosition3D = Vector3.zero;
            GUIManager.BlockInput(value);

            if(value)
            {
                rpc_ReloadConfig.SendPackage(ZRoutedRpc.Everybody, new ZPackage());
                ClearSkillUIGameObjects();
                CreateSkillUI();
                UpdateUIInformation();
            } else
            {
                ClearSkillUIGameObjects();
            }
        }

        void UpdateUIInformation()
        {
            ConsoleLog.Print("UpdateUIInformation called");
            remainingPoints.text = "Remaining points: " + SkillManager.Instance.unspendPoints;
            SkillManager.Instance.UpdateAllSkillInformation();
        }

        private bool xpBarVisible = false;

        public void FadeInXPBar(float fadeTime)
        {
            if(!xpBarVisible)
            {
                StartCoroutine(XPBarFadeIn(fadeTime));
            }
        }

        IEnumerator XPBarFadeIn(float fadeTime)
        {
            xpBarVisible = true;
            ConsoleLog.Print("Showing xp bar!");
            xpBarGroup.alpha = 0f;

            for(float f = 0f; f < fadeTime; f += Time.deltaTime)
            {
                float perc = f / fadeTime;
                xpBarGroup.alpha = perc;
                yield return null;
            }

            xpBarGroup.alpha = 1f;
        }

        public void FadeOutXPBar(float fadeTime)
        {
            if (xpBarVisible)
            {
                StartCoroutine(XPBarFadeOut(fadeTime));
            }
        }

        IEnumerator XPBarFadeOut(float fadeTime)
        {
            xpBarVisible = false;

            ConsoleLog.Print("Removing xp bar!");
            xpBarGroup.alpha = 1f;

            for (float f = 0f; f < fadeTime; f += Time.deltaTime)
            {
                float perc = f / fadeTime;
                xpBarGroup.alpha = 1 - perc;
                yield return null;
            }

            xpBarGroup.alpha = 0f;
        }

        private static IEnumerator RPC_LevelUpEffect(long sender, ZPackage package)
        {
            if (Main.levelUpVFX.Value)
            {
                ConsoleLog.Print(Main.levelUpVFX.Value.ToString());

                if (Player.m_localPlayer != null)
                {
                    long playerID = package.ReadLong();

                    Collider[] colls = Physics.OverlapSphere(Player.m_localPlayer.transform.position, 40f);
                    foreach (Collider coll in colls)
                    {
                        Player player = coll.GetComponent<Player>();
                        if (player == null) { 
                            continue;
                        }

                        if (player.GetPlayerID() != playerID) {
                            continue;
                        }

                        GameObject newEffect = Instantiate(levelUpEffect, player.GetCenterPoint(), Quaternion.identity, player.transform);
                        Destroy(newEffect, 6f);
                        break;
                    }
                }
            }

            yield return null;
        }

        public void LevelUpVFX()
        {
            XPManager.Instance.PlayerLevelUp(Player.m_localPlayer.GetPlayerID());
        }

        IEnumerator LevelUpFadeIn()
        {
            LevelUpVFX();

            levelUpGroup.alpha = 0f;
            levelUpText.text = "Level " + playerLevel;
            levelUpTextShadow.text = levelUpText.text;

            float fadeInTime = 1f;
            float fadeOutTime = 2f;
            float waitTime = 3f;

            for (float f = 0f; f < fadeInTime; f += Time.deltaTime)
            {
                float perc = f / fadeInTime;
                levelUpGroup.alpha = perc;
                yield return null;
            }

            levelUpGroup.alpha = 1f;
            yield return new WaitForSeconds(waitTime);

            for (float f = 0f; f < fadeOutTime; f += Time.deltaTime)
            {
                float perc = f / fadeOutTime;
                levelUpGroup.alpha = 1 - perc;
                yield return null;
            }

            levelUpGroup.alpha = 0f;
        }

        private static IEnumerator RPC_ReloadConfig(long sender, ZPackage package)
        {
            if(ZNet.instance.IsServer())
            {
                ConsoleLog.ReloadConfig();
            }

            yield return null;
        }

        private static IEnumerator RPC_AddExperience(long sender, ZPackage package)
        {
            ConsoleLog.Print("Received Expereience");

            long playerID = package.ReadLong();
            int awardedXP = package.ReadInt();

            if (Player.m_localPlayer != null)
            {

                if (playerID == Player.m_localPlayer.GetPlayerID())
                {

                    Instance.AddExperience(awardedXP);
                    SpawnFloatingXPText(awardedXP);
                }
            }

            yield return null;
        }

        private static IEnumerator RPC_AddExperienceMonster(long sender, ZPackage package)
        {
            ConsoleLog.Print("Received Expereience Monster");

            int awardedXP = package.ReadInt();
            int monsterLevelBonusXp = package.ReadInt();
            int restedBonusXp = package.ReadInt();
            long playerID = package.ReadLong();
 
            if(Player.m_localPlayer != null)
            {
                if(playerID == Player.m_localPlayer.GetPlayerID())
                {
                    int totalXpGained = 0;

                    StatusEffect SERested = Player.m_localPlayer.GetSEMan().GetStatusEffect("Rested");

                    if (awardedXP > 0)
                    {
                        Instance.AddExperience(awardedXP);
                        totalXpGained += awardedXP;
                    }

                    if (monsterLevelBonusXp > 0)
                    {
                        Instance.AddExperience(monsterLevelBonusXp, XPType.MonsterLevel);
                        totalXpGained += monsterLevelBonusXp;
                    }

                    if (SERested != null)
                    {
                        Instance.AddExperience(restedBonusXp, XPType.Rested);
                        totalXpGained += restedBonusXp;
                    }

                    SpawnFloatingXPText(totalXpGained);
                }
            }

            yield return null;
        }

        private static void SpawnFloatingXPText(int totalXpGained)
        {
            if (totalXpGained > 0 && Main.displayXPFloatingText.Value)
            {
                Vector3 spawnPosition = Player.m_localPlayer.GetTopPoint() + new Vector3(Random.Range(-1f, 1f), Random.Range(0, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(-0.2f, 0.2f);

                XPText xpText = Instantiate(xpTextFloating, spawnPosition, Quaternion.identity).GetComponent<XPText>();
                xpText.XPGained(totalXpGained);
            }
        }

        public void AddExperience(int xp, XPType type = XPType.Regular)
        {
            if (!IsPlayerMaxLevel() && xp > 0)
            {
                if (Main.displayXPInCorner.Value)
                {
                    switch (type)
                    {
                        case XPType.Regular:
                            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "You gained +" + xp + "xp");
                            break;
                        case XPType.MonsterLevel:
                            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "-> Monster level bonus: +" + xp + "xp");
                            break;
                        case XPType.Rested:
                            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "-> Rested bonus: +" + xp + "xp");
                            break;
                    }
                }

                playerXP += xp;

                while (playerXP >= XPTable.playerXPTable[playerLevel - 1])
                {
                    gainedNewLevel = true;
                    playerXP -= XPTable.playerXPTable[playerLevel - 1];
                    playerLevel++;

                    XPManager.Instance.SetPlayerLevel(playerLevel);
                    StartCoroutine(LevelUpFadeIn());

                    SkillManager.Instance.UpdateUnspendPoints();

                    if (IsPlayerMaxLevel())
                    {
                        break;
                    }
                }

                XPManager.Instance.SetPlayerXP(playerXP);

                UpdateUI();

                XPManager.Instance.SavePlayerLevel();
                XPManager.Instance.SavePlayerXP();
            }
        }

        public void UpdateUI(bool instantUpdate = false)
        {
            UpdateLevelText();

            if (IsPlayerMaxLevel())
            {
                SetXPText("Max Level");
                playerXP = 0;
                xpFill.fillAmount = 1f;
                return;
            }

            float xpToNextLevel = XPTable.playerXPTable[playerLevel - 1];
            float xpPercentage = playerXP / xpToNextLevel;

            xpFillTarget = xpPercentage;

            string xpString = "";

            xpString += Main.showXp.Value ? playerXP.ToString() : "";
            if(Main.showXp.Value)
            {
                xpString += Main.showRequiredXp.Value ? " / " + xpToNextLevel : "";
                xpString += Main.showPercentageXP.Value ? " (" + (xpPercentage * 100).ToString("N0") + "%)" : "";
            } else
            {
                xpString += Main.showPercentageXP.Value ? (xpPercentage * 100).ToString("N0") + "%" : "";
            }

            if(instantUpdate)
            {
                xpFill.fillAmount = xpFillTarget;
            }

            SetXPText(xpString);
        }

        public void SetXPText(string text)
        {
            text = Localization.instance.Localize(text);
            xpText.text = text;
            xpTextShadow.text = text;
        }

        public void UpdateLevelText()
        {
            levelText.text = Main.showLevel.Value ? "Level " + playerLevel : "";
            levelTextShadow.text = levelText.text;
        }

        public bool IsPlayerMaxLevel()
        {
            return playerLevel > XPTable.playerXPTable.Length;
        }

        public void DestroySelf()
        {
            XPManager.Instance.SetPlayerLevel(playerLevel);
            XPManager.Instance.SetPlayerXP(playerXP);
            Destroy(gameObject);
        }

    }
}

public enum XPType
{
    Regular,
    MonsterLevel,
    Rested
}