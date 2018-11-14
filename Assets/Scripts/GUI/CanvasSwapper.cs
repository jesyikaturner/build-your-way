using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwapper : MonoBehaviour {

    public Canvas mainMenu;
    public Canvas howToPlay;
    public Canvas options;
    public Canvas gameplay;
    private GameManager gameManager;

    public void SetupCanvas(GameManager gameManager)
    {
        this.gameManager = gameManager;
        SetCanvas("MENU");
    }

    public void SetCanvas(string menu)
    {
        mainMenu.gameObject.SetActive(false);
        howToPlay.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        gameplay.gameObject.SetActive(false);

        switch (menu)
        {
            case "MENU":
                mainMenu.gameObject.SetActive(true);
                break;
            case "HOWTOPLAY":
                howToPlay.gameObject.SetActive(true);
                break;
            case "OPTIONS":
                options.gameObject.SetActive(true);
                break;
            case "GAMEPLAY":
                gameplay.gameObject.SetActive(true);
                break;
            default:
                Debug.LogErrorFormat("{0} isn't a valid canvas", menu);
                break;
        }
    }
    public void Btn_Menu()
    {
        SetCanvas("MENU");
    }

    public void Btn_HowToPlay()
    {
        SetCanvas("HOWTOPLAY");
    }

    public void Btn_Options()
    {
        SetCanvas("OPTIONS");
    }

    public void Btn_PVP_Gameplay()
    {
        gameManager.mode = GameManager.GameMode.PVP;
        gameManager.SetupControllers();
        SetCanvas("GAMEPLAY");
    }

    public void Btn_PVC_Gameplay()
    {
        gameManager.mode = GameManager.GameMode.PVC;
        gameManager.SetupControllers();
        SetCanvas("GAMEPLAY");
    }
}
