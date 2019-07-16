using UnityEngine;
using System.IO;
using UnityEditor;

/// <summary>
/// Creates a texture from an animation curve
/// </summary>
public class AnimationCurveToGradientTexture : EditorWindow
{
    public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
    public Vector2 TextureSize = new Vector2(512, 2);
    public Texture2D Result;

    [MenuItem("Tools/Roto/Bake Curve into Texture")]
    static void Init()
    {
        AnimationCurveToGradientTexture window = GetWindow<AnimationCurveToGradientTexture>();
        window.Show();
    }


    private void OnGUI()
    {
        TextureSize = EditorGUILayout.Vector2Field("Texture size", TextureSize);
        Curve = EditorGUILayout.CurveField("Animation curve", Curve);
        Result = EditorGUILayout.ObjectField("Result", Result, typeof(Texture2D), true) as Texture2D;
        if (GUILayout.Button("Bake"))
        {
            Bake();
        }
    }

    public void Bake()
    {
        //create a new texture
        Texture2D gradientTex = new Texture2D((int)TextureSize.x, (int)TextureSize.y, TextureFormat.ARGB32, false);

        //fill with with the animation curve data
        for (int x = 0; x < gradientTex.width; x++)
        {
            float curveValue = Curve.Evaluate((float)x / gradientTex.width);
            for (int y = 0; y < gradientTex.height; y++)
            {
                gradientTex.SetPixel(x, y, new Color(curveValue, curveValue, curveValue, 1));
            }
        }

        //if the texture is empty, prompt the save file panel to get a path
        string path = null;
        if (Result == null)
        {
            path = EditorUtility.SaveFilePanel("Save gradient texture...", "", "GradientTexture.png", "png");
        }
        //or retrieve it from the texture set in the texture field
        else
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(Result));
            path.Replace("\\", "/");
        }

        //if the path is correct, write the texture into an asset
        if (path.Length > 0)
        {
            byte[] data = gradientTex.EncodeToPNG();
            if (data != null)
            {
                File.WriteAllBytes(path, data);
                AssetDatabase.Refresh();
                path = "Assets" + path.Split(new string[] { "Assets" }, System.StringSplitOptions.None)[1];
                //change the texture settings, feel free to change it to whatever you want to.
                TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
                if (importer != null)
                {
                    importer.sRGBTexture = false;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.mipmapEnabled = false;
                    importer.wrapMode = TextureWrapMode.Clamp;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    Texture2D result = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                    Result = result;
                    //ping the object on the project you can see where it is, and drag it directly to a material
                    EditorGUIUtility.PingObject(result);
                }
            }
        }
    }


}
