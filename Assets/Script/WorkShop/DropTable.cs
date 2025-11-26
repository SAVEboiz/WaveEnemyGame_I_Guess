using UnityEngine;
using System.Linq;


[System.Serializable]
public class ItemDropEntry
{
    public DropItem item; 

    
    [Range(0, 100)]
    public float weight = 1f; 
}

public class DropTable : MonoBehaviour
{
    public ItemDropEntry[] dropEntries;

    public GameObject GetRandomDrop()
    {

        float totalWeight = dropEntries.Sum(entry => entry.weight);


        if (totalWeight <= 0) return null;


        float randValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var entry in dropEntries)
        {
            cumulativeWeight += entry.weight;

            if (randValue <= cumulativeWeight)
            {
                return entry.item.itemPrefab;
            }
        }

        return null; 
    }
}