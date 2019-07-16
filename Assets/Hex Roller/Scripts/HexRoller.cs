using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexRoller : MonoBehaviour
{
    public HexGameBoard gameBoard;



    private void Start()
    {
        gameBoard.TileClicked.AddListener(TileClicked);
    }


    private void TileClicked(HexType hexType, Vector3Int position)
    {
        Debug.Log(hexType.name + " clicked.");
    }
}
