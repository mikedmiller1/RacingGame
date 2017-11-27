using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    #region Sound Controls
    /// <summary>
    /// The player object containing the audio source.
    /// </summary>
    public GameObject PlayerObject;


    /// <summary>
    /// The source through which to play the sound.
    /// </summary>
    private AudioSource source;


    /// <summary>
    /// The sound currently being played.
    /// </summary>
    private Sound CurrentSound = Sound.None;


    /// <summary>
    /// Enums of the available sounds.
    /// </summary>
    private enum Sound
    {
        None,
        Idling,
        Accelerating,
        Coasting,
        Braking
    }

    #endregion



    #region Sound Prefabs

    /// <summary>
    /// The initial sound played when the scene starts.
    /// </summary>
    public AudioClip StartUp;


    /// <summary>
    /// Played when the player is stopped and not moving.
    /// </summary>
    public AudioClip Idling;


    /// <summary>
    /// Played when the player is accelerating.
    /// </summary>
    public AudioClip Accelerating;


    /// <summary>
    /// Played when the player is coasting, ie. not accelerating or braking.
    /// </summary>
    public AudioClip Coasting;


    /// <summary>
    /// Played when the player is braking.
    /// </summary>
    public AudioClip Braking;

    #endregion



    #region Unity Methods

    /// <summary>
    /// Called when the object is created.
    /// Awake() is called before Start().
    /// </summary>
    void Awake()
    {
        // Get the audio source for this object
        source = PlayerObject.GetComponent<AudioSource>();
	}


    /// <summary>
    /// Called when the object is initialized
    /// </summary>
    private void Start()
    {
        // Play the start-up sound
        source.PlayOneShot( StartUp );
    }


    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
		// Check the state of the player (ie. accelerating, braking, etc.)

        // If this is different than the sound currently being played
            
            // Stop the current sound

            // Play the corresponding sound
	}

    #endregion
}
