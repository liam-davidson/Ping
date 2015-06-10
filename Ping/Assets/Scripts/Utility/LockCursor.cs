using UnityEngine;
using System.Collections;

public class LockCursor : MonoBehaviour {

	void Start() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	void OnMouseDown() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			//Cursor.lockState = CursorLockMode.None;
			//Cursor.visible = true;
		}
	}
}