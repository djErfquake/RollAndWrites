using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class ExhibitUtilities
{

    /// <summary>
    /// Quits the application regardless if it is running from the Debugger or Live executable.
    /// </summary>
    public static void ExitApplication()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        //Debug.Log("Platform: " + Application.platform);
    }



    /// <summary>
    /// Coroutine that waits a set amount of time, then executes
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <param name="seconds">Number of seconds to wait before executing the action</param>
    /// <returns></returns>
    public static IEnumerator DoActionAfterTime(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }


    /// <summary>
    /// Coroutine that waits a set amount of time, executes, then repeats.  Similar to InvokeRepeating but uses a Coroutine instead
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <param name="startAfterSeconds">Number of seconds to wait before executing the action</param>
    /// <param name="loopingSeconds">Number of seconds to wait between executing the action again</param>
    /// <returns></returns>
    public static IEnumerator DoActionForever(Action action, float startAfterSeconds, float loopingSeconds)
    {
        if (startAfterSeconds > 0)
        {
            yield return new WaitForSeconds(startAfterSeconds);
        }
        
        while (true)
        {
            action();
            yield return new WaitForSeconds(loopingSeconds);
        }
    }


    public static float FPS()
    {
        return Mathf.Round(1 / Time.deltaTime);
    }




    // LOGGING


    public static void LogOnScreen(string message)
    {
        DebugText debugText = DebugText.instance;
        if (debugText == null) { debugText = DebugText.Create();}

        debugText.Log(message);
    }

    public static void LogOnScreenAndFade(string message, float messageFadeTime = -1)
    {
        DebugText debugText = DebugText.instance;
        if (debugText == null) { debugText = DebugText.Create(); }

        if (messageFadeTime > 0) { debugText.LogAndFade(message, messageFadeTime); }
        else { debugText.LogAndFade(message); }
    }

    public static void LogColor(Color textColor)
    {
        DebugText debugText = DebugText.instance;
        if (debugText == null) { debugText = DebugText.Create(); }

        debugText.SetColor(textColor);
    }

    public static void LogFont(string textFont)
    {
        DebugText debugText = DebugText.instance;
        if (debugText == null) { debugText = DebugText.Create(); }

        debugText.SetFont(textFont);
    }

    public static void LogSize(int textSize)
    {
        DebugText debugText = DebugText.instance;
        if (debugText == null) { debugText = DebugText.Create(); }

        debugText.SetFontSize(textSize);
    }

    public static void LogVerbose(bool verbose)
    {
        DebugText debugText = DebugText.instance;
        if (debugText == null) { debugText = DebugText.Create(); }

        debugText.verbose = verbose;
    }





    /// <summary>
    /// Adds an Event Trigger Unity Component to an object.  Good for assigning click event listeners during runtime.
    /// The callback passes the GameObject that was used during the event
    /// </summary>
    /// <param name="go">GameObject to contain the event handler</param>
    /// <param name="eventType">type of event to handle</param>
    /// <param name="callback">function to call when the event is triggered</param>
    public static void AddEventTrigger(GameObject go, EventTriggerType eventType, Action<GameObject> callback)
    {
        EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
        if (eventTrigger == null) { eventTrigger = go.AddComponent<EventTrigger>(); }

        EventTrigger.Entry eventEntry = new EventTrigger.Entry();
        eventEntry.eventID = eventType;
        eventEntry.callback.AddListener((eventData) => { callback(go); });

        eventTrigger.triggers.Add(eventEntry);
    }


    /// <summary>
    /// Adds an Event Trigger Unity Component to an object.  Good for assigning click event listeners during runtime.
    /// </summary>
    /// <param name="go">GameObject to contain the event handler</param>
    /// <param name="eventType">type of event to handle</param>
    /// <param name="callback">function to call when the event is triggered</param>
    public static void AddEventTrigger(GameObject go, EventTriggerType eventType, Action callback)
    {
        EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
        if (eventTrigger == null) { eventTrigger = go.AddComponent<EventTrigger>(); }

        EventTrigger.Entry eventEntry = new EventTrigger.Entry();
        eventEntry.eventID = eventType;
        eventEntry.callback.AddListener((eventData) => { callback(); });

        eventTrigger.triggers.Add(eventEntry);
    }







    /// <summary>
    /// Saves a screenshot of the current gamestate to a folder on the desktop.
    /// </summary>
    public static void Screenshot()
    {
        // create directory if it doesn't exist
        string screenshotDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Screenshots";
        Directory.CreateDirectory(screenshotDirectory);

        // make filename based on time
        string filename = DateTime.Now.ToString("MM-dd-yy_HH-mm-ss") + ".png";

        string fullFilename = screenshotDirectory + "/" + filename;
        ScreenCapture.CaptureScreenshot(fullFilename);

        if (ExhibitBase.instance != null && ExhibitBase.instance.verbose)
        {
            Debug.Log("SAVED SCREENSHOT TO " + fullFilename);
        }
    }

    /// <summary>
    /// Saves a screenshot of the current gamestate to a designated location
    /// </summary>
    /// <param name="filename"></param>
    public static void Screenshot(string filename)
    {
        ScreenCapture.CaptureScreenshot(filename);

        if (ExhibitBase.instance != null && ExhibitBase.instance.verbose)
        {
            Debug.Log("SAVED SCREENSHOT TO " + filename);
        }
    }


    /// <summary>
    /// Generates a Sprite object from an image filepath
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns>a Sprite</returns>
    public static Sprite LoadSpriteFromFile(string filepath)
    {
        Sprite sprite = null;

        if (File.Exists(filepath))
        {
            byte[] fileData = File.ReadAllBytes(filepath);
            Texture2D tex = new Texture2D(2, 2); // this will get automatically resized
            tex.LoadImage(fileData);

            sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        else
        {
            Debug.Log("ERROR LOADING FILE: [" + filepath + "] doesn't exist!");
        }
        return sprite;
    }



    /// <summary>
    /// Saves a render texture to a .png file
    /// </summary>
    /// <param name="saveRenderTexture">RenderTexture to save</param>
    /// <param name="saveFilepath">Location to save the the image file</param>
    public static void SaveRenderTextureToPNG(RenderTexture saveRenderTexture, string saveFilepath)
    {
        RenderTexture currentActiveRenderTexture = RenderTexture.active;
        RenderTexture.active = saveRenderTexture;

        Texture2D virtualPhoto = new Texture2D(saveRenderTexture.width, saveRenderTexture.height, TextureFormat.RGB24, false);
        virtualPhoto.ReadPixels(new Rect(0, 0, saveRenderTexture.width, saveRenderTexture.height), 0, 0);
        byte[] textureBytes = virtualPhoto.EncodeToPNG();

        if (!saveFilepath.EndsWith(".png") && !saveFilepath.EndsWith(".PNG"))
        {
            saveFilepath += ".png";
        }

        File.WriteAllBytes(saveFilepath, textureBytes);

        //Destroy(virtualPhoto);
        RenderTexture.active = currentActiveRenderTexture;
    }


    /// <summary>
    /// Saves a render texture to a .jpg file
    /// </summary>
    /// <param name="saveRenderTexture">RenderTexture to save</param>
    /// <param name="saveFilepath">Location to save the the image file</param>
    public static void SaveRenderTextureToJPG(RenderTexture saveRenderTexture, string saveFilepath)
    {
        RenderTexture currentActiveRenderTexture = RenderTexture.active;
        RenderTexture.active = saveRenderTexture;

        Texture2D virtualPhoto = new Texture2D(saveRenderTexture.width, saveRenderTexture.height, TextureFormat.RGB24, false);
        virtualPhoto.ReadPixels(new Rect(0, 0, saveRenderTexture.width, saveRenderTexture.height), 0, 0);
        byte[] textureBytes = virtualPhoto.EncodeToJPG();

        if (!saveFilepath.EndsWith(".jpg") && !saveFilepath.EndsWith(".JPG") && !saveFilepath.EndsWith(".jpeg") && !saveFilepath.EndsWith(".JPEG"))
        {
            saveFilepath += ".jpg";
        }

        File.WriteAllBytes(saveFilepath, textureBytes);

        //Destroy(virtualPhoto);
        RenderTexture.active = currentActiveRenderTexture;
    }






    /// <summary>
    /// Deletes old files in a given directory, and saves a set amount of more recent files.  It will also optionally empty the Windows Recycling Bin
    /// </summary>
    /// <param name="directory">directory where the files are</param>
    /// <param name="maxFilesToSave">keeps this many recently written files</param>
    /// <param name="emptyRecyclingBin"></param>
    /// <returns>the number of files deleted</returns>
    public static int MakeRoomOnHardDrive(string directory, int maxFilesToSave, bool emptyRecyclingBin = true)
    {
        int deleteCount = 0;
        IEnumerable<FileInfo> deleteFiles = new DirectoryInfo(@directory).GetFiles().OrderByDescending(x => x.LastWriteTime);
        //Debug.Log("Looking at " + deleteFiles.Count<FileInfo>() + " files to delete.");
        deleteFiles = deleteFiles.Skip(maxFilesToSave);
        foreach (FileInfo fileInfo in deleteFiles)
        {
            fileInfo.Delete();
            deleteCount++;
        }

        if (emptyRecyclingBin)
        {
            EmptyRecyclingBin(false, false);
        }

        return deleteCount;
    }


    /// <summary>
    /// Programmatically empties the Windows Recycling Bin
    /// </summary>
    /// <param name="playSound">plays the sound</param>
    /// <param name="showProgressGUI">shows the windows GUI</param>
    public static void EmptyRecyclingBin(bool playSound = false, bool showProgressGUI = false)
    {
        if (!playSound && !showProgressGUI)
        {
            SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOSOUND | RecycleFlag.SHERB_NOCONFIRMATION | RecycleFlag.SHERB_NOPROGRESSUI);
        }
        else if (playSound && !showProgressGUI)
        {
            SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOCONFIRMATION | RecycleFlag.SHERB_NOPROGRESSUI);
        }
        else if (!playSound && showProgressGUI)
        {
            SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOSOUND | RecycleFlag.SHERB_NOCONFIRMATION);
        }
        else
        {
            SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOCONFIRMATION);
        }
    }


    [System.Runtime.InteropServices.DllImport("Shell32.dll")]
    private static extern int SHEmptyRecycleBin(System.IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);
    private enum RecycleFlag : int
    {
        SHERB_NOCONFIRMATION = 0x00000001, // No confirmation, when emptying
        SHERB_NOPROGRESSUI = 0x00000001, // No progress tracking window during the emptying of the recycle bin
        SHERB_NOSOUND = 0x00000004 // No sound when the emptying of the recycle bin is complete
    }










    /// <summary>
    /// Mirrors the x direction of a Texture2D object
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static Texture2D FlipTextureX(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);

        int xN = original.width;
        int yN = original.height;

        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
            }
        }
        flipped.Apply();

        return flipped;
    }

    /// <summary>
    /// Mirrors the x direction of a Texture, given it's array of colors, width and height
    /// </summary>
    /// <param name="original"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Texture2D FlipTextureX(Color[] original, int width, int height)
    {
        Texture2D flipped = new Texture2D(width, height);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                flipped.SetPixel(width - i - 1, j, original[(j * width) + i]);
            }
        }
        flipped.Apply();

        return flipped;
    }

    /// <summary>
    /// Mirrors the y direction of a Texture2D object
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static Texture2D FlipTextureY(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);

        int xN = original.width;
        int yN = original.height;

        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                flipped.SetPixel(i, yN - j - 1, original.GetPixel(i, j));
            }
        }
        flipped.Apply();

        return flipped;
    }

    /// <summary>
    /// Mirrors the y direction of a Texture, given it's array of colors, width and height
    /// </summary>
    /// <param name="original"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Texture2D FlipTextureY(Color[] original, int width, int height)
    {
        Texture2D flipped = new Texture2D(width, height);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                flipped.SetPixel(i, height - j - 1, original[(j * width) + i]);
            }
        }
        flipped.Apply();

        return flipped;
    }


    



    /// <summary>
    /// changes the brightness of a color
    /// </summary>
    /// <param name="color">color to change</param>
    /// <param name="correctionFactor">ranges from -1 to 1, -1 being black, 1 being white</param>
    /// <returns></returns>
    public static Color ChangeColorBrightness(Color color, float correctionFactor)
    {
        float red = color.r;
        float green = color.g;
        float blue = color.b;

        if (correctionFactor < 0 && correctionFactor >= -1)
        {
            correctionFactor = 1 + correctionFactor;
            red *= correctionFactor;
            green *= correctionFactor;
            blue *= correctionFactor;
        }
        else if (correctionFactor <= 1)
        {
            red = (1 - red) * correctionFactor + red;
            green = (1 - green) * correctionFactor + green;
            blue = (1 - blue) * correctionFactor + blue;
        }

        return new Color(red, green, blue);
    }





    /// <summary>
    /// Return a Color with RGBA equal to 255,255,255,0
    /// </summary>
    /// <returns>White with Full Transparency</returns>
    public static Color noAlpha
    {
        get
        {
            Color noAlpha = Color.white;
            noAlpha.a = 0f;
            return noAlpha;
        }
    }












    /// <summary>
    /// returns whether or not the given string is a valid email address or not
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns></returns>
    public static bool IsValidEmailAddress(string emailAddress)
    {
        if (String.IsNullOrEmpty(emailAddress))
            return false;

        return System.Text.RegularExpressions.Regex.IsMatch(emailAddress,
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }



    

}
