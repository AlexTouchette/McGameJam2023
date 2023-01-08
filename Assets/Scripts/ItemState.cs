using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public enum ItemType
{
    None,
    Gourd,
    Plane,
    Car,
    Adrenaline
}

public class ItemState
{
    Dictionary<ItemType, bool> itemState;
    bool gourdFilled = false;
    int carCharges = 0;

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
        switch(itemType)
        {
            case ItemType.Gourd:
                GameObject.Find("GourdItem").GetComponent<CanvasGroup>().alpha = 1.0f;
                break;
            case ItemType.Plane:
                GameObject.Find("PlaneItem").GetComponent<CanvasGroup>().alpha = 1.0f;
                // TODO: Really win
                Debug.Log("You win!");
                break;
            case ItemType.Car:
                GameObject.Find("CarItem").GetComponent<CanvasGroup>().alpha = 1.0f;
                RefillCar();
                break;
            default:
                break;
        }
        
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

    public bool UseCar()
    {
        if (!itemState[ItemType.Car]) return false;
        if (carCharges > 0)
        {
            carCharges--;
            GameObject.Find("CarText").GetComponent<TMPro.TextMeshProUGUI>().text = carCharges.ToString();
            return true;
        }
        return false;
    }

    public void RefillCar()
    {
        if (!itemState[ItemType.Car]) return;
        carCharges = 3;
        GameObject.Find("CarText").GetComponent<TMPro.TextMeshProUGUI>().text = carCharges.ToString();
    }

    public bool isItemActive(ItemType itemType)
    {
        return itemState.ContainsKey(itemType) ? itemState[itemType] : false;
    }
}
