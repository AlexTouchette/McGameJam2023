using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField]
    GameObject cardPrefab;

    [SerializeField]
    int handSize;

    [SerializeField] 
    CardShop cardShop;

    Dictionary<CardType, CardData> possibleCards = new Dictionary<CardType, CardData>();

    // TODO: stacks?
    List<CardData> drawPile = new List<CardData>();
    List<CardData> hand = new List<CardData>();
    List<CardData> discardPile = new List<CardData>();

    TMPro.TextMeshProUGUI drawPileCount;
    TMPro.TextMeshProUGUI discardPileCount;

    // Start is called before the first frame update
    void Start()
    {
        drawPileCount = GameObject.Find("DrawCardCount").GetComponent<TMPro.TextMeshProUGUI>();
        discardPileCount = GameObject.Find("DiscardCardCount").GetComponent<TMPro.TextMeshProUGUI>();

        // This is a game jam lord please forgive me
        possibleCards.Add(CardType.Water, new CardData() 
        { 
            cardType = CardType.Water,
            title = "Water", 
            description = "You found water. Congratulations you won't die of dehydration (maybe dysentry though)"
        }
        );

        possibleCards.Add(CardType.JungleMove, new CardData()
        {
            cardType = CardType.JungleMove,
            title = "Hack and slash",
            description = "Move through a jungle space. So many vines."
        }
        );

        // Create initial deck
        for(int i = 0; i < 8; i++)
            discardPile.Add(possibleCards[CardType.Water]);

        for (int i = 0; i < 4; i++)
            discardPile.Add(possibleCards[CardType.JungleMove]);

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
        

        Vector2 handCentre = GameObject.Find("HandCentre").transform.localPosition;
        StartCoroutine(DrawCards(0.2f, hand, handCentre));

        GameObject.Find("DrawCardCount").GetComponent<TMPro.TextMeshProUGUI>().text = drawPile.Count.ToString();
        GameObject.Find("DiscardCardCount").GetComponent<TMPro.TextMeshProUGUI>().text = discardPile.Count.ToString();
    }

    IEnumerator DrawCards(float time, List<CardData> cardsToDraw, Vector2 handCentre)
    {
        float initialXCardPos = handCentre.x - (float)cardsToDraw.Count / 2 * Card.cardWidth + (float)Card.cardWidth / 2;

        for (int i = 0; i < cardsToDraw.Count; i++)
        {
            yield return new WaitForSeconds(time);
            GameObject card = Instantiate(cardPrefab, GameObject.Find("UI").transform);
            card.GetComponent<Card>().SetCardData(cardsToDraw[i]);

            RectTransform rt = card.GetComponent<RectTransform>();
            rt.anchoredPosition = GameObject.Find("DrawPile").transform.localPosition;
            card.GetComponent<Card>().dest = new Vector2(initialXCardPos + i * Card.cardWidth, handCentre.y);
        }
    }

    public CardData GetRandomCard(List<CardType> excludedTypes)
    {
        List<CardType> cardTypes = Enum.GetValues(typeof(CardType)).OfType<CardType>().ToList();
        IEnumerable<CardType> common = cardTypes.Intersect(excludedTypes).ToList();
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
    }
}
