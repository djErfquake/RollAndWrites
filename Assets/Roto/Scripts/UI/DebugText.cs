using UnityEngine;
using UnityEngine.UI;

[HelpURL("https://code.roto.com/standard-applications/unity-roto-base#debugtext")]
[AddComponentMenu("Roto/Debug Text")]
public class DebugText : MonoBehaviour
{
    private Text textElement;
    private float fadeTime = 5f;
    public bool verbose = false;

    private Coroutine fadeCoroutine;

    private Color fontColor = Color.black;
    private string fontType = "Arial.ttf";
    private int fontSize = 90;


    public static DebugText Create()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("Creating a Canvas object in the Scene for DebugText.");
            canvas = ExhibitBase.CreateCanvas(1920, 1080);
        }

        GameObject textGO = new GameObject();

        RectTransform textTransfrom = textGO.AddComponent<RectTransform>();
        textTransfrom.anchorMin = Vector2.zero;
        textTransfrom.anchorMax = Vector2.one;
        textTransfrom.pivot = new Vector2(0.5f, 0.5f);
        textTransfrom.offsetMin = Vector2.zero;
        textTransfrom.offsetMax = new Vector2(canvas.pixelRect.width, canvas.pixelRect.height);

        DebugText dt = textGO.AddComponent<DebugText>();

        textGO.name = "Debug Text";
        textTransfrom.SetParent(canvas.gameObject.transform);

        dt.textElement = textGO.AddComponent<Text>();

        dt.SetFont(dt.fontType);
        dt.SetColor(dt.fontColor);
        dt.SetFontSize(dt.fontSize);
        dt.textElement.raycastTarget = false;

        return dt;
    }


    public void Log(string message)
    {
        if (verbose) { Debug.Log(message); }
        
        textElement.text = message;
        ShowTextElement();
    }

    public void LogAndFade(string message)
    {
        Log(message);

        if (fadeCoroutine != null) { StopCoroutine(fadeCoroutine); fadeCoroutine = null; }
        fadeCoroutine = StartCoroutine(ExhibitUtilities.DoActionAfterTime(HideTextElement, fadeTime));
    }

    public void LogAndFade(string message, float messageFadeTime)
    {
        Log(message);

        if (fadeCoroutine != null) { StopCoroutine(fadeCoroutine); fadeCoroutine = null; }
        StartCoroutine(ExhibitUtilities.DoActionAfterTime(HideTextElement, messageFadeTime));
    }

    public void ShowTextElement()
    {
        textElement.gameObject.SetActive(true);
        textElement.transform.SetAsLastSibling();
    }

    public void HideTextElement()
    {
        textElement.gameObject.SetActive(false);
    }

    public void SetColor(Color textColor)
    {
        fontColor = textColor;
        if (textElement != null)
        {
            textElement.color = fontColor;
        }
    }

    public void SetFont(string textFont)
    {
        Font newFont = (Font)Resources.GetBuiltinResource(typeof(Font), textFont);
        textElement.font = newFont;
        textElement.material = newFont.material;
        fontType = textFont;

        if (textElement != null)
        {
            textElement.font = newFont;
        }
    }

    public void SetFontSize(int textSize)
    {
        fontSize = textSize;
        if (textElement != null)
        {
            textElement.fontSize = textSize;
        }
    }





    // singleton
    public static DebugText instance;
    private void Awake()
    {
        if (instance && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

}
