using System.Collections;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class ScrollSnapController : MonoBehaviour
{
    public ScrollRect scrollRect;

    public HorizontalLayoutGroup hlg;

    public GameObject card;

    public float snapDuration = 0.25f;

    private Coroutine currentMove;

    public void OnClickLeft()
    {
        Move(-1);
    }

    public void OnClickRight()
    {
        Move(1);
    }

    private void Move(int step)
    {
        if (scrollRect == null || scrollRect.content == null || card == null || hlg == null)
        {
            Debug.LogError("Components missing");
            return;
        }

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();

        float cardWidth = card.GetComponent<RectTransform>().rect.width;
        float spacing = hlg.spacing;
        float delta = -(cardWidth + spacing) * step;
        float curX = content.anchoredPosition.x;
        float curY = content.anchoredPosition.y;

        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;


        if (contentWidth <= viewportWidth) return;

        float minX = viewportWidth - contentWidth;
        float maxX = viewportWidth + contentWidth;

        float targetX = Mathf.Clamp(curX + delta, minX, maxX);
        if (Mathf.Approximately(targetX, curX)) return;

        if (currentMove != null)
        {
            StopCoroutine(currentMove);
            currentMove = null;
        }
       
        currentMove = StartCoroutine(MoveCoroutine(content, curX, targetX, curY, snapDuration));
    }

    private IEnumerator MoveCoroutine(RectTransform content, float fromX, float toX, float y, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float x = Mathf.Lerp(fromX, toX, Mathf.Clamp01(t / duration));
            content.anchoredPosition = new Vector2(x, y);
            yield return null;
        }
        content.anchoredPosition = new Vector2(toX, y);
        currentMove = null;
    }
}
