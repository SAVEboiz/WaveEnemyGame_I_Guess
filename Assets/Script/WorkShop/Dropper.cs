using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(DropTable))]
public class Dropper : MonoBehaviour
{
    private DropTable dropTable;

    void Awake()
    {
        dropTable = GetComponent<DropTable>();

        if (dropTable == null)
        {
            Debug.LogError("Dropper requires a DropTable component on the same GameObject.");
        }
    }
    public void DropItemAndDestroy()
    {
        if (dropTable == null) return;

        GameObject dropPrefab = dropTable.GetRandomDrop();

        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Debug.Log($"?? Dropper '{gameObject.name}' Drop: {dropPrefab.name}");
        }
        else
        {
            Debug.Log($"?? Dropper '{gameObject.name}' Drop");
        }

        Destroy(gameObject);
    }
}
