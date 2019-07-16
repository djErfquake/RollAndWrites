using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image)), RequireComponent(typeof(Shadow))]
public class ChunkyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, IDragHandler
{
    // components
    private Image image;
    private Shadow shadow;

    // settings
    private bool mouseDown = false;
    private bool isOver = false;
    private Vector2 startingPosition = Vector2.zero;
    private Vector2 shadowSize = Vector2.zero;

    // click event
    public UnityEvent onClick;


    private void Awake()
    {
        image = GetComponent<Image>();
        shadow = GetComponent<Shadow>();

        startingPosition = image.rectTransform.anchoredPosition;
        shadowSize = shadow.effectDistance;
    }



    private void Down()
    {
        image.rectTransform.anchoredPosition = startingPosition + shadowSize;
        shadow.effectDistance = Vector2.zero;
    }

    private void Up()
    {
        image.rectTransform.anchoredPosition = startingPosition;
        shadow.effectDistance = shadowSize;
    }





    public void OnPointerDown(PointerEventData eventData)
    {
        Down();

        mouseDown = true;
        isOver = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Up();

        if (isOver)
        {
            onClick.Invoke();
        }

        mouseDown = false;
        isOver = false;
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        if (mouseDown) { Up(); }
        isOver = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mouseDown) { Down(); }
        isOver = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ignore
        // if we don't do this, things screw up with the pointer up event for some reason
    }




}
