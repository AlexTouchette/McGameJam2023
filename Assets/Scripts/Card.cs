using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    CardData cardData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCardData(CardData cardData)
    {
        this.cardData = cardData;
        transform.Find("Title").GetComponent<TMPro.TextMeshProUGUI>().text = cardData.title;
        transform.Find("Description").GetComponent<TMPro.TextMeshProUGUI>().text = cardData.description;
    }
}
