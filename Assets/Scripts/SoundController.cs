using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    #region Sound Controls

    /// <summary>
    /// The game objet which contains the start-up audio source.
    /// </summary>
    public GameObject StartUpAudio;

    /// <summary>
    /// The game object which contains the engine audio source.
    /// </summary>
    public GameObject EngineAudio;


    /// <summary>
    /// The game object which contains the crash audio source.
    /// </summary>
    public GameObject CrashAudio;


    /// <summary>
    /// The game object which contains the cornering audio source.
    /// </summary>
    public GameObject CorneringAudio;



    /// <summary>
    /// The source through which to play the start-up sound.
    /// </summary>
    private AudioSource StartUpAudioSource;


    /// <summary>
    /// The source through which to play the engine and braking sounds.
    /// </summary>
    private AudioSource EngineAudioSource;


    /// <summary>
    /// The source through which to play the crash sounds.
    /// </summary>
    private AudioSource CrashAudioSource;


    /// <summary>
    /// The source through which to play the cornering sounds.
    /// </summary>
    private AudioSource CorneringAudioSource;



    /// <summary>
    /// The engine sound currently being played.
    /// </summary>
    private EngineSound CurrentEngineSound = EngineSound.None;


    /// <summary>
    /// Enums of the available sounds.
    /// </summary>
    private enum EngineSound
    {
        None,
        Idling,
        Accelerating,
        Coasting,
        Braking,
    }

    #endregion



    #region Sound Prefabs

    /// <summary>
    /// The initial sound played when the scene starts.
    /// </summary>
    public AudioClip[] StartUp;


    /// <summary>
    /// Played when the player is stopped and not moving.
    /// </summary>
    public AudioClip[] Idling;


    /// <summary>
    /// Played when the player is accelerating.
    /// </summary>
    public AudioClip[] Accelerating;


    /// <summary>
    /// Played when the player is coasting, ie. not accelerating or braking.
    /// </summary>
    public AudioClip[] Coasting;


    /// <summary>
    /// Played when the player is braking.
    /// </summary>
    public AudioClip[] Braking;


    /// <summary>
    /// Played when the player is cornering.
    /// </summary>
    public AudioClip[] Cornering;


    /// <summary>
    /// Played when the player crashes into a barrier.
    /// </summary>
    public AudioClip[] Crashing;

    #endregion



    #region Unity Methods

    /// <summary>
    /// Called when the object is created.
    /// Awake() is called before Start().
    /// </summary>
    void Awake()
    {
        // Get the audio sources
        StartUpAudioSource = StartUpAudio.GetComponent<AudioSource>();
        EngineAudioSource = EngineAudio.GetComponent<AudioSource>();
        CrashAudioSource = CrashAudio.GetComponent<AudioSource>();
        CorneringAudioSource = CorneringAudio.GetComponent<AudioSource>();
	}

    #endregion



    #region Public Methods

    /// <summary>
    /// Plays a start-up sound.
    /// </summary>
    public void PlayStartUpSound()
    {
        if( !StartUpAudioSource.isPlaying )
        {
            StartUpAudioSource.PlayOneShot( GetRandomSound( StartUp ) );
        }
    }



    /// <summary>
    /// Plays a cornering sound.
    /// </summary>
    public void PlayCorneringSound()
    {
        if( !CorneringAudioSource.isPlaying )
        {
            CorneringAudioSource.PlayOneShot( GetRandomSound( Cornering ) );
        }
    }



    /// <summary>
    /// Stops playing the cornering sound.
    /// </summary>
    public void StopCorneringSound()
    {
        CorneringAudioSource.Stop();
    }



    /// <summary>
    /// Plays a crashing sound.
    /// </summary>
    public void PlayCrashingSound()
    {
        if( !CrashAudioSource.isPlaying )
        {
            CrashAudioSource.PlayOneShot( GetRandomSound( Crashing ) );
        }
    }


    /// <summary>
    /// Plays an accelerating sound.
    /// </summary>
    public void PlayAccelerationSound()
    {
        // If the current engine sound is not accelerating, or the engine audio is not already playing
        if( CurrentEngineSound != EngineSound.Accelerating ||
            !EngineAudioSource.isPlaying )
        {
            EngineAudioSource.Stop();
            EngineAudioSource.PlayOneShot( GetRandomSound( Accelerating ) );
            CurrentEngineSound = EngineSound.Accelerating;
        }
    }


    /// <summary>
    /// Plays a braking sound.
    /// </summary>
    public void PlayBrakingSound()
    {
        // If the current engine sound is not braking, or the engine audio is not already playing
        if( CurrentEngineSound != EngineSound.Braking ||
            !EngineAudioSource.isPlaying )
        {
            EngineAudioSource.Stop();
            EngineAudioSource.PlayOneShot( GetRandomSound( Braking ) );
            CurrentEngineSound = EngineSound.Braking;
        }
    }


    /// <summary>
    /// Plays a coasting sound.
    /// </summary>
    public void PlayCoastingSound()
    {
        // If the current engine sound is not coasting, or the engine audio is not already playing
        if( CurrentEngineSound != EngineSound.Coasting ||
            !EngineAudioSource.isPlaying )
        {
            EngineAudioSource.Stop();
            EngineAudioSource.PlayOneShot( GetRandomSound( Coasting ) );
            CurrentEngineSound = EngineSound.Coasting;
        }
    }


    /// <summary>
    /// Plays an idling sound.
    /// </summary>
    public void PlayIdlingSound()
    {
        // If the current engine sound is not idling, or the engine audio is not already playing
        if( CurrentEngineSound != EngineSound.Idling ||
            !EngineAudioSource.isPlaying )
        {
            EngineAudioSource.Stop();
            EngineAudioSource.PlayOneShot( GetRandomSound( Idling ) );
            CurrentEngineSound = EngineSound.Idling;
        }
    }

    #endregion



    #region Private Methods

    /// <summary>
    /// Selects a random sound from an array of sounds.
    /// </summary>
    /// <param name="Clips">The array of sounds to choose from.</param>
    /// <returns>A randomly chosen sound.</returns>
    AudioClip GetRandomSound( AudioClip[] Clips )
    {
        // Get a random number for the sound index
        int ClipNum = Random.Range( 0, Clips.Length );

        // Return a random sound from the array
        return Clips[ ClipNum ];
    }

    #endregion
}
