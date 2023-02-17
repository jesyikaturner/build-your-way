using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwapper : MonoBehaviour {

    // Canvas Fade Transition
    [SerializeField] private Canvas activeCanvas;
    [SerializeField] private Canvas prevCanvas;
    [SerializeField] private bool transitionActive = true;

    public string prevCanvasString;
    public string activeCanvasString = "DEBUG";


    public Canvas mainMenu;
    public Canvas debugMenu;
    public Canvas howToPlay;
    public Canvas options;
    public Canvas gameplay;
    public Canvas credits;
    public Canvas pause;
    public CameraDrift cam;

    private GameManager gameManager;
    private SoundManager soundManager;

    public void SetupCanvas(GameManager gameManager, SoundManager soundManager)
    {
        this.gameManager = gameManager;
        this.soundManager = soundManager;

        SetCanvas(activeCanvasString);
        activeCanvas = debugMenu;
        soundManager.PlayMusic(1);
    }

    public void SetCanvas(string menu)
    {
        prevCanvasString = activeCanvasString;
        activeCanvasString = menu;
        switch (menu)
        {
            case "MENU":
                cam.StartDrifting();
                prevCanvas = activeCanvas;
                activeCanvas = mainMenu;
                gameManager.isPlaying = false;
                break;
            case "DEBUG":
                cam.StartDrifting();
                prevCanvas = activeCanvas;
                activeCanvas = debugMenu;
                gameManager.isPlaying = false;
                break;
            case "HOWTOPLAY":
                prevCanvas = activeCanvas;
                activeCanvas = howToPlay;
                break;
            case "OPTIONS":
                prevCanvas = activeCanvas;
                activeCanvas = options;
                break;
            case "GAMEPLAY":
                cam.StopDrifting();
                prevCanvas = activeCanvas;
                activeCanvas = gameplay;
                gameManager.isPlaying = true;
                break;
            case "CREDITS":
                prevCanvas = activeCanvas;
                activeCanvas = credits;
                gameManager.isPlaying = false;
                break;
            case "PAUSE":
                prevCanvas = activeCanvas;
                activeCanvas = pause;
                gameManager.isPlaying = false;
                break;
            default:
                Debug.LogErrorFormat("{0} isn't a valid canvas", menu);
                break;
        }
        if(!transitionActive)
        {
            StartCoroutine(CanvasFadeTrasition(0.01f, 0.01f));
            transitionActive = true;
        }

    }
    public void Btn_Menu()
    {
        transitionActive = false;
        SetCanvas("MENU");
        soundManager.PlaySound("SELECT");
        gameManager.RemoveControllers();

    }

    public void Btn_HowToPlay()
    {
        transitionActive = false;
        SetCanvas("HOWTOPLAY");
        soundManager.PlaySound("SELECT");
    }

    public void Btn_Credits()
    {
        transitionActive = false;
        SetCanvas("CREDITS");
        soundManager.PlaySound("SELECT");
    }

    public void Btn_Pause()
    {
        transitionActive = false;
        SetCanvas("PAUSE");
        soundManager.PlaySound("SELECT");
    }

    public void Btn_Options()
    {
        transitionActive = false;
        SetCanvas("OPTIONS");
        soundManager.PlaySound("SELECT");
    }

    public void Btn_PVP_Gameplay()
    {
        transitionActive = false;
        gameManager.mode = GameManager.GameMode.PVP;
        gameManager.SetupControllers();
        SetCanvas("GAMEPLAY");
        soundManager.PlaySound("SELECT");
    }

    public void Btn_PVC_Gameplay()
    {
        transitionActive = false;
        gameManager.mode = GameManager.GameMode.PVC;
        gameManager.SetupControllers();
        SetCanvas("GAMEPLAY");
        soundManager.PlaySound("SELECT");
    }

    public void Btn_CvC_Gameplay()
    {
        transitionActive = false;
        gameManager.mode = GameManager.GameMode.CVC;
        gameManager.SetupControllers();
        SetCanvas("GAMEPLAY");
        soundManager.PlaySound("SELECT");
    }

    // Allows the restart button to play the sound effect
    public void Btn_Restart()
    {
        transitionActive = false;
        soundManager.PlaySound("SELECT");
    }

    public void Btn_Back()
    {
        transitionActive = false;
        soundManager.PlaySound("SELECT");
        SetCanvas(prevCanvasString);
    }

    private IEnumerator CanvasFadeTrasition(float outDelay, float inDelay)
    {
        CanvasGroup prevCanvasGroup = prevCanvas.GetComponent<CanvasGroup>();
        while (prevCanvasGroup.alpha > 0)
        {
            prevCanvasGroup.alpha-= 0.1f;
            yield return new WaitForSeconds(outDelay);
        }
        prevCanvas.gameObject.SetActive(false);

        CanvasGroup activeCanvasGroup = activeCanvas.GetComponent<CanvasGroup>();
        activeCanvasGroup.alpha = 0;
        activeCanvas.gameObject.SetActive(true);

        while (activeCanvasGroup.alpha < 1)
        {
            activeCanvasGroup.alpha += 0.1f;
            yield return new WaitForSeconds(inDelay);
        }

        transitionActive = false;
    }
}
