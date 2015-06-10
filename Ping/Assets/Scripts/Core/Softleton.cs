using UnityEngine;
using System.Collections;

public abstract class Softleton<T> : MonoBehaviour where T : Softleton<T> {
	private static T _instance;

	public static T Instance {
		get {
			return (T) _instance;
		}

		set {
			if(_instance == null) {
				_instance = value;
			} else {
				UI.ToastError("Tried to instantiate " + typeof(T).Name + " when the instance was not null, skipping...");
			}
		}
	}

	void Awake() {
		if(_instance != null) {
			UI.ToastError ("WARNING: Multiple softletons detected in scene. (" + typeof(T).FullName + ").");
			UI.ToastError ("Using only the first instance to compensate. Clean up your scene!");
			DestroyImmediate(this);
			return;
		}

		_instance = (T) this;
	}
}
