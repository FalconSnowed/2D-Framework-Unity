using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public List<ItemData> allItems;

    public ItemData GetItemByID(string id)
    {
        return allItems.Find(item => item.itemID == id);
    }
}
