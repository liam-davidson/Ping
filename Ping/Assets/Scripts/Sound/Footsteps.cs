using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {
	public AudioClip[] defaultFootstepSounds;
	public AudioClip[] defaultJumpSounds;

	public void Play() {
		GetComponent<AudioSource>().PlayOneShot (defaultFootstepSounds [Mathf.FloorToInt (Random.value * defaultFootstepSounds.Length)]);
	}

	public void PlayJump() {
		GetComponent<AudioSource>().PlayOneShot (defaultJumpSounds [Mathf.FloorToInt (Random.value * defaultJumpSounds.Length)]);
	}
}
