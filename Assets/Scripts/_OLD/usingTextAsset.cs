using UnityEngine;
using System.Collections;

public class usingTextAsset : MonoBehaviour {

	public TextAsset asset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//print(asset.text);
	}

	void OnGUI ()
		
	{
		
		GUI.TextArea (new Rect (10,20,Screen.width/2,Screen.height/2),"Text" + asset);
		
	}
}
