using System.IO;
using NiceJson;

public class ConfigHelper
{
    /// <summary>
    /// Loads and Parses an external JSON file
    /// </summary>
    /// <param name="filepath">the location of the json file</param>
    /// <returns>a JsonNode of parsed JSON</returns>
    public static JsonNode Load(string filepath)
    {
        JsonNode jsonConfig = null;
        using (StreamReader sr = new StreamReader(filepath))
        {
            string json = sr.ReadToEnd();
            jsonConfig = JsonNode.ParseJsonString(json);
        }
        return jsonConfig;
    }

    /// <summary>
    /// Saves a JsonNode to a file.  If the file exists already, it is overwritten.
    /// </summary>
    /// <param name="config">the JsonNode to save</param>
    /// <param name="filepath">location to be saved</param>
    /// <param name="prettyPrint">if the file should be human readable or not</param>
    public static void Save(JsonNode config, string filepath, bool prettyPrint = true)
    {
        if (prettyPrint) { File.WriteAllText(filepath, config.ToJsonPrettyPrintString()); }
        else { File.WriteAllText(filepath, config.ToJsonString()); }
    }


    /// <summary>
    /// check if a file exists
    /// </summary>
    /// <param name="filepath">the location of the json file</param>
    /// <returns>true if the file exists</returns>
    public static bool ConfigExists(string filepath)
    {
        return File.Exists(filepath);
    }
}
