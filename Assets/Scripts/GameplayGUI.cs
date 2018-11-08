using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayGUI : MonoBehaviour {

    public Canvas gameplayGUI;
    public Sprite playerOneIcon;
    public Sprite playerTwoIcon;

    private BoardManager boardManager;
    private Image playerIndicator;

	// Use this for initialization
	void Start () {
		
	}

    public void SetupGameplayGUI(BoardManager boardManager)
    {
        this.boardManager = boardManager;
        playerIndicator = gameplayGUI.transform.Find("PlayerIndicator").GetComponent<Image>();
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
	}
}
