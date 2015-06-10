using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI : SVBLM.Core.Singleton<UI> {
	private GUISkin skin;
	public List<Toast> displayed = new List<Toast>();
	public Queue<Toast> queue = new Queue<Toast>();
	public Queue<Toast> postHumousQueue = new Queue<Toast>();

	public Dictionary<GameObject, Color> highlightedObjects = new Dictionary<GameObject, Color>();

	void Start() {
		skin = (GUISkin) Resources.Load ("HUDSkin", typeof(GUISkin));
	}

	void OnGUI() {
		GUI.skin = skin;
		foreach (Toast toast in displayed) {
			toast.Draw(skin.label);
		}
	}

	void MakeToast(Toast toast) {
		toast.onToastDeadEvent += OnToastDead;
		queue.Enqueue (toast);
	}

	void Update() {
		while (postHumousQueue.Count > 0) {
			displayed.Remove(postHumousQueue.Dequeue());
		}

		if (displayed.Count < 5 && queue.Count > 0) {
			//Hop the queues
			displayed.Add(queue.Dequeue());
		}

		int index = 0;
		foreach (Toast toast in displayed) {
			toast.Update(index);
			index ++;
		}
	}

	void OnToastDead(Toast toast) {
		postHumousQueue.Enqueue (toast);
	}

	public static void Toast(Toast toast) {
		Instance.MakeToast(toast);
	}

	public static void Toast(string message) {
		Toast toast = new Toast (message);
		Instance.MakeToast(toast);
	}

	public static void ToastError(string message) {
		if(Debug.isDebugBuild) {
			Toast toast = new Toast(message);
			toast.color = Color.red;
			toast.lifetime = 10.0f;
			Instance.MakeToast(toast);
		}
	}

	public static void ToastWarning(string message) {
		if(Debug.isDebugBuild) {
			Toast toast = new Toast(message);
			toast.color = Color.yellow;
			toast.lifetime = 10.0f;
			Debug.LogWarning(message);
			Instance.MakeToast(toast);
		}
	}

	public static void ToastDebug(string message) {
		if(Debug.isDebugBuild) {
			Toast toast = new Toast(message);
			toast.color = new Color(0.3f, 0.4f, 0.55f, 1.0f);
			toast.importance = global::Toast.Importance.PITIFUL;
			toast.lifetime = 10.0f;
			Debug.Log(message);
			Instance.MakeToast(toast);
		}
	}

	public static void Highlight(GameObject go, Color color) {
		if (Camera.main.GetComponent<HighlightEffect> () == null && Game.SupportsImageEffects()) {
			Debug.Log("Hello");
			Camera.main.gameObject.AddComponent<HighlightEffect>();
		} else {
			SimpleHighlight h = go.GetComponent<SimpleHighlight>();
			if(h != null) GameObject.Destroy(h);

			h = go.AddComponent<SimpleHighlight>();
			h.color = color;
		}

		Instance.highlightedObjects[go] = color;
		go.layer = LayerMask.NameToLayer ("Highlighted");
		foreach (Transform goTransform in go.transform) {
			goTransform.gameObject.layer = LayerMask.NameToLayer ("Highlighted");
		}
	}

	public static void Unhighlight(GameObject go) {
		if(Instance.highlightedObjects.ContainsKey(go)) {
			go.layer = 0;
			Instance.highlightedObjects.Remove(go);
		} else {
			UI.ToastWarning("Tried to unhighlight " + go.name + " when it wasnt initially highlighted.");
		}


	}
}
