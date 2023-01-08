using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector] public Vector2 dest;
    [HideInInspector] public bool deleteOnEnd;
    [HideInInspector] public bool buyable = false;
    [HideInInspector] public CardData cardData;

    [SerializeField]
    static public int cardHeight = 230;

    [SerializeField]
    static public int cardWidth = 150;

    
    Vector2 start;
    RectTransform rt;
    float lerpTime;
    float fadeTime;
    bool fade;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        start = rt.anchoredPosition;

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cardWidth);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cardHeight);

        if(!buyable) transform.localScale = new Vector3(0, 0, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (fade)
        {
            fadeTime += Time.deltaTime;
            GetComponent<CanvasGroup>().alpha = 1 - fadeTime;
            if (fadeTime > 1f)
                Destroy(gameObject);
        }
        if (buyable) return;
        if (rt.anchoredPosition != dest)
        {
            lerpTime += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(start, dest, lerpTime);
            float scaleValue = deleteOnEnd ? 1 - lerpTime : lerpTime;
            transform.localScale = new Vector3(scaleValue, scaleValue, transform.localScale.z);
        }
        else if (lerpTime > 0)
        {
            if(deleteOnEnd)
            {
                Destroy(gameObject);
            }
            else
            {
                start = rt.anchoredPosition;
                lerpTime = 0.0f;
            }
        }
    }

    public void SetCardData(CardData cardData)
    {
        this.cardData = cardData;
        transform.Find("Title").GetComponent<TMPro.TextMeshProUGUI>().text = cardData.title;
        transform.Find("Description").GetComponent<TMPro.TextMeshProUGUI>().text = cardData.description;
        transform.Find("CardImage").GetComponent<Image>().sprite = cardData.image;
    }

    public void Fade()
    {
        fade = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buyable) GameObject.Find("CardShop").GetComponent<CardShop>().BuyCard(gameObject);
    }
}
