using UnityEngine;
using System.Collections;

public class PersistenceDemoCube : MonoBehaviour {

	public bool isActive = false;
	public string listeningKey = "Persistence_Demo_1_Active";

	// Use this for initialization
	void Start () {
		Game.RegisterOnPersistHandler(OnGameStateChange);
		isActive = PersistenceManager.ReadBool(listeningKey);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P)) {
			PersistenceManager.Persist(listeningKey, !isActive);
		}

		if(isActive) { 
			GetComponent<Renderer>().material.color = Color.red;
		} else {
			GetComponent<Renderer>().material.color = Color.white;
		}
	}

	public void OnGameStateChange(string key, object value) {
		if(key == listeningKey) {
			isActive = PersistenceManager.ReadBool(listeningKey);
		}
	}
}
