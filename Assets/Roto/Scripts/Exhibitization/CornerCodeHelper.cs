using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CornerCodeHelper : MonoBehaviour
{
    public float cornerCodeTimeoutSeconds = 2f;
    public float cornerCodeButtonSize = 100f;

    [HideInInspector]
    public bool addedCornerCodeButtons = false;

    private GameObject cornerCodeTestParent;
    private List<Image> cornerCodeTestImages = new List<Image>();
    private List<Rect> cornerCodeRects = new List<Rect>();
    private Color cornerCodeTestColor = Color.clear;

    private Dictionary<string, Action> cornerCodeListeners = new Dictionary<string, Action>();

    private Coroutine cornerCodeTimeoutCoroutine;

    private string cornerCode = "";




    // check for corner code press
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < cornerCodeRects.Count; i++)
            {
                if (cornerCodeRects[i].Contains(Input.mousePosition))
                {
                    CornerCodeClicked((i + 1).ToString());
                }
            }
        }
    }




    /// <summary>
    /// Adds a Code Sequence as the top overlay so that when the corners of the screen are pressed in a certain order, the application performs an Action.
    /// 1 - Top-Left
    /// 2 - Top-Right
    /// 3 - Bottom-Left
    /// 4 - Bottom-Right
    /// </summary>
    /// <param name="newCode">the triggering code</param>
    /// <param name="codeAction">the Action to trigger</param>
    public void AddCornerCode(string newCode, Action codeAction)
    {
        if (!addedCornerCodeButtons)
        {
            AddButtons();
        }
        cornerCodeListeners.Add(newCode, codeAction);
    }



    private void ResetCornerCode()
    {
        cornerCode = "";
    }


    public void BringToFront()
    {
        if (addedCornerCodeButtons)
        {
            cornerCodeTestParent.transform.SetAsLastSibling();
        }
    }

    private void CornerCodeClicked(string buttonIndex)
    {
        cornerCode += buttonIndex;

        if (ExhibitBase.instance.verbose)
            Debug.Log("cornerCode: " + cornerCode);

        if (cornerCodeTimeoutCoroutine != null) { StopCoroutine(cornerCodeTimeoutCoroutine); cornerCodeTimeoutCoroutine = null; }

        if (cornerCodeListeners.ContainsKey(cornerCode))
        {
            cornerCodeListeners[cornerCode]();
            ResetCornerCode();
        }
        else
        {
            //cornerCodeTimeoutCoroutine = StartCoroutine(CornerCodeTimeout());
            cornerCodeTimeoutCoroutine = StartCoroutine(ExhibitUtilities.DoActionAfterTime(ResetCornerCode, cornerCodeTimeoutSeconds));
        }
    }





    public void SetColor(Color c)
    {
        cornerCodeTestColor = c;
        bool setActive = c == Color.clear;

        if (addedCornerCodeButtons)
        {
            for (int i = 0; i < cornerCodeTestImages.Count; i++)
            {
                cornerCodeTestImages[i].color = cornerCodeTestColor;
                cornerCodeTestImages[i].gameObject.SetActive(setActive);
            }
        }
    }

    public void SetTimeoutSeconds(float ccTimeoutSeconds)
    {
        cornerCodeTimeoutSeconds = ccTimeoutSeconds;
    }

    public void SetButtonSize(float ccButtonSize)
    {
        cornerCodeButtonSize = ccButtonSize;

        if (addedCornerCodeButtons)
        {
            for (int i = 0; i < cornerCodeTestImages.Count; i++)
            {
                RectTransform buttonRectTransform = cornerCodeTestImages[i].gameObject.GetComponent<RectTransform>();
                buttonRectTransform.sizeDelta = new Vector2(cornerCodeButtonSize, cornerCodeButtonSize);
            }

            CanvasScaler canvasScaler = FindObjectOfType<Canvas>().GetComponent<CanvasScaler>();
            Vector2 resolution = canvasScaler.referenceResolution;
            cornerCodeRects[0] = new Rect(0, resolution.y - cornerCodeButtonSize, cornerCodeButtonSize, cornerCodeButtonSize); // top-left
            cornerCodeRects[1] = new Rect(resolution.x - cornerCodeButtonSize, resolution.y - cornerCodeButtonSize, cornerCodeButtonSize, cornerCodeButtonSize); // top-right
            cornerCodeRects[2] = new Rect(0, 0, cornerCodeButtonSize, cornerCodeButtonSize); // bottom-left
            cornerCodeRects[3] = new Rect(resolution.x - cornerCodeButtonSize, 0, cornerCodeButtonSize, cornerCodeButtonSize); // bottom-right
        }
    }



    /// <summary>
    /// adds the corner overlay buttons in order to add Code Sequences
    /// </summary>
    public void AddButtons()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            Debug.Log("Creating a Canvas object in the Scene for the corner code buttons.");
            canvas = ExhibitBase.CreateCanvas(1920, 1080);
        }

        cornerCodeTestParent = new GameObject();
        cornerCodeTestParent.name = "Corner Codes";
        RectTransform cornerCodeRectTransform = cornerCodeTestParent.AddComponent<RectTransform>();
        cornerCodeRectTransform.position.Set(0, 0, 0);
        cornerCodeRectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        cornerCodeRectTransform.SetParent(canvas.GetComponent<RectTransform>(), false);


        GameObject[] buttons = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject buttonGameObject = new GameObject();
            buttonGameObject.name = "Corner Code Button " + i;
            RectTransform buttonRectTransform = buttonGameObject.AddComponent<RectTransform>();
            buttonRectTransform.SetParent(cornerCodeRectTransform);
            buttonRectTransform.sizeDelta = new Vector2(cornerCodeButtonSize, cornerCodeButtonSize);
            Image buttonImage = buttonGameObject.AddComponent<Image>();
            buttonImage.color = cornerCodeTestColor;
            buttons[i] = buttonGameObject;
            cornerCodeTestImages.Add(buttonImage);
        }

        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        Vector2 resolution = canvasScaler.referenceResolution;

        // set positions of test images
        buttons[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(-(resolution.x / 2) + (cornerCodeButtonSize / 2), (resolution.y / 2) - (cornerCodeButtonSize / 2));
        buttons[1].GetComponent<RectTransform>().anchoredPosition = new Vector3((resolution.x / 2) - (cornerCodeButtonSize / 2), (resolution.y / 2) - (cornerCodeButtonSize / 2));
        buttons[2].GetComponent<RectTransform>().anchoredPosition = new Vector3(-(resolution.x / 2) + (cornerCodeButtonSize / 2), -(resolution.y / 2) + (cornerCodeButtonSize / 2));
        buttons[3].GetComponent<RectTransform>().anchoredPosition = new Vector3((resolution.x / 2 - (cornerCodeButtonSize / 2)), -(resolution.y / 2) + (cornerCodeButtonSize / 2));

        // create rectangles
        cornerCodeRects.Add(new Rect(0, resolution.y - cornerCodeButtonSize, cornerCodeButtonSize, cornerCodeButtonSize)); // top-left
        cornerCodeRects.Add(new Rect(resolution.x - cornerCodeButtonSize, resolution.y - cornerCodeButtonSize, cornerCodeButtonSize, cornerCodeButtonSize)); // top-right
        cornerCodeRects.Add(new Rect(0, 0, cornerCodeButtonSize, cornerCodeButtonSize)); // bottom-left
        cornerCodeRects.Add(new Rect(resolution.x - cornerCodeButtonSize, 0, cornerCodeButtonSize, cornerCodeButtonSize)); // bottom-right

        if (cornerCodeTestColor == Color.clear)
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }

        addedCornerCodeButtons = true;

        StartCoroutine(ExhibitUtilities.DoActionAfterTime(BringToFront, 5f));
    }
}
