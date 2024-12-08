using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace RPG.Stats
{

    [CreateAssetMenu(fileName = "ClassProgression", menuName = "Stats/New Progression", order = 0)]
    public class ClassProgression : ScriptableObject
    {
        [SerializeField] ProgressionPlan[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> progressionLookup = null;

        void BuildLookup()
        {
            if (progressionLookup != null) return;

            progressionLookup = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            // Perhaps we can replace this with a LINQ statement in the future.
            foreach (ProgressionPlan plan in characterClasses)
            {
                if (progressionLookup.ContainsKey(plan.Class)) continue;

                progressionLookup.Add(plan.Class, new Dictionary<Stat, float[]>());

                Dictionary<Stat, float[]> statsPerLevel = progressionLookup[plan.Class];

                for (int i = 0; i < plan.levels.Length; i++)
                {
                    foreach (StatValue statValue in plan.levels[i].stats)
                    {
                        if (!statsPerLevel.ContainsKey(statValue.stat))
                        {
                            statsPerLevel.Add(statValue.stat, new float[plan.levels.Length]);
                        }
                        statsPerLevel[statValue.stat][i] = statValue.value;
                    }
                }
            }
            //DebugLookupDictionary();
        }

        private float[] GetStatArray(Stat stat, CharacterClass charClass)
        {
            BuildLookup();
            return progressionLookup[charClass].ContainsKey(stat) ? progressionLookup[charClass][stat] : new float[0];
        }


        public float GetHealth(CharacterClass charClass, int level)
        {
            return GetStat(Stat.Health, charClass, level);
        }

        public float GetStat(Stat stat, CharacterClass charClass, int level)
        {
            float[] statsPerLevel = GetStatArray(stat, charClass);

            return level < 0
                ? statsPerLevel[0]
                : level > statsPerLevel.Length
                    ? statsPerLevel[statsPerLevel.Length - 1]
                    : statsPerLevel[level - 1];
        }

        public float[] GetLevelProgression(CharacterClass charClass)
        {
            return GetStatArray(Stat.ExperienceToLevelUp, charClass);
        }

        public int GetClassLevelCount(CharacterClass charClass, Stat stat)
        {
            return GetStatArray(stat, charClass).Length;
        }

        #region Debug
        void DebugLookupDictionary()
        {
            Debug.Log("Here's the dictionary");
            StringBuilder sb = new StringBuilder();
            foreach (CharacterClass charclass in progressionLookup.Keys)
            {
                sb.Append(charclass.ToString()).AppendLine(":");

                foreach (Stat stat in progressionLookup[charclass].Keys)
                {
                    sb.Append(stat.ToString()).Append(": ");

                    foreach (float val in progressionLookup[charclass][stat])
                    {

                        sb.Append(val).Append(" ");
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }
        #endregion

        #region ProgressionPlan
        [System.Serializable]
        class ProgressionPlan
        {
            [SerializeField] CharacterClass characterClass = CharacterClass.None;
            public ClassLevel[] levels = new ClassLevel[0];

            public CharacterClass Class
            {
                get { return characterClass; }
            }

            public ClassLevel GetStatsOfClassLevel(int level)
            {
                return levels == null || level < 1 || level > levels.Length
                    ? null
                    : levels[level - 1];
            }

            public float GetStatOfClassLevel(Stat stat, int level)
            {
                return GetStatsOfClassLevel(level)?.GetStatValue(stat) ?? 0f;
            }
        }
        #endregion

        #region ClassLevel
        [System.Serializable]
        class ClassLevel
        {
            public StatValue[] stats = null;

            public float GetStatValue(Stat stat)
            {
                foreach (StatValue statValue in stats)
                {
                    if (statValue.stat == stat)
                    {
                        return statValue.value;
                    }
                }
                throw new System.InvalidOperationException($"Cannot find stat of type {stat}");
            }

            public float Health
            {
                get { return GetStatValue(Stat.Health); }
            }
        }
        #endregion

        #region StatValue
        // Structs are weird.
        // I don't want to over complicate it by adding defaults and constructors.
#pragma warning disable 649 //Warning about no possible default value or not initialized.
        [System.Serializable]
        struct StatValue
        {
            public Stat stat;
            public float value;
        }
#pragma warning restore 649

        #endregion
    }
}
