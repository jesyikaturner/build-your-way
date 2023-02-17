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
    public GameObject pausedImage;
    public GameObject restartButton;
    public GameObject menuButton;

    private List<IController> controllers;

    public void SetupGameplayGUI(GameManager gameManager, BoardManager boardManager, ref List<IController> controllers)
    {
        this.boardManager = boardManager;
        this.gameManager = gameManager;
        this.controllers = controllers;

        //playerIndicator = gameplayGUI.transform.Find("INDICATOR").GetComponent<Image>();
        //winnerText = gameplayGUI.transform.Find("WINNER").GetComponent<Text>();

        winnerText.gameObject.SetActive(false);
        restartButton.SetActive(false);
        menuButton.SetActive(false);
        pausedImage.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        SetActivePlayerIndicator();

        // if (gameManager.PlayerOneWins)
        // {
        //     winnerText.text = "PLAYER 1 WINS";
        //     winnerText.gameObject.SetActive(true);
        //     restartButton.SetActive(true);
        //     menuButton.SetActive(true);
        // }

        // if (gameManager.PlayerTwoWins)
        // {
        //     winnerText.text = "PLAYER 2 WINS";
        //     winnerText.gameObject.SetActive(true);
        //     restartButton.SetActive(true);
        //     menuButton.SetActive(true);
        // }

        // if (Input.GetKeyUp(KeyCode.P) && gameManager.isPlaying)
        // {
        //     HandlePause();
        // }
    }

    private void SetActivePlayerIndicator()
    {
        // return if the controllers haven't been set yet
        if (controllers == null || controllers.Count < 1)
            return;

        int activeController = 0;

        for (int i = 0; i < controllers.Count; i++)
        {
            ComputerPlayer currCompController = null;
            PlayerControls currPlayerController = null;

            // figure out what component the current controller is and set the activeactive controller to the corresponding index (+1 because of 0 indexing)
            if ( GetComponents<ComputerPlayer>().Length > 1)
                currCompController = controllers[i].GetType() == typeof(ComputerPlayer) ? GetComponents<ComputerPlayer>()[i] : null;
            else
                currCompController = controllers[i].GetType() == typeof(ComputerPlayer) ? GetComponent<ComputerPlayer>() : null;
            if ( GetComponents<PlayerControls>().Length > 1)
                currPlayerController = controllers[i].GetType() == typeof(PlayerControls) ? GetComponents<PlayerControls>()[i] : null;
            else
                currPlayerController = controllers[i].GetType() == typeof(PlayerControls) ? GetComponent<PlayerControls>() : null;

            if ((currCompController && currCompController.isActive) || (currPlayerController && currPlayerController.isActive))
                activeController = i+1;
        }
        
        // change the indicator based on the active controller
        switch(activeController)
        {
            case 1:
                playerIndicator.sprite = playerOneIcon;
                break;
            case 2:
                playerIndicator.sprite = playerTwoIcon;
                break;
            default:
                Debug.LogErrorFormat("{0} is returning a bad value.", activeController);
                break;
        }
    }

    // public void HandlePause()
    // {
    //     boardManager.IsPaused(!boardManager.IsPaused());
    //     if (boardManager.IsPaused())
    //     {
    //         restartButton.SetActive(true);
    //         menuButton.SetActive(true);
    //         pausedImage.SetActive(true);
    //         //StopComputerPlayer();
    //     }
    //     else
    //     {
    //         restartButton.SetActive(false);
    //         menuButton.SetActive(false);
    //         pausedImage.SetActive(false);
    //         //StartComputerPlayer();
    //     }
    // }

    public void Btn_Restart()
    {
        boardManager.RestartBoard();
        // HandlePause();
        winnerText.gameObject.SetActive(false);
    }


}
