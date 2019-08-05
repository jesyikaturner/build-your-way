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
    private const float moveDelay = 0.01f; // moveDelay for play should be 0.2f
    private const int MAX_INVALID_MOVES = 3;

    // Private Variables
    private BoardManager boardManager;
    private SoundManager soundManager;
    private PlayerHand compHand;
    [SerializeField]
    private List<Tile> possibleMoves, goalTiles;
    private int playerID;
    private int invalidMoveCounter;

    public void SetupPlayerControls(SoundManager soundManager, BoardManager boardManager, int playerID)
    {
        this.boardManager = boardManager;
        this.soundManager = soundManager;
        this.playerID = playerID;

        possibleMoves = new List<Tile>();
        goalTiles = new List<Tile>();

        compHand = boardManager.GetHands()[playerID-1];

        // Getting the tiles that the attackers need to move to.
        foreach(Tile cell in boardManager.GetBoardArray())
        {
            if (playerID == 2 && cell.GetState("BASE1"))
                goalTiles.Add(cell);

            if (playerID == 1 && cell.GetState("BASE2"))
                goalTiles.Add(cell);
        }

        StartComputerLogic();
    }

    public void StartComputerLogic()
    {
        if (boardManager.IsPaused())
            return;
        StartCoroutine(ComputerLogic());
    }

    private IEnumerator ComputerLogic()
    {
        while (!boardManager.IsPaused())
        {
            yield return new WaitForSeconds(moveDelay);
            if (boardManager.GetCurrPlayer() == playerID) 
            {
                int choice = Random.Range(1, 4); // Random number between 1 and 3

                switch (choice)
                {
                    case 1:
                        DestroyTile(null);
                        break;
                    case 2:
                        MoveTile(null);
                        break;
                    case 3:
                        MoveAttacker(null);
                        break;
                    default:
                        Debug.LogError(choice);
                        break;
                }

                /*
                if (!DestroyTile(null))
                {
                    if (!MoveTile(null))
                    {
                        MoveAttacker(null);
                    }
                }*/
            }
        }
    }

    /* Goes through the steps of random selecting, add the possible 
     * moves to a list then random selecting one of those for the tile to be moved to.
     */
    public bool MoveTile(Tile place)
    {
        if (invalidMoveCounter > MAX_INVALID_MOVES)
        {
            Debug.LogFormat("{0}: Exiting, too many invalid moves. Counter at {1}", playerID, invalidMoveCounter);
            return false;
        }

        Debug.LogFormat("{0}: Cleared possibleMoves array", playerID);
        possibleMoves.Clear();

        Debug.LogFormat("{0}: Showing possible moves.", playerID);
        boardManager.PossibleTilePlacements();

        Debug.LogFormat("{0}: Adding selected tiles to possibleMoves array.", playerID);
        foreach (Tile cell in boardManager.GetBoardArray())
        {
            if (cell.isSelected)
            {
                possibleMoves.Add(cell);
            }
        }
        Debug.LogFormat("{0}: Selected tiles all added.", playerID);

        if (possibleMoves.Count == 0)
        {
            Debug.LogFormat("{0}: Increasing invalidMoveCounter by 1. Counter at {1}", playerID, invalidMoveCounter);
            invalidMoveCounter++;

            Debug.LogFormat("{0}: Exiting out of MoveTile(), there's no possible moves", playerID);
            selected = null;
            boardManager.ClearBoard();
            return false;
        }

        Debug.LogFormat("{0}: Selected random card from own hand.", playerID);
        selected = compHand.GetPlayerHand()[Random.Range(0, compHand.GetPlayerHand().Length)];

        Debug.LogFormat("{0}: Selected closest empty place to goal tiles to place new tile.", playerID);
        Tile selectedMove = GetClosestPosition(possibleMoves);

        if (selectedMove)
        {
            Debug.LogFormat("{0}: Placed hand tile.", playerID);
            selectedMove.SetState(selected.GetState());
        }    
        else
        {
            Debug.LogErrorFormat("{0}: A null tile is being returned from selecting closest tile. Defaulting to random selection.", playerID);
            possibleMoves[Random.Range(0, possibleMoves.Count)].SetState(selected.GetState());
        }


        // PLAY SOUND
        soundManager.PlaySound("SELECT");

        boardManager.SubtractMove(1);

        selected.SetState("EMPTY");
        selected = null;
        boardManager.ClearBoard();
        return true;
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
        Debug.LogFormat("{0}: Cleared possibleMoves array", playerID);
        possibleMoves.Clear();

        // grabs all the attacker pieces that aren't on the goal tiles
        Debug.LogFormat("{0}: Getting all attacker pieces that aren't on goal tiles.", playerID);
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
        Debug.LogFormat("{0}: All attackers gathered.", playerID);

        // if there's no possible moves, exit out of the method.
        if (possibleMoves.Count == 0)
            return false;

        Tile selectedMove = null;

        foreach (Tile cell in possibleMoves)
        {
            if (!cell.GetAttacker().CheckHistory(possibleMoves))
            {
                selectedMove = GetClosestPosition(possibleMoves);
                break;
            }
        }

        if(selectedMove)
            selected = selectedMove;
        else
            selected = possibleMoves[Random.Range(0, possibleMoves.Count)];


        // checks to see if any of the possible moves are already in the history
        possibleMoves.Clear();
        boardManager.PossibleAttackerMoves(selected);

        foreach (Tile cell in boardManager.GetBoardArray())
        {
            if(cell.isSelected)
                possibleMoves.Add(cell);

        }

        if (possibleMoves.Count == 0)
            return false;


        selectedMove = GetClosestPosition(possibleMoves);
        if (!selectedMove || selected.GetAttacker().CheckHistory(selectedMove))
        {
            if(invalidMoveCounter > 1)
            {
                selectedMove = possibleMoves[Random.Range(0, possibleMoves.Count)];
                invalidMoveCounter++;
            }
            else
            {
                invalidMoveCounter++;
                boardManager.ClearBoard();
                return false;
            }
        }

        if (invalidMoveCounter > MAX_INVALID_MOVES)
        {
            foreach (Tile cell in possibleMoves)
            {
                selected.GetAttacker().ClearHistory(cell);
            }
            invalidMoveCounter = 0;
        }

        selected.GetAttacker().ToggleSelect();
        boardManager.SetAttacker(selected, selectedMove);
        boardManager.ClearBoard();
        UpdateGoalTiles(selectedMove);
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
        Debug.LogFormat("{0}: Cleared possibleMoves array", playerID);
        possibleMoves.Clear();

        Debug.LogFormat("{0}: Showing breakable tiles.", playerID);
        boardManager.ShowBreakableTiles();

        Debug.LogFormat("{0}: Adding all breakable tiles to possibleMoves array.", playerID);
        foreach (Tile cell in boardManager.GetBoardArray())
        {
            if (cell.isBreakable)
            {
                possibleMoves.Add(cell);
            }
        }
        Debug.LogFormat("{0}: All breakble tiles added.", playerID);

        if (possibleMoves.Count == 0)
        {
            Debug.LogFormat("{0}: Exiting out of DestroyTile(), no valid breakable tiles.", playerID);
            boardManager.ClearBoard();
            return false;
        }
            

        Tile selectedMove = GetClosestPosition(possibleMoves);
        if (!selectedMove)
            selectedMove = possibleMoves[Random.Range(0, possibleMoves.Count)];

        if (selectedMove.GetState("BLOCK"))
        {
            if (boardManager.SubtractMove(2))
            {
                selectedMove.SetState("EMPTY");
                soundManager.PlaySound("SELECT");
            }
            else
            {
                return false;
            }
        }

        if (invalidMoveCounter > MAX_INVALID_MOVES)
        {
            if (selectedMove.GetState("WALK"))
            {
                if (boardManager.SubtractMove(1))
                {
                    Debug.LogFormat("{0}: Destroyed walk tile. Set counter to 0", playerID);
                    selectedMove.SetState("EMPTY");
                    soundManager.PlaySound("SELECT");
                    invalidMoveCounter = 0;
                }
                else
                {
                    Debug.LogErrorFormat("{0}: Exiting out of DestroyTile(), unable to make this move.", playerID);
                    Debug.Break();
                    return false;
                }
            }
        }
        else
        {
            if (selectedMove.GetState("WALK"))
            {
                Debug.LogFormat("{0}: Exiting out of DestroyTile(), selected walk tile. No need to break.", playerID);
                return false;
            }
        }

        boardManager.ClearBoard();
        return true;
    }
}
