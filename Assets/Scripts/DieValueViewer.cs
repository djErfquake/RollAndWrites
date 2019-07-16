using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DieValueViewer : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI textComponent;
    public Image imageComponent;
    public Outline outline;

    [Header("Defaults")]
    public DieFace unused;

    private DieFace currentFace;

    public void SetViewAnimated(DieFace newFace)
    {
        currentFace = newFace;

        if (textComponent != null)
        {
            textComponent.text = currentFace.text;
        }

        if (imageComponent != null)
        {
            imageComponent.sprite = currentFace.sprite;

            Color noAlpha = currentFace.color; noAlpha.a = 0f;
            imageComponent.color = noAlpha;
            imageComponent.DOFade(1f, 0.4f).SetEase(Ease.OutCubic);

            imageComponent.rectTransform.localScale = Vector2.zero;
            imageComponent.rectTransform.DOScale(1f, 0.4f).SetEase(Ease.OutCubic);
        }
    }


    public void SetView(DieFace newFace)
    {
        currentFace = newFace;

        if (textComponent != null)
        {
            textComponent.text = currentFace.text;
        }

        if (imageComponent != null)
        {
            imageComponent.sprite = currentFace.sprite;
            imageComponent.color = currentFace.color;
        }
    }


    public DieFace GetCurrentFace()
    {
        return currentFace;
    }

    public void SetToUnused()
    {
        SetView(unused);
    }

    public bool Unused()
    {
        return currentFace == unused;
    }


    public void SetOutline(Color outlineColor)
    {
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }

    public void SetOutlineIfUnused(Color outlineColor)
    {
        if (Unused())
        {
            outline.effectColor = outlineColor;
            outline.enabled = true;
        }
    }

    public void HideOutline()
    {
        if (outline != null) { outline.enabled = false; }
    }

    public bool Outlined()
    {
        return outline.enabled;
    }


    public void SetText(string text)
    {
        textComponent.text = text;
    }




    private void OnValidate()
    {
        SetToUnused();
    }
}
