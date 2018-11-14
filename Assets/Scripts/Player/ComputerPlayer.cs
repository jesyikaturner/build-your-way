using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Very basic random AI Computer Player.
 */ 
public class ComputerPlayer : MonoBehaviour, IPlayer {

    // Public Variables for the Inspector
    [SerializeField]
    private Tile selected;

    // Constants
    private const float moveDelay = 0.4f;
    private const int MAX_MOVE_HISTORY = 2;

    // Private Variables
    private BoardManager boardManager;
    private SoundManager soundManager;
    private PlayerHand compHand;
    private List<Tile> possibleMoves, goalTiles, moveHistory;
    private int playerID;

    public void SetupPlayerControls(SoundManager soundManager, BoardManager boardManager, int playerID)
    {
        this.boardManager = boardManager;
        this.soundManager = soundManager;
        this.playerID = playerID;

        possibleMoves = new List<Tile>();
        goalTiles = new List<Tile>();
        moveHistory = new List<Tile>();
        compHand = boardManager.GetHands()[playerID-1];

        // Getting the tiles that the attackers need to move to.
        foreach(Tile cell in boardManager.GetBoardArray())
        {
            if (playerID == 2 && cell.GetState("BASE1"))
                goalTiles.Add(cell);

            if (playerID == 1 && cell.GetState("BASE2"))
                goalTiles.Add(cell);
        }

        StartCoroutine(ComputerLogic());
    }

    private IEnumerator ComputerLogic()
    {
        while(!boardManager.IsPaused)
        {
            yield return new WaitForSeconds(moveDelay);
            if(boardManager.GetCurrPlayer() == playerID)
            {
                if (!DestroyTile(null))
                {
                    if (!MoveTile(null))
                    {
                        MoveAttacker(null);
                    }

                }

            }
        }
    }
    /* Goes through the steps of random selecting, add the possible 
     * moves to a list then random selecting one of those for the tile to be moved to.
     */
    public bool MoveTile(Tile place)
    {
        possibleMoves.Clear();
        selected = compHand.GetPlayerHand()[Random.Range(0, compHand.GetPlayerHand().Length)];

        boardManager.PossibleTilePlacements();

        foreach(Tile cell in boardManager.GetBoardArray())
        {
            if(cell.isSelected)
                possibleMoves.Add(cell);
        }

        if (possibleMoves.Count == 0)
            return false;

        Tile selectedMove = GetClosestPosition(possibleMoves);
        if (selectedMove)
            selectedMove.SetState(selected.GetState());
        else
            possibleMoves[Random.Range(0, possibleMoves.Count)].SetState(selected.GetState());

        // PLAY SOUND
        soundManager.PlaySound("SELECT");

        UpdateHistory(selectedMove);
        boardManager.SubtractMove(1);


        selected.SetState("EMPTY");
        selected = null;
        boardManager.ClearBoard();
        return true;
    }

    /*
     * Keeps the movehistory to a set size. If it exceeds the limit, then it clears
     * the list and adds the new entry.
     */ 
    private void UpdateHistory(Tile selectedMove)
    {
        if(moveHistory.Count < MAX_MOVE_HISTORY)
        {
            moveHistory.Add(selectedMove);
        }
        else
        {
            moveHistory.Clear();
            moveHistory.Add(selectedMove);
        }
    }

    /*
     * Checks if a move in the possiblemoves list is already in the movehistory list.
     */ 
    private bool CheckHistory(List<Tile> possibleMoves)
    {
        foreach(Tile cell in possibleMoves)
        {
            if (moveHistory.Contains(cell))
                return true;
        }
        return false;
    }

    /*
     * Gets the closest tile out of the possiblemoves to the goaltiles
     */ 
    private Tile GetClosestPosition(List<Tile> possiblePlaces)
    {
        Tile selectedPlace = null;
        float distance = float.MaxValue;
        foreach (Tile cell in goalTiles)
        {
            foreach(Tile place in possiblePlaces)
            {
                if(Vector3.Distance(cell.transform.position,place.transform.position) < distance)
                {
                    selectedPlace = place;
                    distance = Vector3.Distance(cell.transform.position, place.transform.position);
                }
            }
        }
        return selectedPlace;
    }

    /*
     * Same as the move tile, except checking which tiles have the p2 attacker,
     * selecting the tiles
     */ 
    public bool MoveAttacker(Tile place)
    {
        // clear the list for possible moves
        possibleMoves.Clear();

        // add possiblee moves that are adjacent to the attacker pieces
        foreach (Tile cell in boardManager.GetBoardArray())
        {
            if(cell.GetAttacker() && cell.GetAttacker().Team == playerID)
            {
                // make sure its not already on the goal tiles.
                if(playerID == 2 && !cell.GetState("BASE1"))
                    possibleMoves.Add(cell);

                if (playerID == 1 && !cell.GetState("BASE2"))
                    possibleMoves.Add(cell);
            }
        }
        // if there's no possible moves, exit out of the method.
        if(possibleMoves.Count == 0)
            return false;

        Tile selectedMove = null;
        
        // checks to see if any of the possible moves are already in the history
        if (!CheckHistory(possibleMoves))
        {
            // if there isnt then it'll get the one closest to the goal tiles
            selectedMove = GetClosestPosition(possibleMoves);
            if (selectedMove)
                selected = selectedMove;
            else
                selected = possibleMoves[Random.Range(0, possibleMoves.Count)];
        }
        else
        {
            // if one of the moves are already in there, then it'll just choose randomly.
            selected = possibleMoves[Random.Range(0, possibleMoves.Count)];
        }


        possibleMoves.Clear();
        boardManager.PossibleAttackerMoves(selected);

        foreach (Tile cell in boardManager.GetBoardArray())
        {
            if(cell.isSelected)
                possibleMoves.Add(cell);

        }

        if (possibleMoves.Count == 0)
            return false;

        if (!CheckHistory(possibleMoves))
        {
            selectedMove = GetClosestPosition(possibleMoves);
            if (!selectedMove)
                selectedMove = possibleMoves[Random.Range(0, possibleMoves.Count)];
        }
        else
        {
            selectedMove = possibleMoves[Random.Range(0, possibleMoves.Count)];
        }

        selected.GetAttacker().ToggleSelect();
        boardManager.SetAttacker(selected, selectedMove);
        boardManager.ClearBoard();
        UpdateGoalTiles(selectedMove);
        UpdateHistory(selectedMove);
        boardManager.SubtractMove(1);

        // PLAY SOUND
        soundManager.PlaySound("SELECT");

        selected = null;
        return true;
    }

    private void UpdateGoalTiles(Tile selectedMove)
    {
        if (goalTiles.Contains(selectedMove))
            goalTiles.Remove(selectedMove);
    }

    public bool DestroyTile(Tile place)
    {
        possibleMoves.Clear();
        boardManager.ShowBreakableTiles();
        foreach(Tile cell in boardManager.GetBoardArray())
        {
            if (cell.isBreakable)
            {
                possibleMoves.Add(cell);
            }
        }

        if (possibleMoves.Count == 0)
            return false;

        Tile selectedMove = GetClosestPosition(possibleMoves);
        if(!selectedMove)
            selectedMove = possibleMoves[Random.Range(0, possibleMoves.Count)];

        if (selectedMove.GetState("WALK"))
            return false;

        if (selectedMove.GetState("BLOCK"))
        {
            boardManager.SubtractMove(2);
            selectedMove.SetState("EMPTY");
        }
        boardManager.ClearBoard();
        return true;
    }
}