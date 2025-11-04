using UnityEngine;
using UnityEngine.UI;

public class ScrollSnapController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform[] cards;

    private int currentIndex = 0;
    public float smoothSpeed = 10f;

    private float[] cardPositions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardPositions = new float[cards.Length];
        float contentWidth = content.rect.width - scrollRect.viewport.rect.width;

        for (int i = 0; i < cards.Length; i++)
        {
            float normalized = 0;
            if (contentWidth > 0)
            {
                normalized = Mathf.Clamp01(cards[i].anchoredPosition.x / contentWidth);
            }
            cardPositions[i] = normalized;
        }
    }

    public void PrevCard()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ScrollToCard(currentIndex);
        }
    }

    public void NextCard()
    {
        if (currentIndex < cards.Length - 1)
        {
            currentIndex++;
            ScrollToCard(currentIndex);
        }
    }

    private void ScrollToCard(int index)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothScroll(cardPositions[index]));
    }

    private System.Collections.IEnumerator SmoothScroll(float target)
    {
        while (Mathf.Abs(scrollRect.horizontalNormalizedPosition - target) > 0.001f)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, target, Time.deltaTime * smoothSpeed);
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = target;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
