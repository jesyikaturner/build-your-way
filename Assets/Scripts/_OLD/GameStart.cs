using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour {

	/*public GameObject board;
	public BoardManager boardScript;

	public GameObject playerIcons;

	public GUIText title1;
	public GUIText title2;
	//public GUIText title3;

	public GUISkin skin;
	public bool gamePlay = false;
	
	public GUIText player1win;
	public GUIText player2win;

	// Use this for initialization
	void Start () {
		board = GameObject.Find("BoardManager");
		boardScript = board.GetComponent<BoardManager>();

		playerIcons = GameObject.Find("PlayerIcons");
		playerIcons.active = false;
		board.active = false;

	}
	
	// Update is called once per frame
	void Update () {
	}
	

	void OnGUI(){
		GUI.skin = skin;
		if(!gamePlay){
			title1.active = true;
			title2.active = true;
			//title3.active = true;
			//GUI.Label(new Rect(Screen.width/2-256,Screen.height-121.5f,256,111),title);


			if(GUI.Button(new Rect(Screen.width/2-110,Screen.height-100,200,50),"Start Game")){
				board.active = true;
				playerIcons.active = true;
				gamePlay = true;

			}
		}else{
			title1.active = false;
			title2.active = false;
			//title3.active = false;

		}

		if(boardScript.p1_winGame){
			boardScript.isPlaying = false;
			player1win.active = true;
			if(GUI.Button(new Rect(Screen.width/2-20,Screen.height/2-100,100,25),"Restart"))
				Application.LoadLevel(Application.loadedLevel);

		}
		if(boardScript.p2_winGame){
			boardScript.isPlaying = false;
			player2win.active = true;
			if(GUI.Button(new Rect(Screen.width/2-20,Screen.height/2-100,100,25),"Restart"))
				Application.LoadLevel(Application.loadedLevel);
		}


	}*/
}
