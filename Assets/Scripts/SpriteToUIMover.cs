using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteToUIMover : MonoBehaviour
{
    public Vector3 offset;
    public RectTransform toRectTransform;
    public float moveDuration = 3f;

    public void MoveToUI()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToUICo());
    }

    private IEnumerator MoveToUICo()
    {
        Vector3 fromPosition = transform.position;

        Vector2 toPositionScreen = RectTransformToScreenSpace(toRectTransform).center;
        Vector3 toPosition = Camera.main.ScreenToWorldPoint(new Vector3(toPositionScreen.x, toPositionScreen.y, transform.position.z - Camera.main.transform.position.z));

        float timer = moveDuration;
        while (timer > 0f)
        {
            float p = 1f - (timer / moveDuration);
            transform.position = Vector3.Lerp(fromPosition, toPosition + offset, p);

            timer -= Time.deltaTime;
            yield return null;
        }
    }

    private Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }
}
