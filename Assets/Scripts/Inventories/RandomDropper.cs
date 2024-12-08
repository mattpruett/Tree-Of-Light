using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

public class RandomDropper : ItemDropper
{
    [Tooltip("How far can the drops be scattered from the dropper.")]
    [SerializeField]
    float scatterDistance = 1f;

    [SerializeField]
    LootTable lootTable;

    const int ATTEMPTS = 10;

    protected override Vector3 GetDropLocation()
    {
        int attempt = 0;
        while (attempt < ATTEMPTS)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
            {
                return hit.position;
            }
            attempt++;
        }

        return transform.position;
    }

    public void RandomDrop()
    {
        if (lootTable == null) return;
        foreach (Loot loot in lootTable.GenerateRandomLoot(GetComponent<BaseStats>().Level))
        {
            DropItem(loot.item, loot.number);
        }
    }
}
