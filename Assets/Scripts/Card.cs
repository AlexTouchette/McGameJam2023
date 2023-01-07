using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Vector2 dest;
    public bool deleteOnEnd;

    CardData cardData;
    Vector2 start;
    RectTransform rt;
    float lerpTime;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        start = rt.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
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
    }


}
