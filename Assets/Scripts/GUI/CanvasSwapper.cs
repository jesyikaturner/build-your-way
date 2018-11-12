using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwapper : MonoBehaviour {

    public Canvas mainMenu;
    public Canvas howToPlay;
    public Canvas options;
    public Canvas gameplay;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
