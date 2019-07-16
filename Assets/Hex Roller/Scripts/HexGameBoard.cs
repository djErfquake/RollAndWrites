using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class HexGameBoard : MonoBehaviour
{
    [Header("Components")]
    public Tilemap tilemap;
    public Tilemap highlightTilemap;

    [Header("Hex Types")]
    public List<HexType> hexTypes;
    public Tile highlightTile;

    [Header("Sections")]
    public List<HexSection> sections = new List<HexSection>();
    public List<StarterHex> starterHexes = new List<StarterHex>();


    public TileEvent TileClicked = new TileEvent();



    private List<Vector3Int> highlightedTiles = new List<Vector3Int>();







    private void Start()
    {
        // setup board
        for (int i = 0; i < sections.Count; i++)
        {
            for (int j = 0; j < sections[i].coordinates.Count; j++)
            {
                tilemap.SetTile(sections[i].coordinates[j], sections[i].tile);
            }
        }

        for (int i = 0; i < starterHexes.Count; i++)
        {
            tilemap.SetTile(starterHexes[i].coordinate, starterHexes[i].hexType.tile);
        }
    }

    [ButtonMethod]
    public void GenerateMap()
    {
        Start();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePosition = new Vector3Int((int)pos.x, (int)pos.y, 0);
            TileBase tile = tilemap.GetTile(mousePosition);
 
            if (tile != null)
            {
                Debug.Log(tile.name + " clicked");
                for (int i = 0; i < hexTypes.Count; i++)
                {
                    if (tile.name == hexTypes[i].name)
                    {
                        TileClicked.Invoke(hexTypes[i], mousePosition);
                    }
                }
            }
        }
        
    }
    


    public void HighlightTiles(List<Vector3Int> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            highlightedTiles.Add(tiles[i]);
            highlightTilemap.SetTile(tiles[i], highlightTile);
        }
    }

    public void UnhighlightTiles()
    {
        for (int i = 0; i < highlightedTiles.Count; i++)
        {
            highlightTilemap.SetTile(highlightedTiles[i], null);
        }
        highlightedTiles.Clear();
    }


}


[System.Serializable]
public class HexSection
{
    public string name;
    public Tile tile;
    public Color color = Color.white;
    public List<Vector3Int> coordinates = new List<Vector3Int>();
}

[CreateAssetMenu(fileName = "New Hex Type", menuName = "Hex Type")]
public class HexType : ScriptableObject
{
    public int value = 3;
    public RuleTile tile;
}

[System.Serializable]
public class StarterHex
{
    public HexType hexType;
    public Vector3Int coordinate;
}

public class TileEvent : UnityEvent<HexType, Vector3Int>
{
    public HexType hexType;
    public Vector3Int coordinate;
}
