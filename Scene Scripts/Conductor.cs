using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    // Song beats per minute
    // This is determined by the song you're trying to sync up to
    public float songBpm;

    // The number of seconds for each song beat
    public float secPerBeat;

    // Initial beat offset used to get align music
    public float initialBeatOffset;

    // Current song position, in seconds
    public float songPosition;

    // Current song position, in beats
    public float songPositionInBeats;

    // How many seconds have passed since the song started
    public float dspSongTime;

    // AudioSource attached to this GameObject that will play the music.
    [Tooltip("Insert Audio Source here")][SerializeField] AudioSource musicSource = null;

    // The GameObject (PlayerCharacter) that is movable on beat
    [Tooltip("Insert Player Character here")] [SerializeField] GameObject playerCharacter = null;

    [Tooltip("The % of a beat allowed either side of the beat " +
        "hitting for a player to hit their action")][SerializeField] float beatAllowance = 0.2f;

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
        songPositionInBeats = (songPosition - initialBeatOffset*secPerBeat) / secPerBeat;

        // Then process the inputs during the beat
        ProcessBeatInputs();
    }

    private void ProcessBeatInputs()
    {
        // Only allow actions if within the beat allowance
        if (songPositionInBeats % 1 < beatAllowance || songPositionInBeats % 1 > 1 - beatAllowance)
        {
            OnBeatProcessing();
        }

        // Otherwise, we check for inputs to see if a mistake was made off beat
        else
        {
            OffBeatProcessing();

        }
    }

    private void OnBeatProcessing()
    {
        onBeat = true;

        if (!actionMade && !mistakeMade)
        {
            // Allow player to Process movement.
            // actionMade becomes true once a movement is processed,
            // and this condition is no longer called
            actionMade = playerCharacter.GetComponent<PlayerController>().ProcessMovement();
        }

        // Now for the shooting & reloading
        if (!actionMade && !mistakeMade)
        {
            // If a bullet's loaded we check for shooting. If not, we check for reloading
            if (playerCharacter.GetComponent<PlayerController>().bulletLoaded)
            {
                // If a movement wasn't made, see if the player has tried to shoot instead
                actionMade = playerCharacter.GetComponent<PlayerController>().ProcessShooting();
                shotFired = actionMade;
            }

            if (!playerCharacter.GetComponent<PlayerController>().bulletLoaded)
            {
                // If a movement wasn't made, see if the player has tried to shoot instead
                actionMade = playerCharacter.GetComponent<PlayerController>().ProcessReloading();
                shotFired = actionMade;
            }

        }
    }

    private void OffBeatProcessing()
    {
        // onBeat condition called before it is changed back to false,
        // so all the resets can be done in this condition
        if (onBeat)
        {
            mistakeMade = false;
            actionMade = false;
            onBeat = false;
            shotFired = false;
        }

        // Outside the beat, we check if there's any input, and don't allow
        // the player to move on the next beat if so. Note we're only using the
        // input functions here - we don't actually want an action to happen off
        // beat, only see if one was attempted

        // Only check if a mistake hasn't already been made
        if (!mistakeMade)
        {
            mistakeMade = playerCharacter.GetComponent<PlayerController>().ProcessMovementInput();

            // If no mistake made for movement, check if mistake made with shooting

            if (!mistakeMade)
            {
                mistakeMade = playerCharacter.GetComponent<PlayerController>().ProcessShootingInput();

                if (!mistakeMade)
                {
                    // And finally if not, check for reloading input

                    mistakeMade = playerCharacter.GetComponent<PlayerController>().ProcessReloadingInput();
                }
            }
        }
    }
}
