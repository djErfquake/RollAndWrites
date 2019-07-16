using NiceJson;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[HelpURL("https://code.roto.com/standard-applications/unity-roto-base#on-screen-keyboard")]
[AddComponentMenu("Roto/Keyboard")]
public class Keyboard : MonoBehaviour
{
    // keyboard and layout information
    public List<RectTransform> keyboardContainers = new List<RectTransform>();
    private Dictionary<string, Transform> layouts = new Dictionary<string, Transform>();
    private string currentLayout;

    // set color (normally clear, but can be other colors for debugging)
    public Color keyColor = Color.clear;

    // spacing
    private float spacing;
    private Vector2 defaultSize;
    private Dictionary<string, Vector2> otherSizes = new Dictionary<string, Vector2>();
    

    public KeyboardEvent keyboardEvents = new KeyboardEvent();




    public void Setup(JsonNode keyboardInfo)
    {
        // load button spacing
        spacing = keyboardInfo["spacing"];

        // keep track of button sizes
        JsonArray buttonSizes = keyboardInfo["button-sizes"] as JsonArray;
        foreach (JsonNode buttonSize in buttonSizes)
        {
            if (buttonSize["name"] == "default")
            {
                defaultSize = new Vector2(buttonSize["width"], buttonSize["height"]);
            }
            else
            {
                otherSizes.Add(buttonSize["name"], new Vector2(buttonSize["width"], buttonSize["height"]));
            }
        }

        // create buttons
        JsonArray layoutInfo = keyboardInfo["layouts"] as JsonArray;
        for (int i = 0; i < layoutInfo.Count; i++)
        {
            // parse json for layout information
            JsonArray keyboardRows = layoutInfo[i]["rows"] as JsonArray;
            AddKeyboard(keyboardRows, keyboardContainers[i]);

            // add to data structure
            layouts.Add(layoutInfo[i]["name"], keyboardContainers[i]);
        }

        // set intial keyboard
        currentLayout = layoutInfo[0]["name"];
        SetLayout(currentLayout);
    }



    public void SetLayout(string newLayout)
    {
        currentLayout = newLayout;
        foreach (KeyValuePair<string, Transform> layout in layouts)
        {
            layout.Value.gameObject.SetActive(false);
        }
        layouts[currentLayout].gameObject.SetActive(true);
    }

    public string CurrentLayout()
    {
        return currentLayout;
    }



    private void AddKeyboard(JsonArray rows, Transform container)
    {
        Navigation buttonNavNone = new Navigation();
        buttonNavNone.mode = Navigation.Mode.None;
        float x = 0f, y = 0f;
        for (int r = 0; r < rows.Count; r++)
        {
            x = rows[r]["x"];
            y = rows[r]["y"];

            JsonArray keys = rows[r]["keys"] as JsonArray;
            for (int k = 0; k < keys.Count; k++)
            {
                string keyName = "BLANK";
                Vector2 buttonSize = defaultSize;

                if (keys[k].ContainsKey("size"))
                {
                    keyName = keys[k]["text"];
                    buttonSize = otherSizes[keys[k]["size"]];
                }
                else
                {
                    keyName = keys[k];
                }

                // create GameObject
                GameObject keyboardKey = new GameObject();
                keyboardKey.name = "Keyboard Key " + keyName;

                RectTransform rectTransform = keyboardKey.AddComponent<RectTransform>();
                rectTransform.SetParent(container);
                rectTransform.localScale = Vector3.one;
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
                rectTransform.sizeDelta = buttonSize;
                rectTransform.anchoredPosition = new Vector2(x, y);

                Image image = keyboardKey.AddComponent<Image>();
                image.color = keyColor;

                //Button button = keyboardKey.AddComponent<Button>();
                //button.navigation = buttonNavNone;
                //button.onClick.AddListener(() => {
                //    keyboardEvents.Invoke(keyName);
                //});
                EventTrigger eventTrigger = keyboardKey.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) =>
                {
                    keyboardEvents.Invoke(keyName);
                });
                eventTrigger.triggers.Add(entry);

                x += buttonSize.x + spacing;
            }


        }
    }


}

public class KeyboardEvent : UnityEvent<string> { }
