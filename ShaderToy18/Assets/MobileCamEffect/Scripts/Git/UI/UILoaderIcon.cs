using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PositionAnimationInfo
{
    public Vector3 minDotPosition;
    public Vector3 maxDotPosition;
}

public class UILoaderIcon : MonoBehaviour
{
    [SerializeField] int dotN;
    [SerializeField] Sprite dotSprite;
    [SerializeField] float dotSize = 100;
    [SerializeField] float rayLength = 50;

    RectTransform pivot;
    RectTransform[] dots;

    void Awake()
    {
        pivot = GetComponent<RectTransform>();

        dots = new RectTransform[dotN];
        dotPositionInfo = new PositionAnimationInfo[dotN];

        for (int i = 0; i < dotN; i++)
            dots[i] = SpawnDot(i);

        minDotScale = new Vector3(0.55f, 0.55f, 0.55f);
        maxDotScale = new Vector3(1.15f, 1.15f, 1.15f);
        minRotSpeed = rotationAnimSpeed * 0.5f;
        maxRotSpeed = rotationAnimSpeed * 4f;

        endScale = pivot.localScale;
    }

    RectTransform SpawnDot(int i)
    {
        GameObject dot = new GameObject("DotImage_" + i.ToString());

        float index = ((float)i / (float)dotN) * Mathf.PI * 2.0f;
        Vector3 localPos = new Vector3(Mathf.Sin(index), Mathf.Cos(index), 0.0f) * rayLength;

        RectTransform dotTransform = dot.AddComponent<RectTransform>();
        dotTransform.SetParent(pivot, false);
        dotTransform.localPosition = localPos;
        dotTransform.sizeDelta = new Vector2(dotSize, dotSize);

        dotPositionInfo[i] = new PositionAnimationInfo
        {
            minDotPosition = dotTransform.localPosition,
            maxDotPosition = dotTransform.localPosition * 1.3f
        };

        Image dotImage = dot.AddComponent<Image>();
        dotImage.sprite = dotSprite;
        dotImage.color = Color.black;
        dotImage.preserveAspect = true;
        dotImage.raycastTarget = false;

        return dotTransform;
    }

    void Update()
    {
        for (int i = 0; i < dots.Length; i++)
        {
            UpdateDotMovement(dots[i], i);
        }

        rotationAlpha += Time.deltaTime;

        pivot.Rotate(0.0f, 0.0f, Mathf.LerpUnclamped(minRotSpeed, maxRotSpeed, pivotRotationCurve.Evaluate(rotationAlpha)) * Time.deltaTime);
	}

    Vector3 minDotScale;
    Vector3 maxDotScale;
    float minRotSpeed;
    float maxRotSpeed;
    [SerializeField] float scaleAnimSpeed;
    [SerializeField] AnimationCurve dotScaleCurve;
    [SerializeField] float rotationAnimSpeed;
    [SerializeField] AnimationCurve pivotRotationCurve;
    float scaleAlpha;
    float rotationAlpha;

    PositionAnimationInfo[] dotPositionInfo;

    void UpdateDotMovement(RectTransform dotT, int i)
    {
        scaleAlpha += Time.deltaTime * scaleAnimSpeed;

        float alpha = dotScaleCurve.Evaluate(scaleAlpha + (i / (float)dotN));

        dotT.localScale = Vector3.LerpUnclamped(minDotScale, maxDotScale, alpha);
        dotT.localPosition = Vector3.LerpUnclamped(dotPositionInfo[i].minDotPosition, dotPositionInfo[i].maxDotPosition, alpha);
    }

    public void StartAnimation()
    {
        enabled = true;
        StartCoroutine(FadeCoroutine(1));
    }

    public void StopAnimation()
    {
        StartCoroutine(FadeCoroutine(-1));
    }

    public void Reset()
    {
        enabled = false;
        pivot.localScale = Vector3.zero;
    }

    [SerializeField] float fadeAnimSpeed;
    Vector3 endScale;
    IEnumerator FadeCoroutine(int fadeDirection)
    {
        float alpha = fadeDirection > 0 ? 0.0f : 1.0f;

        while (alpha >= 0.0f && alpha <= 1.0f)
        {
            alpha += Time.deltaTime * fadeAnimSpeed * fadeDirection;
            pivot.localScale = Vector3.Lerp(Vector3.zero, endScale, dotScaleCurve.Evaluate(alpha));

            yield return null;
        }

        if (fadeDirection < 0.0f)
        {
            enabled = false;
            pivot.localScale = Vector3.zero;
        }
        else
            pivot.localScale = Vector3.one;
    }
}
