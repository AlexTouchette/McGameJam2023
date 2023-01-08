using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public enum ItemType
{
    None,
    Gourd
}

public class ItemState
{
    Dictionary<ItemType, bool> itemState;
    bool gourdFilled = false;

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
        GameObject.Find("GourdItem").GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void FillGourd()
    {
        if (!itemState[ItemType.Gourd]) return;
        gourdFilled = true;
        GameObject.Find("GourdText").GetComponent<TMPro.TextMeshProUGUI>().text = "1";
    }

    public bool UseGourd()
    {
        if (!itemState[ItemType.Gourd]) return false;
        if (gourdFilled)
        {
            GameObject.Find("GourdText").GetComponent<TMPro.TextMeshProUGUI>().text = "0";
            gourdFilled = false;
            return true;
        }
        return false;
    }

    public bool isItemActive(ItemType itemType)
    {
        return itemState.ContainsKey(itemType) ? itemState[itemType] : false;
    }
}
