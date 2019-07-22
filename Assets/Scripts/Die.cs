using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Die : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI textComponent;
    public Image imageComponent;

    [Header("Faces")]
    public List<DieFace> faces;
    private DieFace currentFace = null;


    // availability
    private bool available = true;


    // tweens
    private Tween transitionTween;
    private bool transitioning = false;



    public void Roll()
    {
        currentFace = RandomFace();

        if (textComponent != null)
        {
            textComponent.text = currentFace.text;
        }

        if (imageComponent != null)
        {
            imageComponent.color = currentFace.color;
            imageComponent.sprite = currentFace.sprite;
        }
    }

    public void SetFace(DieFace newFace)
    {
        currentFace = newFace;

        if (textComponent != null)
        {
            textComponent.text = currentFace.text;
        }

        if (imageComponent != null)
        {
            imageComponent.color = currentFace.color;
            imageComponent.sprite = currentFace.sprite;
        }
    }


    public void Hide()
    {
        if (imageComponent != null && !transitioning)
        {
            available = false;

            if (transitionTween != null) { transitionTween.Kill(); transitionTween = null; }
            transitioning = true;

            imageComponent.rectTransform.localScale = Vector2.one;
            transitionTween = imageComponent.rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                transitioning = false;

                gameObject.SetActive(false);
            });
        }
    }

    public void Show()
    {
        if (imageComponent != null)
        {
            available = true;

            if (transitionTween != null) { transitionTween.Kill(); transitionTween = null; }
            transitioning = true;

            gameObject.SetActive(true);
            imageComponent.rectTransform.localScale = Vector2.zero;
            transitionTween = imageComponent.rectTransform.DOScale(1f, 0.4f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                transitioning = false;
            });
        }
    }


    public void Highlight()
    {
        if (imageComponent != null)
        {
            if (transitionTween != null) { transitionTween.Kill(); transitionTween = null; }

            transitionTween = imageComponent.rectTransform.DOScale(1.3f, 0.4f).SetEase(Ease.OutCubic);
        }
    }

    public void UnHighlight()
    {
        if (imageComponent != null)
        {
            if (transitionTween != null) { transitionTween.Kill(); transitionTween = null; }

            transitionTween = imageComponent.rectTransform.DOScale(1f, 0.4f).SetEase(Ease.InCubic);
        }
    }


    public bool Available() { return available; }


    public DieFace GetFace()
    {
        return currentFace;
    }

    public DieFace RandomFace()
    {
        return faces[Random.Range(0, faces.Count)];
    }
}


[System.Serializable]
public class DieFace
{
    public string name = "Die Face";
    public string text = string.Empty;
    public Color color = Color.white;
    public Sprite sprite = null;
}
