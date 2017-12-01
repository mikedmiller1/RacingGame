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

	void Start ()
	{
		UIPanel.gameObject.SetActive (false); //make sure our pause menu is disabled when scene starts
		isPaused = false; //make sure isPaused is always false when our scene opens
	}

	void Update ()
	{
		//If player presses escape and game is not paused. Pause game. If game is paused and player presses escape, unpause.
		if (Input.GetKeyDown (KeyCode.Escape) && !isPaused)
		{
			Pause ();
		}
		else if (Input.GetKeyDown (KeyCode.Escape) && isPaused)
		{
			Resume ();
		}
	}

	public void Pause ()
	{
		Time.timeScale = 0;
		isPaused = true;
		UIPanel.gameObject.SetActive (true); //turn on the pause menu
	}

	public void Resume ()
	{
		Time.timeScale = 1;
		isPaused = false;
		UIPanel.gameObject.SetActive (false); //turn off pause menu
	}

	public void Restart ()
	{
		Resume ();
		SceneManager.LoadScene ("IntroScreen");
	}
}