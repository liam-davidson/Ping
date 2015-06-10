using UnityEngine;
using System.Collections;
using System;

public class Game : SVBLM.Core.Singleton<Game> {
	public static new Camera camera {
		get {
			return Camera.main;
		}
		private set {}
	}

	void Start() {
		Application.targetFrameRate = 60;
	}


	#region UI
	public static bool UIEnabled = true;
	#endregion


	#region persistence
	// Convenience Methods
	public static void Persist(string key, int val) { PersistenceManager.Persist (key, val); }
	public static void Persist(string key, string val) { PersistenceManager.Persist (key, val); }
	public static void Persist(string key, bool val) { PersistenceManager.Persist (key, val); }
	public static int GetPersistedInt(string key) { return PersistenceManager.ReadInt (key); }
	public static bool GetPersistedBool(string key) { return PersistenceManager.ReadBool (key); }
	public static string GetPersistedString(string key) { return PersistenceManager.ReadString (key); }
	public static void RegisterOnPersistHandler(Persistence.OnPersistHandler handler) { PersistenceManager.RegisterOnPersistHandler (handler); }
	public static void RemoveOnPersistHandler(Persistence.OnPersistHandler handler) { PersistenceManager.RemoveOnPersistHandler (handler); }
	#endregion

	#region promode
	public static bool SupportsImageEffects() { return SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures; }
	#endregion

	public static PlayerControllerRedux Player { 
		get {
			return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerRedux>();
		}
		set {}
	}

	public static void PoofAway(GameObject gameObject) {
		GameObject ps = (GameObject) Instantiate (Resources.Load<GameObject> ("Poof"));
		ps.transform.position = gameObject.transform.position;
		Destroy (gameObject);
	}

	void OnApplicationQuit() {

	}
}
