using UnityEngine;

[CreateAssetMenu(fileName = "NewDropItem", menuName = "Standalone Drop System/Drop Item")]
public class DropItem : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab;
}