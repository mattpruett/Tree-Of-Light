using System;
using System.Collections;
using GameDevTV.Utils;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 100)]
        [SerializeField] int startingLevel = 1;

        [SerializeField] CharacterClass characterClass = CharacterClass.None;
        [SerializeField] ClassProgression classProgression = null;
        [SerializeField] GameObject levelUpEffect = null;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;
        Experience experience;

        void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        void Start()
        {
            currentLevel.ForceInit();
        }

        void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += ExperienceGained;
            }
        }

        void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= ExperienceGained;
            }
        }

        void ExperienceGained()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUp();
            }
        }

        void LevelUp()
        {
            PlayLevelUpEffect();
            onLevelUp();
        }

        private void PlayLevelUpEffect()
        {
            if (levelUpEffect == null) return;

            Instantiate(levelUpEffect, transform);
        }

        public float GetHealth()
        {
            if (classProgression == null) return 0;

            return classProgression.GetHealth(characterClass, currentLevel.value);
        }

        public float GetStat(Stat stat)
        {
            if (classProgression == null) return 0f;

            return (GetBaseStat(stat)
                + GetStatBonuses(stat))
                * (1 + (GetPercentageModifer(stat) / 100));
        }

        private float GetBaseStat(Stat stat)
        {
            return classProgression.GetStat(stat, characterClass, currentLevel.value);
        }

        private float GetPercentageModifer(Stat stat)
        {
            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetStatBonuses(Stat stat)
        {
            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        public float GetExperienceReward()
        {
            return GetStat(Stat.Experience);
        }

        public int Level
        {
            get
            {
                if (currentLevel.value < 1) CalculateLevel();
                return currentLevel.value;
            }
        }

        private int CalculateLevel()
        {
            Experience xp = GetComponent<Experience>();

            if (xp == null) return startingLevel;

            int maxLevel = classProgression.GetClassLevelCount(characterClass, Stat.ExperienceToLevelUp);

            float experiencePoints = xp.value;

            float[] levelProgression = classProgression.GetLevelProgression(characterClass);

            if (levelProgression == null || levelProgression.Length == 0) return startingLevel;

            int level = 1;
            while (level <= maxLevel && experiencePoints >= levelProgression[level - 1])
            {
                level++;
            }
            return level;
        }
    }
}
