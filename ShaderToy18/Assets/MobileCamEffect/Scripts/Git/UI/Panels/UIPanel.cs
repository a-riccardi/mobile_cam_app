using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    [SerializeField] float animSpeed;
    [SerializeField] RectTransform panelTransform;
    [SerializeField] AnimationCurve animCurve;

    Vector3 closedPosition;
    Vector3 openedPosition;

    AnimDirection direction;
    float alpha;

    UnityAction<AnimDirection> callback;

    public virtual void Open(bool immediate = false)
    {
        if (immediate)
            alpha = 1.0f;

        direction = AnimDirection.FORWARD;
        StartCoroutine(AnimationCoroutine());
    }

    public virtual void Close(bool immediate = false)
    {
        if (immediate)
            alpha = 0.0f;

        direction = AnimDirection.BACKWARD;
        StartCoroutine(AnimationCoroutine());
    }

    public void SetupPositions(Vector3 open, Vector3 close)
    {
        openedPosition = open;
        closedPosition = close;
    }

    public void RegisterCallback(UnityAction<AnimDirection> _callback)
    {
        callback += _callback;
    }

    IEnumerator AnimationCoroutine()
    {
        while (direction == AnimDirection.BACKWARD && alpha > 0.0f || direction == AnimDirection.FORWARD && alpha < 1.0)
        {
            alpha += Time.deltaTime * animSpeed * (float)direction;
            panelTransform.anchoredPosition3D = Vector3.LerpUnclamped(closedPosition, openedPosition, animCurve.Evaluate(alpha));
            yield return null;
        }

        if (direction == AnimDirection.FORWARD)
            panelTransform.anchoredPosition3D = openedPosition;
        else
            panelTransform.anchoredPosition3D = closedPosition;

        if (callback != null)
        {
            callback.Invoke(direction);
            callback = null;
        }
    }
}
