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
        }
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
