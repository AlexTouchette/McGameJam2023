using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DeckManager : MonoBehaviour
{
    [SerializeField]
    GameObject cardPrefab;

    [SerializeField]
    int handSize;

    [SerializeField] 
    CardShop cardShop;

    Dictionary<CardType, CardData> possibleCards = new Dictionary<CardType, CardData>();
    [HideInInspector] public Dictionary<string, int> movementPoints = new Dictionary<string, int>();

    List<CardData> drawPile = new List<CardData>();
    List<CardData> hand = new List<CardData>();
    List<CardData> discardPile = new List<CardData>();

    TMPro.TextMeshProUGUI drawPileCount;
    TMPro.TextMeshProUGUI discardPileCount;

    TMPro.TextMeshProUGUI jungleMovementPointsText;
    TMPro.TextMeshProUGUI mountainsMovementPointsText;
    TMPro.TextMeshProUGUI desertMovementPointsText;

    public Tilemap LootTileMap;
    private TileManager m_Tm;
    private GameManager m_Gm;

    ItemState itemState = new ItemState();

    // Start is called before the first frame update
    void Start()
    {
        movementPoints.Add("MoutainsTile", 0);
        movementPoints.Add("DesertTile", 0);
        movementPoints.Add("JungleTile", 0);

        drawPileCount = GameObject.Find("DrawCardCount").GetComponent<TMPro.TextMeshProUGUI>();
        discardPileCount = GameObject.Find("DiscardCardCount").GetComponent<TMPro.TextMeshProUGUI>();

        jungleMovementPointsText = GameObject.Find("JungleMovementText").GetComponent<TMPro.TextMeshProUGUI>();
        mountainsMovementPointsText = GameObject.Find("MountainsMovementText").GetComponent<TMPro.TextMeshProUGUI>();
        desertMovementPointsText = GameObject.Find("DesertMovementText").GetComponent<TMPro.TextMeshProUGUI>();

        // This is a game jam lord please forgive me
        possibleCards.Add(CardType.Water, new CardData()
        {
            cardType = CardType.Water,
            title = "Water",
            description = "You found water. Congratulations you won't die of dehydration (maybe dysentry though)",
            numToCraft = 0,
            itemType = ItemType.None
        }
        );

        m_Tm = GameObject.Find("Grid").GetComponent<TileManager>();
        m_Gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            
        possibleCards.Add(CardType.JungleMove, new CardData()
        {
            cardType = CardType.JungleMove,
            title = "Hack and slash",
            description = "Move through a jungle space. So many vines.",
            numToCraft = 0,
            itemType = ItemType.None
        }
        );

        possibleCards.Add(CardType.MountainMove, new CardData()
        {
            cardType = CardType.MountainMove,
            title = "Climb",
            description = "Move in mountain terrain. Do you like bouldering?",
            numToCraft = 0,
            itemType = ItemType.None
        }
        );

        possibleCards.Add(CardType.DesertMove, new CardData()
        {
            cardType = CardType.DesertMove,
            title = "Take a walk",
            description = "Take a stroll in the desert.",
            numToCraft = 0,
            itemType = ItemType.None
        }
        );

        possibleCards.Add(CardType.Gourd, new CardData()
        {
            cardType = CardType.Gourd,
            title = "Gourd",
            description = "Holds 1 water when you draw in excess",
            numToCraft = 3,
            itemType = ItemType.Gourd
        }
        );

        

        // Create initial deck
        for (int i = 0; i < 1; i++)
            discardPile.Add(possibleCards[CardType.Gourd]);

        for (int i = 0; i < 3; i++)
            discardPile.Add(possibleCards[CardType.JungleMove]);

        for (int i = 0; i < 3; i++)
            discardPile.Add(possibleCards[CardType.MountainMove]);

        for (int i = 0; i < 3; i++)
            discardPile.Add(possibleCards[CardType.DesertMove]);

        Draw();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shuffle()
    {
        // TODO: validate this is truly random
        foreach (CardData cardData in drawPile)
            discardPile.Add(cardData);
        drawPile.Clear();
        drawPile = discardPile.OrderBy(a => UnityEngine.Random.Range(0, int.MaxValue)).ToList();
        discardPile.Clear();
    }

    public void Draw()
    {
        movementPoints["MountainTile"] = 0;
        movementPoints["DesertTile"] = 0;
        movementPoints["JungleTile"] = 0;

        GameObject[] instantiatedCards = GameObject.FindGameObjectsWithTag("Card");

        for (int i = 0; i < instantiatedCards.Length; i++)
        {
            Card card = instantiatedCards[i].GetComponent<Card>();
            card.dest = GameObject.Find("DiscardPile").transform.localPosition;
            card.deleteOnEnd = true;
        }

        for (int i = 0; i < hand.Count; i++)
            discardPile.Add(hand[i]);
        hand.Clear();

        if (drawPile.Count == 0) Shuffle();

        hand = new List<CardData>();
        if(handSize > drawPile.Count)
        {
            hand = drawPile.GetRange(0, drawPile.Count);
            drawPile.RemoveRange(0, drawPile.Count);
            Shuffle();
        }
        int nbOfCardsToDraw = Math.Min(handSize, handSize - hand.Count);
        hand = hand.Concat(drawPile.GetRange(drawPile.Count - nbOfCardsToDraw, nbOfCardsToDraw)).ToList();
        drawPile.RemoveRange(drawPile.Count - nbOfCardsToDraw, nbOfCardsToDraw);
        CheckMovementDamage();
        
        Vector2 handCentre = GameObject.Find("HandCentre").transform.localPosition;
        CalculateMovementPoints();

        StartCoroutine(DrawCards(0.2f, hand, handCentre));

        drawPileCount.text = drawPile.Count.ToString();
        discardPileCount.text = discardPile.Count.ToString();
    }

    void CheckMovementDamage()
    {
        bool hasWater = false;
        foreach (CardData card in hand)
        {
            if (card.cardType == CardType.Water)
            {
                hasWater = true;
            }
        }

        if (!hasWater)
        {
            m_Gm.TakeDamage(1);
        }
    }
    
    void CalculateMovementPoints()
    {
        foreach(CardData cardData in hand)
        {
            switch(cardData.cardType)
            {
                case CardType.JungleMove:
                    movementPoints["JungleTile"]++;
                    break;
                case CardType.MountainMove:
                    movementPoints["MountainTile"]++;
                    break;
                case CardType.DesertMove:
                    movementPoints["DesertTile"]++;
                    break;
                default:
                    break;
            }
        }
        UpdateUIMovementPoints();
    }

    List<CardData> CheckCraftedItems()
    {
        Dictionary<CardData, int> cardTypeCounts = new Dictionary<CardData, int>();
        List<CardData> cardTypesToRemove = new List<CardData>();
        foreach(CardData cardData in hand)
        {
            if (cardTypeCounts.ContainsKey(cardData))
                cardTypeCounts[cardData]++;
            else
                cardTypeCounts.Add(cardData, 1);
        }
        foreach(var pair in cardTypeCounts)
        {
            if(pair.Key.numToCraft != 0 && pair.Value >= pair.Key.numToCraft)
            {
                while(hand.Contains(pair.Key))
                    hand.Remove(pair.Key);
                while (drawPile.Contains(pair.Key))
                    drawPile.Remove(pair.Key);
                while (discardPile.Contains(pair.Key))
                    discardPile.Remove(pair.Key);
                UpdateUICardCounts();

                cardTypesToRemove.Add(pair.Key);
                itemState.ActivateItem(pair.Key.itemType);
            }
        }
        return cardTypesToRemove;
    }

    IEnumerator DrawCards(float time, List<CardData> cardsToDraw, Vector2 handCentre)
    {
        float initialXCardPos = handCentre.x - (float)cardsToDraw.Count / 2 * Card.cardWidth + (float)Card.cardWidth / 2;
        List<Card> drawnCards = new List<Card>();

        for (int i = 0; i < cardsToDraw.Count; i++)
        {
            yield return new WaitForSeconds(time);
            GameObject cardObject = Instantiate(cardPrefab, GameObject.Find("UI").transform);
            Card card = cardObject.GetComponent<Card>();
            card.SetCardData(cardsToDraw[i]);
            drawnCards.Add(card);
            RectTransform rt = cardObject.GetComponent<RectTransform>();
            rt.anchoredPosition = GameObject.Find("DrawPile").transform.localPosition;
            cardObject.GetComponent<Card>().dest = new Vector2(initialXCardPos + i * Card.cardWidth, handCentre.y);
        }
        yield return new WaitForSeconds(1);
        List<CardData> cardsToRemove = CheckCraftedItems();
        foreach(Card drawnCard in drawnCards)
        {
            if (cardsToRemove.Contains(drawnCard.cardData))
                drawnCard.Fade();
        }
    }

    void UpdateUICardCounts()
    {
        discardPileCount.text = discardPile.Count.ToString();
        drawPileCount.text = drawPile.Count.ToString();
    }

    public void UpdateUIMovementPoints()
    {
        jungleMovementPointsText.text = "Jungle: " + movementPoints["JungleTile"].ToString();
        mountainsMovementPointsText.text = "Mountains: " + movementPoints["MountainTile"].ToString();
        desertMovementPointsText.text = "Desert: " + movementPoints["DesertTile"].ToString();
    }

    public CardData GetRandomCard(List<CardType> excludedTypes)
    {
        List<CardType> cardTypes = Enum.GetValues(typeof(CardType)).OfType<CardType>().ToList();
        List<CardType> common = cardTypes.Intersect(excludedTypes).ToList();

        foreach(CardType ct in cardTypes)
            if (itemState.isItemActive(possibleCards[ct].itemType) && !common.Contains(ct))
                common.Add(ct);

        cardTypes.RemoveAll(cardType => common.Contains(cardType));
        CardType cardType = cardTypes[UnityEngine.Random.Range(0, cardTypes.Count)];
        return possibleCards[cardType];
    }

    public void AddCardToDiscard(Card card)
    {
        discardPile.Add(card.cardData);
        discardPileCount.text = discardPile.Count.ToString();
    }

    public void ShowShop()
    {
        cardShop.gameObject.SetActive(true);
        cardShop.GenerateNewCards(2);
        LootTileMap.SetTile(Vector3Int.FloorToInt(m_Tm.CurrentPosition), null);
    }
}
