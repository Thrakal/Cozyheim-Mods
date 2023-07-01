using UnityEngine;

namespace Cozyheim.API {

    public class DifficultyScalerBase : MonoBehaviour {

        private float startHealth = 0f;
        private int level = 0;

        private float damageMultiplier = 0f;
        private float healthMultiplier = 0f;
        private float biomeMultiplier = 0f;
        private float nightMultiplier = 0f;
        private float bossKillMultiplier = 0f;
        private float starMultiplier = 0f;

        public void SetAllMultipliers(float health, float damage, float biome, float night, float bossKill, float star) {
            healthMultiplier = health;
            damageMultiplier = damage;
            biomeMultiplier = biome;
            nightMultiplier = night;
            bossKillMultiplier = bossKill;
            starMultiplier = star;
        }

        public void SetLevel(int value) {
            level = value;
        }

        public void SetStartHealth(float value) {
            startHealth = value;
        }

        public void SetHealthMultiplier(float value) {
            healthMultiplier = value;
        }

        public void SetDamageMultiplier(float value) {
            damageMultiplier = value;
        }

        public void SetBiomeMultiplier(float value) {
            biomeMultiplier = value;
        }

        public void SetNightMultiplier(float value) {
            nightMultiplier = value;
        }

        public void SetBossKillMultiplier(float value) {
            bossKillMultiplier = value;
        }

        public void SetStarMultiplier(float value) {
            starMultiplier = value;
        }

        public int GetLevel() {
            return level;
        }

        public float GetHealthMultiplier() {
            return healthMultiplier;
        }

        public float GetStartHealth() {
            return startHealth;
        }

        public float GetDamageMultiplier() {
            return damageMultiplier;
        }

        public float GetBiomeMultiplier() {
            return biomeMultiplier;
        }

        public float GetNightMultiplier() {
            return nightMultiplier;
        }

        public float GetBossKillMultiplier() {
            return bossKillMultiplier;
        }

        public float GetStarMultiplier() {
            return starMultiplier;
        }

        public float GetTotalDamageMultiplier() {
            return damageMultiplier + biomeMultiplier + nightMultiplier + bossKillMultiplier + starMultiplier;
        }

        public float GetTotalHealthMultiplier() {
            return healthMultiplier + biomeMultiplier + nightMultiplier + bossKillMultiplier + starMultiplier;
        }
    }

}

