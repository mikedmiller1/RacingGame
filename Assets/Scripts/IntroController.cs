using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
	// Use this for initialization
	void Start()
    {
		
	}
	

	// Update is called once per frame
	void Update()
    {
        // Check for a key press
        if( Input.anyKeyDown )
        {
            // Change to the track scene
            StartCoroutine( LoadTrackAsync() );
        }
	}


    /// <summary>
    /// Loads the track scene.
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadTrackAsync()
    {
        // Start loading the track scene
        AsyncOperation AsyncLoad = SceneManager.LoadSceneAsync( "Circuit" );

        // Wait until the scene is fully loaded before exiting
        while( !AsyncLoad.isDone )
        {
            yield return null;
        }
    }
}
