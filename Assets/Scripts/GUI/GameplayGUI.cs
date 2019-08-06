using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayGUI : MonoBehaviour {

    public Canvas gameplayGUI;
    public Sprite playerOneIcon;
    public Sprite playerTwoIcon;

    private BoardManager boardManager;
    private GameManager gameManager;

    public Image playerIndicator;
    public Text winnerText;
    public GameObject restartButton;
    public GameObject menuButton;

	// Use this for initialization
	void Start () {
		
	}

    public void SetupGameplayGUI(GameManager gameManager, BoardManager boardManager)
    {
        this.boardManager = boardManager;
        this.gameManager = gameManager;

        //playerIndicator = gameplayGUI.transform.Find("INDICATOR").GetComponent<Image>();
        //winnerText = gameplayGUI.transform.Find("WINNER").GetComponent<Text>();

        winnerText.gameObject.SetActive(false);
        restartButton.SetActive(false);
        menuButton.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        switch (boardManager.GetCurrPlayer())
        {
            case 1:
                playerIndicator.sprite = playerOneIcon;
                break;
            case 2:
                playerIndicator.sprite = playerTwoIcon;
                break;

            default:
                Debug.LogError("Current Player is returning a bad value.");
                break;
        }


        if (gameManager.PlayerOneWins)
        {
            winnerText.text = "PLAYER 1 WINS";
            winnerText.gameObject.SetActive(true);
            restartButton.SetActive(true);
            menuButton.SetActive(true);
        }

        if (gameManager.PlayerTwoWins)
        {
            winnerText.text = "PLAYER 2 WINS";
            winnerText.gameObject.SetActive(true);
            restartButton.SetActive(true);
            menuButton.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.P) && gameManager.isPlaying)
        {
            HandlePause();
        }
    }

    public void HandlePause()
    {
        boardManager.IsPaused(!boardManager.IsPaused());
        StartComputerPlayer();

        if (boardManager.IsPaused())
        {
            restartButton.SetActive(true);
            menuButton.SetActive(true);
        }
        else
        {
            restartButton.SetActive(false);
            menuButton.SetActive(false);
        }
    }

    private void StartComputerPlayer()
    {
        List<IPlayer> players = gameManager.GetPlayers();

        foreach (IPlayer player in players)
        {
            if (player.GetType() == typeof(ComputerPlayer))
            {
                ComputerPlayer cp = (ComputerPlayer)player;
                cp.StartComputerLogic();
            }
        }
    }

    public void Btn_Restart()
    {
        boardManager.RestartBoard();
        HandlePause();
        winnerText.gameObject.SetActive(false);
    }


}
