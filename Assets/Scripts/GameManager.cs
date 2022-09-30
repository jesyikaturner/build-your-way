using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    public enum GameMode
    {
        PVP, PVC, CVC
    }
    public GameMode mode;

    public BoardManager boardManager;
    public SoundManager soundManager;
    public DeckManager deckManager;

    private List<IController> players;

    public bool PlayerOneWins = false;
    public bool PlayerTwoWins = false;
    public bool isPlaying = false;

    [SerializeField] private int curr_moves = 2;
	private int curr_player = 1;
    private const int max_moves = 2; // Total moves each player can do


	// Use this for initialization
	void Start () {
        gameObject.GetComponent<GameplayGUI>().SetupGameplayGUI(this, boardManager);
        gameObject.GetComponent<CanvasSwapper>().SetupCanvas(this, soundManager);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(players != null)
        {
            boardManager.currentPlayer = players[boardManager.SwitchPlayer() - 1];
            if (boardManager.currentPlayer.GetType() == typeof(ComputerPlayer))
            {
                ComputerPlayer cp = (ComputerPlayer)boardManager.currentPlayer;
                cp.StartComputerLogic();
            }
        }*/

        SwitchPlayer();

        // PlayerOneWins = CheckForWinner(1);
        // PlayerTwoWins = CheckForWinner(2);
    }

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

    public List<IController> GetPlayers()
    {
        return players;
    }

    // public bool CheckForWinner(int playerID)
    // {
    //     int counter = 0;
    //     foreach (Tile cell in boardManager.GetBoardArray())
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
        if(curr_moves < 1)
		{
            players[0].ToggleActive();
            players[1].ToggleActive();
			curr_moves = max_moves;
		}
    }
}
