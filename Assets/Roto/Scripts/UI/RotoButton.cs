using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[HelpURL("https://code.roto.com/standard-applications/unity-roto-base#ui-helpers")]
[AddComponentMenu("Roto/Button")]
[RequireComponent(typeof(Image))]
public class RotoButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, IDragHandler
{
    private Image image;

    [Header("Settings")]
    private bool mouseDown = false;
    private bool isOver = false;
    private bool disabled = false;
    private bool selected = false;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite downpressSprite;
    public Sprite disabledSprite;
    public Sprite selectedSprite;


    public UnityEvent onClick;

    private RotoButtonGroup group;


    private void Awake()
    {
        image = GetComponent<Image>();
    }


    public void SetGroup(RotoButtonGroup buttonGroup)
    {
        group = buttonGroup;
    }





    public void Enable(bool enable)
    {
        disabled = !enable;

        if (disabled && disabledSprite != null)
        {
            image.sprite = disabledSprite;
        }
        else if (selected && selectedSprite != null)
        {
            image.sprite = selectedSprite;
        }
        else
        {
            image.sprite = normalSprite;
        }
    }


    public bool IsEnabled()
    {
        return !disabled;
    }



    public void Select(bool select)
    {
        selected = select;

        if (selected && selectedSprite != null)
        {
            image.sprite = selectedSprite;
        }
        else
        {
            image.sprite = normalSprite;
        }
    }

    public bool IsSelected()
    {
        return selected;
    }








    public void OnPointerDown(PointerEventData eventData)
    {
        if (!disabled)
        {
            image.sprite = downpressSprite;

            mouseDown = true;
            isOver = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!disabled)
        {
            image.sprite = normalSprite;

            if (isOver)
            {
                onClick.Invoke();
                if (group != null) { group.ButtonPressed(this);}
            }

            mouseDown = false;
            isOver = false;
        }    
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        if (!disabled)
        {
            if (mouseDown) { image.sprite = normalSprite; }
            isOver = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!disabled)
        {
            if (mouseDown) { image.sprite = downpressSprite; }
            isOver = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ignore
        // if we don't do this, things screw up with the pointer up event for some reason
    }
}
