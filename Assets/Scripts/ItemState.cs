using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public enum ItemType
{
    Gourd
}

public class ItemState
{
    Dictionary<ItemType, bool> itemState;

    public ItemState()
    {
        itemState = new Dictionary<ItemType, bool>();
        List<ItemType> itemTypes = Enum.GetValues(typeof(ItemType)).OfType<ItemType>().ToList();
        foreach (ItemType itemType in itemTypes)
            itemState.Add(itemType, false);
    }

    public void ActivateItem(ItemType itemType)
    {
        itemState[itemType] = true;
        GameObject.Find("GourdItem").GetComponent<Image>().color = new Color(255, 255, 255, 255);
    }
}
