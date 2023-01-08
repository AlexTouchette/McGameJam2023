using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardShop : MonoBehaviour
{
    [SerializeField]
    DeckManager deckManager;

    [SerializeField]
    GameObject cardPrefab;

    RectTransform rt;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateNewCards(int nbOfCards)
    {
        foreach(Transform child in transform)
            if (child.gameObject.CompareTag("Card")) 
                Destroy(child.gameObject);

        List<CardType> cardTypesToExclude = new List<CardType>();
        List<CardData> heldCards = deckManager.GetAllHeldCards();
        List<CardType> heldCraftable = new List<CardType>();
        foreach(CardData cardData in heldCards)
        {
            if (cardData.numToCraft > 0 && !heldCraftable.Contains(cardData.cardType))
            {
                heldCraftable.Add(cardData.cardType);
                cardTypesToExclude.Add(cardData.cardType);
            }
        }
        for(int i = 0; i < nbOfCards; i++)
        {
            float initialXCardPos = rt.rect.center.x - (float)nbOfCards / 2 * Card.cardWidth + (float)Card.cardWidth / 2;
            GameObject cardObject = Instantiate(cardPrefab, transform);
            Card card = cardObject.GetComponent<Card>();

            CardData cardData = deckManager.GetRandomCard(cardTypesToExclude);
            card.SetCardData(cardData);
            cardTypesToExclude.Add(cardData.cardType);
            card.buyable = true;

            RectTransform cardRt = card.GetComponent<RectTransform>();

            cardRt.anchoredPosition = new Vector2(initialXCardPos + i * Card.cardWidth, rt.rect.center.y);
            if(i == nbOfCards - 1)
            {
                for(int j = 0; j < heldCraftable.Count; j++)
                {
                    cardObject = Instantiate(cardPrefab, transform);
                    card = cardObject.GetComponent<Card>();

                    card.SetCardData(deckManager.possibleCards[heldCraftable[j]]);
                    card.buyable = true;

                    cardRt = card.GetComponent<RectTransform>();

                    cardRt.anchoredPosition = new Vector2(initialXCardPos + (i + j + 1) * Card.cardWidth, rt.rect.center.y);
                }
            }
        }
    }

    void GenerateShopCard()
    {

    }

    public void BuyCard(GameObject cardObject)
    {
        cardObject.transform.SetParent(GameObject.Find("UI").transform);
        Card card = cardObject.GetComponent<Card>();
        deckManager.AddCardToDiscard(card);
        card.deleteOnEnd = true;
        card.dest = GameObject.Find("DiscardPile").transform.localPosition;
        card.buyable = false;

        gameObject.SetActive(false);
    }
}
