using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIViewPanel : UIPanel
{
    [SerializeField] Image photoImage;
    [SerializeField] UILoaderIcon loaderIcon;
    [SerializeField] RectTransform buttonPanelTransform;

    Texture2D photoReference;
    Sprite currentUIPhoto;

    void Start()
    {
        photoImage.sprite = currentUIPhoto;
        loaderIcon.Reset();
        buttonPanelTransform.localScale = Vector3.zero;
    }

    public void SetupPhotoImage(Texture2D newPhoto)
    {
        photoReference = newPhoto;
        currentUIPhoto = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), new Vector2(0.5f, 0.5f), 2000.0f, 0, SpriteMeshType.FullRect);
        photoImage.sprite = currentUIPhoto;

        ApplicationManager.CachePhoto(photoReference, OnPhotoCached);

        loaderIcon.StartAnimation();
    }

    public void OnSharePressed()
    {
        ApplicationManager.SharePhoto();
    }

    public void OnSavePressed()
    {
        ApplicationManager.SavePhoto();
    }

    public void OnCloseButtonPressed()
    {
        UIManager.CloseViewPanel();
        RegisterCallback(OnViewPanelIsClosed);
    }

    void OnPhotoCached()
    {
        loaderIcon.StopAnimation();
        StartCoroutine(ButtonFadeInCoroutine());
    }

    IEnumerator ButtonFadeInCoroutine()
    {
        float alpha = 0.0f;

        while (alpha <= 1.0f)
        {
            alpha += Time.deltaTime * 2.0f;
            buttonPanelTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, alpha);

            yield return null;
        }

        buttonPanelTransform.localScale = Vector3.one;
    }

    void OnViewPanelIsClosed(AnimDirection direction)
    {
        buttonPanelTransform.localScale = Vector3.zero;
    }
}
