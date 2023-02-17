using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    // Constants
    private const int MAX_MOVES = 2;

    // GameMode Modes
    public enum GameMode
    {
        PVP, PVC, CVC
    }
    public GameMode mode;

    // Managers
    public BoardManager boardManager;
    public SoundManager soundManager;

    private List<IController> players;

    public bool PlayerOneWins = false;
    public bool PlayerTwoWins = false;
    public bool isPlaying = false;

    public int currMovesLeft;


	// Use this for initialization
	void Start () {
        gameObject.GetComponent<CanvasSwapper>().SetupCanvas(this, soundManager);
        currMovesLeft = boardManager.CurrMovesLeft = MAX_MOVES;
    }

    // Update is called once per frame
    void Update()
    {
        currMovesLeft = boardManager.CurrMovesLeft;
        SwitchPlayer();

        // PlayerOneWins = CheckForWinner(1);
        // PlayerTwoWins = CheckForWinner(2);
    }

#region Player Controllers Creation and Deletion
    public void SetupControllers()
    {
        players = new List<IController>();

        switch (mode)
        {
            case GameMode.PVP:
                players.Add(gameObject.AddComponent<PlayerControls>());
                players[0].SetupControls(soundManager, boardManager, 1);
                players[0].ToggleActive();
                players.Add(gameObject.AddComponent<PlayerControls>());
                players[1].SetupControls(soundManager, boardManager, 2);
                break;

            case GameMode.PVC:
                players.Add(gameObject.AddComponent<PlayerControls>());
                players[0].SetupControls(soundManager, boardManager, 1);
                players[0].ToggleActive();
                players.Add(gameObject.AddComponent<ComputerPlayer>());
                players[1].SetupControls(soundManager, boardManager, 2);
                break;

            case GameMode.CVC:
                players.Add(gameObject.AddComponent<ComputerPlayer>());
                players[0].SetupControls(soundManager, boardManager, 1);
                players[0].ToggleActive();
                players.Add(gameObject.AddComponent<ComputerPlayer>());
                players[1].SetupControls(soundManager, boardManager, 2);
                break;

            default:
                Debug.LogError("gamemode is invalid");
                break;
        }
        
        gameObject.GetComponent<GameplayGUI>().SetupGameplayGUI(this, boardManager, ref players);
    }

    public void RemoveControllers()
    {
        if (players == null || players.Count < 1) // if player doesnt exist, return out
            return;
        foreach(IController player in players)
        {
            if (player.GetType() == typeof(ComputerPlayer))
                Destroy(gameObject.GetComponent<ComputerPlayer>());
            if (player.GetType() == typeof(PlayerControls))
                Destroy(gameObject.GetComponent<PlayerControls>());
        }
    }
#endregion

    // public bool CheckForWinner(int playerID)
    // {
    //     int counter = 0;
    //     foreach (Tile cell in boardManager.BoardArray)
    //     {
    //         if (cell.team != playerID && cell.team != 0)
    //         {
    //             if (cell.GetAttacker() && cell.GetAttacker().Team == playerID)
    //                 counter++;
    //         }
    //     }
    //     if (counter > 2)
    //     {
    //         boardManager.IsPaused(true);
    //         return true;
    //     }
    //     //boardManager.IsPaused(false);
    //     return false;
    // }

    private void SwitchPlayer()
    {
        // Swap player when the moves get below 1. Then reset the moves to max.
        if(currMovesLeft < 1)
		{
            players[0].ToggleActive();
            players[1].ToggleActive();
			boardManager.CurrMovesLeft = MAX_MOVES;
		}
    }
}
