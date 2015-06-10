using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Soundtrack : Softleton<Soundtrack> {
	public AudioClip[] Songs;

	public AudioSource soundtrackSource;
	public AudioSource oneShotSource;

	private int _currentTrack;

	private float _scaling = 1.0f;
	private float fadeVolume = 1.0f;
	public float scaling = 1.0f;


	public int currentTrack {
		get { return _currentTrack; }
		set {
			// don't go out of bounds.
			_currentTrack = value % Songs.Length;
		}
	}

	void Start() {
		if(soundtrackSource == null && GetComponent<AudioSource>() != null) {
			soundtrackSource = GetComponent<AudioSource>();
		}

		if(oneShotSource == null) {
			oneShotSource = gameObject.AddComponent<AudioSource>();
		}

		soundtrackSource.spatialBlend = 0f;
		oneShotSource.spatialBlend = 0f;

		soundtrackSource.loop = true;
		if(Songs != null && Songs.Length > 0) {
			soundtrackSource.clip = Songs[_currentTrack];
			soundtrackSource.Play();
		}
	}

	void Update() {
		_scaling = Mathf.Lerp(_scaling, scaling, 0.1f);
		soundtrackSource.volume = fadeVolume * _scaling;
	}

	public IEnumerator FadeTo(int trackNumber) {
		currentTrack = trackNumber;

		while(fadeVolume > 0.02f) {
			fadeVolume -= 0.3f * Time.deltaTime;
			yield return null;
		}

		soundtrackSource.clip = Songs[_currentTrack];

		fadeVolume = 1.0f;
		soundtrackSource.Play();

	}

	public IEnumerator FadeTo(AudioClip clip) {
		
		while(fadeVolume > 0.02f) {
			fadeVolume -= 0.3f * Time.deltaTime;
			yield return null;
		}
		
		soundtrackSource.clip = clip;
		
		fadeVolume = 1.0f;
		soundtrackSource.Play();
		
	}

	public IEnumerator StopPlaying() {
		while(fadeVolume > 0.01f) {
			fadeVolume -= 0.7f * Time.deltaTime;
			yield return null;
		}

		soundtrackSource.Stop();
	}

	public static void Reset() {
		Instance.scaling = 1.0f;
		Instance.StartCoroutine(Instance.FadeTo(Instance.currentTrack));
	}

	public static void Play() {
		Instance.StartCoroutine(Instance.FadeTo(Instance.currentTrack));
	}

	public static void Play(int trackNumber) {
		Instance.StartCoroutine(Instance.FadeTo(trackNumber));
	}

	public static void Stop() {
		Instance.StartCoroutine(Instance.StopPlaying());
	}

	public static void Next() {
		Instance.StartCoroutine(Instance.FadeTo(Instance.currentTrack + 1));
	}

	public static void Prev() {
		Instance.StartCoroutine(Instance.FadeTo(Instance.currentTrack - 1));
	}

	public static void Quiet() {
		Instance.scaling = 0.25f;
	}

	public static void VeryQuiet() {
		Instance.scaling = 0.1f;
	}

	public static void Loud() {
		Instance.scaling = 1.0f;
	}

	public static void PlayOneShot(AudioClip clip) {
		if(clip != null)
			Instance.oneShotSource.PlayOneShot(clip);
	}

	public static void PlaySpecific(AudioClip clip) {
		if(clip != null) 
			Instance.StartCoroutine(Instance.FadeTo(clip));
	}

}
