using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Water,
    JungleMove,
    DesertMove,
    MountainMove,
    Gourd,
    Plane,
    Car,
    Adrenaline
}
public struct CardData
{
    public CardType cardType { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public int numToCraft { get; set; }
    public ItemType itemType { get; set; }
}
