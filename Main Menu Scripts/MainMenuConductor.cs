using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuConductor : MonoBehaviour
{
    // Song beats per minute
    // This is determined by the song you're trying to sync up to
    public float songBpm;

    // The number of seconds for each song beat
    public float secPerBeat;

    // Current song position, in seconds
    public float songPosition;

    // Current song position, in beats
    public float songPositionInBeats;

    // How many seconds have passed since the song started
    public float dspSongTime;

    // AudioSource attached to this GameObject that will play the music.
    [Tooltip("Insert Audio Source here")][SerializeField] AudioSource musicSource = null;

    // Public fields used for analysing if a beat was hit
    public bool actionMade = false;
    public bool onBeat;
    public bool mistakeMade;
    public bool shotFired;

    // Start is called before the first frame update
    void Start()
    {
        // Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>();

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        // Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        // Determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;
    }
}
