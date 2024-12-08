using System;
using GameDevTV.Utils;
using RPG.Core;
using GameDevTV.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        // Needed in order to serialize the event.
        [Serializable]
        public class TakeDamageEvent : UnityEvent<float> { }

        [SerializeField]
        TakeDamageEvent takeDamage = null;
#pragma warning disable 649
        [SerializeField] UnityEvent onDie;
#pragma warning restore 649

        LazyValue<float> healthPoints;

        bool isDead = false;
        BaseStats baseStats;

        void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        void Start()
        {
            healthPoints.ForceInit();
        }

        void OnEnable()
        {
            baseStats.onLevelUp += LeveledUp;
        }

        void OnDisable()
        {
            baseStats.onLevelUp -= LeveledUp;
        }

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (isDead) return;

            float actualDamage = Mathf.Min(healthPoints.value, damage);
            takeDamage.Invoke(actualDamage);

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            CheckForDeath(instigator, true);
        }

        void Die(GameObject instigator)
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            AwardXP(instigator);
        }

        void AwardXP(GameObject instigator)
        {
            if (instigator == null) return;

            var experiencePoints = GetComponent<BaseStats>().GetExperienceReward();
            Experience xp = instigator.GetComponent<Experience>();

            if (xp == null) return;

            xp.GainExperience(experiencePoints);
        }

        void LeveledUp()
        {
            float newBaseHealth = baseStats.GetHealth();
            healthPoints.value += newBaseHealth - maxValue;
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = Mathf.Max(0, (float)state);
            CheckForDeath(null, false);
        }

        public bool isAlive
        {
            get { return !isDead; }
        }

        public void CheckForDeath(GameObject instigator, bool triggerEvent)
        {
            if (healthPoints.value == 0)
            {
                if (triggerEvent) onDie.Invoke();

                Die(instigator);
            }
        }

        public void Heal(float healingValue)
        {
            print($"Healing {healingValue}");
            if (isDead) return;

            float baseHealth = GetInitialHealth();
            //float actualHealing = Mathf.Min(baseHealth - healthPoints.value, healingValue);
            //takeDamage.Invoke(actualHealing);
            healthPoints.value = Mathf.Min(healthPoints.value + healingValue, baseHealth);
        }

        public float GetFractionalPercentage()
        {
            // TODO: there's a null reference exception here when switching scenes and waiting
            return healthPoints.value / baseStats.GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * GetFractionalPercentage();
        }

        public float value
        {
            get { return healthPoints.value; }
        }

        public float maxValue
        {
            get { return baseStats.GetStat(Stat.Health); }
        }
    }
}