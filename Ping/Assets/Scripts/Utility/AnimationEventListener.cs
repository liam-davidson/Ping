using UnityEngine;
using System.Collections;
using System;

public class AnimationEventListener : MonoBehaviour {
	public Action action;
	public void SetCallback(Action action) {
		this.action = action;
	}

	public void OnEvent() {
		action.Invoke ();
	}
}
