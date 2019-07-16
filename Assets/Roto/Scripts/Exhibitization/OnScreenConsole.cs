using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenConsole : MonoBehaviour
{
    public const int FONT_SIZE = 23;


    [HideInInspector]
    public GameObject ui;
    private RectTransform uiRT;
    private float uiWidth = 300;
    private float logHeight = 30;


    private int maxShownLogs = 36;
    private bool showStackTrace = false;
    public bool ShowStackTrace
    {
        get { return showStackTrace; }
        set { showStackTrace = value; logHeight = (showStackTrace) ? 71 : 30; maxShownLogs = (showStackTrace) ? 15 : 36; }
    }

    private Color bgColor1, bgColor2, lastColor;


    private bool active = false;
    private List<LogMessage> messages = new List<LogMessage>();
    public Dictionary<LogType, string> logtypeColors;


    private void Start()
    {
        // listen for logs
        Application.logMessageReceived += LogMessageRecieved;
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) )
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                active = !active;
                ui.SetActive(active);
                if (active) { transform.SetAsLastSibling(); }
            }
        }
    }


    private void OnDestroy()
    {
        Application.logMessageReceived -= LogMessageRecieved;
    }




    public void SetTextColor(Color color, LogType logType)
    {
        string colorString = "#" + ColorUtility.ToHtmlStringRGB(color);

        if (logtypeColors.ContainsKey(logType))
        {
            logtypeColors[logType] = colorString;
        }
        else
        {
            logtypeColors.Add(logType, colorString);
        }
    }

    public void SetTextColor(Color color)
    {
        SetTextColor(color, LogType.Log);
    }


    public void SetBackgroundColor(Color color)
    {
        ui.GetComponent<Image>().color = color;

        float h, s, v; Color.RGBToHSV(color, out h, out s, out v);
        if (v >= 0.5f)
        {
            v -= 0.05f; bgColor1 = Color.HSVToRGB(h, s, v);
            v -= 0.05f; bgColor2 = Color.HSVToRGB(h, s, v);
        }
        else
        {
            v += 0.05f; bgColor1 = Color.HSVToRGB(h, s, v);
            v += 0.05f; bgColor2 = Color.HSVToRGB(h, s, v);
        }
    }






    private void LogMessageRecieved(string condition, string stackTrace, LogType type)
    {
        messages.Add(new LogMessage(condition, stackTrace, type));
        if (messages.Count > maxShownLogs)
        {
            GameObject.Destroy(messages[0].ui);
            messages.RemoveAt(0);
        }

        AddLogMessage(messages[messages.Count - 1]);
    }


    private void AddLogMessage(LogMessage message)
    {
        // Log Item Container
        GameObject messageContainer = new GameObject();
        messageContainer.name = "Message";
        RectTransform messageContainerRT = messageContainer.AddComponent<RectTransform>();
        messageContainerRT.SetParent(uiRT);
        messageContainerRT.sizeDelta = new Vector2(uiWidth, logHeight);
        Image messageContainerImage = messageContainer.AddComponent<Image>();
        messageContainerImage.color = (lastColor == bgColor1) ? bgColor2 : bgColor1; lastColor = messageContainerImage.color;
        messageContainerImage.raycastTarget = false;


        // Text
        GameObject textGO = new GameObject();
        textGO.name = "On-Screen Console Text";
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.SetParent(messageContainerRT);
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.pivot = new Vector2(0.5f, 0.5f);
        textRT.anchoredPosition = new Vector2(2.5f, 0);
        Text text = textGO.AddComponent<Text>();
        Font newFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = newFont;
        text.material = newFont.material;
        text.fontSize = FONT_SIZE;
        text.color = Color.black;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        text.text = message.ToString(this);

        message.ui = messageContainer;
    }




    public static OnScreenConsole Create(Canvas canvas, float width)
    {
        // Main Object
        GameObject mainGO = new GameObject();
        mainGO.name = "On-Screen Console";
        RectTransform mainRT = mainGO.AddComponent<RectTransform>();
        mainRT.SetParent(canvas.transform);
        mainRT.sizeDelta = new Vector2(width, Screen.height);
        mainRT.anchorMin = new Vector2(0.5f, 0.5f);
        mainRT.anchorMax = new Vector2(0.5f, 0.5f);
        mainRT.pivot = new Vector2(0, 0.5f);
        mainRT.anchoredPosition = new Vector2(-Screen.width / 2f, 0);
        OnScreenConsole osc = mainGO.AddComponent<OnScreenConsole>();

        // Background
        GameObject uiGO = new GameObject();
        uiGO.name = "On-Screen Console UI";
        RectTransform uiRT = uiGO.AddComponent<RectTransform>();
        uiRT.SetParent(mainRT);
        uiRT.sizeDelta = new Vector2(width, Screen.height);
        uiRT.anchorMin = new Vector2(0.5f, 0.5f);
        uiRT.anchorMax = new Vector2(0.5f, 0.5f);
        uiRT.pivot = new Vector2(0.5f, 0.5f);
        uiRT.anchoredPosition = new Vector2(0, 0);
        Image uiImage = uiGO.AddComponent<Image>();
        uiImage.raycastTarget = false;
        VerticalLayoutGroup uiLG = uiGO.AddComponent<VerticalLayoutGroup>();
        uiLG.padding = new RectOffset(0, 0, 0, 0);
        uiLG.childAlignment = TextAnchor.UpperLeft;
        uiLG.childControlWidth = true; uiLG.childControlHeight = false;
        uiLG.childForceExpandWidth = true; uiLG.childForceExpandHeight = false;
        osc.ui = uiGO;
        osc.uiRT = uiRT;
        uiGO.gameObject.SetActive(false);

        osc.uiWidth = width;

        // log colors
        osc.logtypeColors = new Dictionary<LogType, string>();
        osc.logtypeColors.Add(LogType.Log, "#011A37");
        osc.logtypeColors.Add(LogType.Error, "#FB2650");
        //osc.logtypeColors.Add(LogType.Warning, "#FFC254");
        osc.logtypeColors.Add(LogType.Warning, "#FDB636");
        osc.logtypeColors.Add(LogType.Exception, "#FB2650");
        osc.logtypeColors.Add(LogType.Assert, "#FB2650");

        // set bg colors
        osc.SetBackgroundColor(new Color(0.996f, 0.996f, 0.996f));
        

        return osc;
    }

    
}


public class LogMessage
{
    public string condition;
    public string stackTrace;
    public LogType type;

    public GameObject ui;


    public LogMessage(string condition, string stackTrace, LogType type)
    {
        this.condition = condition;
        this.stackTrace = stackTrace;
        this.type = type;
    }

    public string ToString(OnScreenConsole osc)
    {
        string logString = "<color=" + osc.logtypeColors[type] + ">" + condition + "</color>";
        if (osc.ShowStackTrace)
        {
            logString = "<color=" + osc.logtypeColors[type] + ">" + condition + "\n<size=" + (OnScreenConsole.FONT_SIZE - 5).ToString() + ">" + stackTrace + "</size></color>";
        }

        return logString;
    }
}
