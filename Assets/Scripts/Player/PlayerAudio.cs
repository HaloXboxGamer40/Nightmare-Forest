using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour {

    // Audio stuff
    public AudioClip[] movement;
    public AudioSource audioSource;

    public Player player;

    // Make sure that our audiosource can loop
    void Awake() {
        audioSource.loop = true;
    }

    // This is where the real movement comes into play
    void FixedUpdate() {

        if (!player.IsWalking && !player.IsRunning && player.isGrounded) {
            audioSource.Stop();
            return;
        }
        // Play the music
        int clipIndex = player.IsRunning ? 1 : 0;
        //clipIndex = Mathf.Clamp(clipIndex, 0, movement.Length - 1);
        AudioClip desired = movement[clipIndex];

        // If we dont already have our audio selected then we set that here
        if (audioSource.clip != desired) {
            audioSource.clip = desired;
            audioSource.loop = true;
            audioSource.Play();
        } else if (!audioSource.isPlaying) { // else if we aren't even playing it then we set that here!
            audioSource.Play();
        }
    }
}