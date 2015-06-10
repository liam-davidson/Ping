using UnityEngine;
using System;
using System.Collections.Generic;

namespace Persistence {
	public delegate void OnPersistHandler(string key, object value);
}

public class PersistenceManager : SVBLM.Core.Singleton<PersistenceManager> {
	protected PersistenceManager() {}

	public event Persistence.OnPersistHandler onPersistEvent;

	public static bool ReadBool(string key) {
		return PlayerPrefs.GetInt(key, 0) == 1;
	}

	public static void Persist(string key, bool value) {
		PlayerPrefs.SetInt(key, value ? 1 : 0);
		Instance.NotifyChanged(key, value);
	}

	public static int ReadInt(string key) {
		return PlayerPrefs.GetInt(key, -1);
	}
	
	public static void Persist(string key, int value) {
		PlayerPrefs.SetInt(key, value);
		Instance.NotifyChanged(key, value);
	}

	public static string ReadString(string key) {
		return PlayerPrefs.GetString(key, "");
	}
	
	public static void Persist(string key, string value) {
		PlayerPrefs.SetString(key, value);
		Instance.NotifyChanged(key, value);
	}

	public static void RegisterOnPersistHandler(Persistence.OnPersistHandler handler) {
		Instance.onPersistEvent += handler;
	}

	public static void RemoveOnPersistHandler(Persistence.OnPersistHandler handler) {
		Instance.onPersistEvent -= handler;
	}

	public void NotifyChanged(string key, object value) {
		if(onPersistEvent != null) onPersistEvent (key, value);
	}
}
