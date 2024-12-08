using System.Collections.Generic;
using System.Linq;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Inventories
{

    [CreateAssetMenu(menuName = "RPG/Inventory/Loot Table")]
    public class LootTable : ScriptableObject
    {
        // - Drop Chance
        // - Min drops
        // - Max drops
        // - Potential Drops
        //   - Related chance
        //   - Min items
        //   - Max items
        [SerializeField]
        LootConfig[] potentialLoot;
        [SerializeField]
        [Range(0, 100)]
        float[] dropChancePercentage;
        [SerializeField]
        int[] minDrops;
        [SerializeField]
        int[] maxDrops;

        public IEnumerable<Loot> GenerateRandomLoot(int level)
        {
            if (!ShouldGenerateLoot(level))
            {
                yield break;
            }

            int i = GenerateRandomNumberOfLoot(level);
            while (i > 0)
            {
                yield return GetRandomLoot(level);
                i--;
            }
        }

        private int GenerateRandomNumberOfLoot(int level)
        {
            return Random.Range(
                GetByLevel(minDrops, level),
                GetByLevel(maxDrops, level)// + 1
            );
        }

        bool ShouldGenerateLoot(int level)
        {
            return Random.Range(1, 100) < GetByLevel(dropChancePercentage, 1);
        }

        Loot GetRandomLoot(int level)
        {
            LootConfig lootConfig = SelectRandomItem(level);
            return new Loot
            {
                item = lootConfig.item,
                number = lootConfig.GetRandomNumber(level)
            };
        }

        LootConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance);
            float chanceTotal = 0;
            foreach (LootConfig loot in potentialLoot)
            {
                chanceTotal += GetByLevel(loot.relativeChance, level);
                if (chanceTotal > randomRoll)
                {
                    return loot;
                }
            }
            return null;
        }

        private float GetTotalChance(int level)
        {
            return potentialLoot == null
                ? 0
                : potentialLoot.Sum(loot => GetByLevel(loot.relativeChance, level));
        }

        [System.Serializable]
        class LootConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;

            public int GetRandomNumber(int level)
            {
                return item.IsStackable()
                    ?
                        Random.Range(
                            GetByLevel(minNumber, level),
                            GetByLevel(maxNumber, level) + 1
                        )
                    :
                        1;
            }
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            return values.Length == 0
                ? default
                : level > values.Length
                    ? values[values.Length - 1]
                    : level <= 0
                        ? default
                        : values[level - 1];

        }
    }

    public struct Loot
    {
        public InventoryItem item;
        public int number;
    }
}