using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[HelpURL("https://code.roto.com/standard-applications/unity-roto-base#ui-helpers")]
[AddComponentMenu("Roto/UI Sprite")]
[RequireComponent(typeof(Image))]
public class UISpriteAnimation : MonoBehaviour
{
    [Header("Settings")]
    public bool loop = true;
    public bool animateOnStart = false;
    public float speed = 1f;
    public bool resize = false;

    [Header("Sprites")]
    public List<Sprite> sprites = new List<Sprite>();
    private int spriteIndex = 0;
    private Image image;

    // animation
    private Coroutine animationCoroutine;

    [HideInInspector]
    public bool isPlaying = false;


    private void Awake()
    {
        image = GetComponent<Image>();
        Stop();
    }

    private void Start()
    {
        if (animateOnStart)
        {
            Play();
        }
    }

    /// <summary>
    /// pause and reset
    /// </summary>
    public void Stop()
    {
        Pause();
        SetToSpriteIndex(0);
    }


    public void Play()
    {
        Stop();
        animationCoroutine = StartCoroutine(ExhibitUtilities.DoActionForever(NextSprite, speed, speed));
        isPlaying = true;
    }


    

    public void Pause()
    {
        if (animationCoroutine != null) { StopCoroutine(animationCoroutine); animationCoroutine = null; }
        isPlaying = false;
    }


    private void NextSprite()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Count)
        {
            if (loop)
            {
                spriteIndex = 0;
            }
            else
            {
                Pause();
            }
        }

        SetToSpriteIndex(spriteIndex);
    }

    private void SetToSpriteIndex(int index)
    {
        spriteIndex = index;
        if (spriteIndex < sprites.Count)
        {
            image.sprite = sprites[spriteIndex];

            if (resize) { image.rectTransform.sizeDelta = sprites[spriteIndex].rect.size; }
        }
    }
}
