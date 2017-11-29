using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Need this for calling UI scripts

public class MenuControl : MonoBehaviour
{

	[SerializeField]
	Transform UIPanel;
	//Will assign our panel to this variable so we can enable/disable it

	[SerializeField]
	Text timeText;
	//Will assign our Time Text to this variable so we can modify the text it displays.

	bool isPaused;
	bool[] isStage;
	//Used to determine paused state

	void Start ()
	{
		UIPanel.gameObject.SetActive (false); //make sure our pause menu is disabled when scene starts
		isPaused = false; //make sure isPaused is always false when our scene opens
		isStage = new bool[1];
		for (int i = 0; i < isStage.Length; i++)
			isStage [0] = false;
	}

	void Update ()
	{

//		timeText.text = "Runtime: " + Time.timeSinceLevelLoad; //Tells us the time since the scene loaded

		//If player presses escape and game is not paused. Pause game. If game is paused and player presses escape, unpause.
		if (Input.GetKeyDown (KeyCode.Escape) && !isPaused)
			Pause ();
		else if (Input.GetKeyDown (KeyCode.Escape) && isPaused)
			UnPause ();
	}

	public void Pause ()
	{
//		GameObject[] all_Objs = SceneManager.GetActiveScene ().GetRootGameObjects ();
//		foreach (GameObject g in all_Objs) {
//			g.SetActive (false);
//		}
//		c.gameObject.SetActive (true);
//		Time.timeScale = 0.0f;
		isPaused = true;
		UIPanel.gameObject.SetActive (true); //turn on the pause menu
//		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("Intro"));
	}

	public void UnPause ()
	{
//		GameObject[] all_Objs = GameObject.FindObjectsOfType<GameObject> ();
//		foreach (GameObject g in all_Objs) {
//			g.SetActive (true);
//		}
		//		c.gameObject.SetActive (false);
//		Time.timeScale = 1.0f;
		isPaused = false;
		UIPanel.gameObject.SetActive (false); //turn off pause menu
		int x = -1;
		for (int i = 0; i < isStage.Length; i++) {
			if (isStage [i])
				x = i;
			isStage [i] = false;
		}
		switch (x) {
		case 0: 
			SceneManager.SetActiveScene (SceneManager.GetSceneByName ("Circuit"));
			break;
		}
	}

	public void Restart ()
	{
		SceneManager.LoadScene (1);
	}

	public void StageSelect ()
	{
//		isStage [0] = true;
//		SceneManager.LoadScene ("Circuit", LoadSceneMode.Additive);
//		UnPause ();
	}
}