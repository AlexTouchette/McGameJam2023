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

    Dictionary<CardType, CardData> possibleCards = new Dictionary<CardType, CardData>();

    // TODO: stacks?
    List<CardData> drawPile = new List<CardData>();
    List<CardData> hand = new List<CardData>();
    List<CardData> discardPile = new List<CardData>();

    // Start is called before the first frame update
    void Start()
    {
        // This is a game jam lord please forgive me
        possibleCards.Add(CardType.Water, new CardData() 
        { 
            title = "Water", 
            description = "You found water. Congratulations you won't die of dehydration (maybe dysentry though)"
        }
        );

        possibleCards.Add(CardType.JungleMove, new CardData()
        {
            title = "Hack and slash",
            description = "Move through a jungle space. So many vines."
        }
        );

        // Create initial deck
        for(int i = 0; i < 8; i++)
            discardPile.Add(possibleCards[CardType.Water]);

        for (int i = 0; i < 2; i++)
            discardPile.Add(possibleCards[CardType.JungleMove]);

        Shuffle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shuffle()
    {
        // TODO: validate this is truly random
        drawPile = discardPile.OrderBy(a => UnityEngine.Random.Range(0, int.MaxValue)).ToList();
        discardPile.Clear();
    }

    public void Draw()
    {
        GameObject[] instantiatedCards = GameObject.FindGameObjectsWithTag("Card");

        for (int i = 0; i < instantiatedCards.Length; i++)
            Destroy(instantiatedCards[i]);

        for(int i = 0; i < hand.Count; i++)
        {
            discardPile.Add(hand[i]);
        }
        hand.Clear();

        if (drawPile.Count == 0) Shuffle();

        int nbOfCardsToDraw = Math.Min(handSize, drawPile.Count);
        List<CardData> cardsToDraw = drawPile.GetRange(drawPile.Count - nbOfCardsToDraw, nbOfCardsToDraw);
        for(int i = 0; i < cardsToDraw.Count; i++)
        {
            Debug.Log(cardsToDraw[i].title);
        }
        drawPile.RemoveRange(drawPile.Count - nbOfCardsToDraw, nbOfCardsToDraw);

        //Vector2 handCentre = GameObject.Find("HandCentre").transform.position;
        //float initialXCardPos = handCentre.x - (float) nbOfCardsToDraw / 2 * cardWidth;
        //Debug.Log(initialXCardPos);

        for (int i = 0; i < cardsToDraw.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, GameObject.Find("Hand").transform);
            card.GetComponent<Card>().SetCardData(cardsToDraw[i]);
            hand.Add(cardsToDraw[i]);

            card.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 230);
        }

        cardsToDraw.Clear();

        GameObject.Find("DrawCardCount").GetComponent<TMPro.TextMeshProUGUI>().text = drawPile.Count.ToString();
        GameObject.Find("DiscardCardCount").GetComponent<TMPro.TextMeshProUGUI>().text = discardPile.Count.ToString();
    }
}
