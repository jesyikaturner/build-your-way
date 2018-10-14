using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Random AI
 */ 
public class ComputerPlayer : MonoBehaviour {

    private BoardManager boardManager;
    public PlaceScript tile;
    public List<PlaceScript> possibleMoves;
    public bool CannotPlaceTile = false;
    private float timer = 0;

    void Start ()
    {
        boardManager = transform.GetComponent<BoardManager>();
        possibleMoves = new List<PlaceScript>();
    }

    void Update ()
    {
        timer += Time.deltaTime;
        if(boardManager.curr_player == 2)
        {
            if(timer > 1f)
            {
                if (!PlaceTile())
                    if (!MoveAttacker())
                        DestroyTile();
                timer = 0;
            }

        }  
    }

    /* Goes through the steps of random selecting, add the possible 
     * moves to a list then random selecting one of those for the tile to be moved to.
     */ 
    private bool PlaceTile()
    {
        possibleMoves.Clear();
        tile = boardManager.playerHands[Random.Range(0,boardManager.playerHands.Length-1)];

        while(tile.playerHand != 2)
        {
            tile = boardManager.playerHands[Random.Range(0, boardManager.playerHands.Length-1)];
        }

        boardManager.PossibleTilePlacements();

        foreach(PlaceScript cell in boardManager.boardArray)
        {
            if(cell.blockType == -5)
            {
                possibleMoves.Add(cell);
            }
        }

        if (possibleMoves.Count > 0)
        {
            possibleMoves[Random.Range(0, possibleMoves.Count - 1)].blockType = tile.blockType;
            boardManager.SubtractMove(1);
        }
        else
            return false;
        tile = null;
        boardManager.ClearBoard();
        return true;
    }

    /*
     * Same as the move tile, except checking which tiles have the p2 attacker,
     * selecting the tiles
     */ 
    private bool MoveAttacker()
    {
        possibleMoves.Clear();
        foreach (PlaceScript cell in boardManager.boardArray)
        {
            if(cell.GetAttacker() && cell.GetAttacker().team == 2)
            {
                possibleMoves.Add(cell);
            }
        }
        if(possibleMoves.Count == 0)
            return false;

        tile = possibleMoves[Random.Range(0, possibleMoves.Count)];
        possibleMoves.Clear();
        boardManager.PossibleAttackerMoves(tile);

        foreach (PlaceScript cell in boardManager.boardArray)
        {
            if(cell.blockType == -1 || cell.blockType == -3 || cell.blockType == -4)
            {
                possibleMoves.Add(cell);
            }
        }

        if (possibleMoves.Count == 0)
            return false;

        PlaceScript place = possibleMoves[Random.Range(0, possibleMoves.Count - 1)];

        place.SetAttacker(tile.GetAttacker());
        place.GetAttacker().transform.position = new Vector3(place.transform.position.x, boardManager.pieceOffset, place.transform.position.z);
        tile.SetAttacker(null);
        boardManager.ClearBoard();
        boardManager.SubtractMove(1);
        tile = null;
        return true;
    }

    public bool DestroyTile()
    {
        possibleMoves.Clear();
        boardManager.ShowBreakableTiles();
        foreach(PlaceScript cell in boardManager.boardArray)
        {
            if (cell.isBreakable)
            {
                possibleMoves.Add(cell);
            }
        }

        PlaceScript place = possibleMoves[Random.Range(0, possibleMoves.Count - 1)];

        if (place.blockType == 1)
        {
            boardManager.SubtractMove(1);
            place.blockType = 0;
        }
        if (place.blockType == 2)
        {
            boardManager.SubtractMove(2);
            place.blockType = 0;
        }
        boardManager.ClearBoard();

        return true;
    }
}
