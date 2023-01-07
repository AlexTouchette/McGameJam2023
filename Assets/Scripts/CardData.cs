using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Water,
    Gourd,
    JungleMove
}
public struct CardData
{
    public CardType cardType { get; set; }
    public string title { get; set; }
    public string description { get; set; }
}
