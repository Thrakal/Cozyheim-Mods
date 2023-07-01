using UnityEngine;

namespace Cozyheim.API {

    public class DifficultyScalerComp : MonoBehaviour {

        private float damageMultiplier = 1f;
        private float healthMultiplier = 1f;
        private float biomeMultiplier = 1f;
        private float nightMultiplier = 1f;
        private float bossKillMultiplier = 1f;

        public void SetAllMultipliers(float health, float damage, float biome, float night, float bossKill) {
            healthMultiplier = health;
            damageMultiplier = damage;
            biomeMultiplier = biome;
            nightMultiplier = night;
            bossKillMultiplier = bossKill;
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

        public float GetHealthMultiplier() {
            return healthMultiplier;
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

    }
}

