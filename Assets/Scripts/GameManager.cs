using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum GameMode
    {
        PVP, PVC, CVC
    }
    public GameMode mode;

    public BoardManager boardManager;

    private List<IPlayer> players;

    private GameplayGUI gameplayGUI;

    public bool PlayerOneWins = false;
    public bool PlayerTwoWins = false;


	// Use this for initialization
	void Start () {
        players = new List<IPlayer>();

        switch (mode)
        {
            case GameMode.PVP:
                players.Add(gameObject.AddComponent<PlayerControls>());
                players[0].SetupPlayerControls(boardManager, 1);
                players.Add(gameObject.AddComponent<PlayerControls>());
                players[1].SetupPlayerControls(boardManager, 2);
                break;

            case GameMode.PVC:
                players.Add(gameObject.AddComponent<PlayerControls>());
                players[0].SetupPlayerControls(boardManager, 1);
                players.Add(gameObject.AddComponent<ComputerPlayer>());
                players[1].SetupPlayerControls(boardManager, 2);
                break;

            case GameMode.CVC:
                players.Add(gameObject.AddComponent<ComputerPlayer>());
                players[0].SetupPlayerControls(boardManager, 1);
                players.Add(gameObject.AddComponent<ComputerPlayer>());
                players[1].SetupPlayerControls(boardManager, 2);
                break;

            default:
                Debug.LogError("Game mode is invalid!");
                break;
        }

        gameplayGUI = gameObject.GetComponent<GameplayGUI>();
        gameplayGUI.SetupGameplayGUI(boardManager);
    }
	
	// Update is called once per frame
	void Update () {
        boardManager.SwitchPlayer();

        PlayerOneWins = boardManager.CheckForWinner(1);
        PlayerTwoWins = boardManager.CheckForWinner(2);
    }
}
