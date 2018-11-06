using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public BoardManager boardManager;
    public bool player_v_computer = false;

    private List<PlayerControls> players;
    private List<ComputerPlayer> computers;

    public bool PlayerOneWins = false;
    public bool PlayerTwoWins = false;


	// Use this for initialization
	void Start () {
        players = new List<PlayerControls>();
        computers = new List<ComputerPlayer>();

        if (player_v_computer)
        {
            players.Add(gameObject.AddComponent<PlayerControls>());
            players[0].SetupPlayerControls(boardManager, 1);
            computers.Add(gameObject.AddComponent<ComputerPlayer>());
            computers[0].SetupComputerControls(boardManager, 2);
        }
        else
        {
            players.Add(gameObject.AddComponent<PlayerControls>());
            players[0].SetupPlayerControls(boardManager, 1);
            players.Add(gameObject.AddComponent<PlayerControls>());
            players[1].SetupPlayerControls(boardManager, 2);
        }
	}
	
	// Update is called once per frame
	void Update () {
        boardManager.SwitchPlayer();
        //boardManager.FillHands();

        //PlayerOneWins = boardManager.CheckForWinner(1);
        //PlayerTwoWins = boardManager.CheckForWinner(2);
    }
}
