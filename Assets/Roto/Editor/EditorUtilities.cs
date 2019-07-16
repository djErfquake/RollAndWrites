using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class EditorUtilities
{
    /// <summary>
    /// Update the timestamp on the executable after new build
    /// </summary>
    /// <param name="target"></param>
    /// <param name="pathToBuiltProject"></param>
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        File.SetLastWriteTime(pathToBuiltProject, System.DateTime.Now);
        Debug.Log("Project built. Write time set to " + System.DateTime.Now.ToShortDateString());
    }




    /// <summary>
    /// Adds a menu option to unity to create a blank configuration json file
    /// </summary>
    [MenuItem("Tools/Roto/Create Configuration File")]
    private static void CreateConfigurationFileFromMenu()
    {
        string configurationFileName = Application.streamingAssetsPath + "/config.json";

        // creates streaming assets directory if necessary
        Directory.CreateDirectory(Application.streamingAssetsPath);

        // only do something if a "config.json" file doesn't exists in StreamingAssets
        if (!File.Exists(configurationFileName))
        {
            using (FileStream fs = File.Create(configurationFileName))
            {
                byte[] startingText = new System.Text.UTF8Encoding(true).GetBytes("{\n\t\"timeout-seconds\": 40\n}");
                fs.Write(startingText, 0, startingText.Length);
            }

            AssetDatabase.ImportAsset("Assets/StreamingAssets/config.json");

            Debug.Log("File successfully created.");
        }
        else
        {
            Debug.Log(configurationFileName + " already exists!");
        }
    }

}
