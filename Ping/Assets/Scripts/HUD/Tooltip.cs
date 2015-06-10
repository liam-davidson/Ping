using UnityEngine;
using System.Collections;

public class Tooltip : MonoBehaviour {
	public string tip = "I AM ERROR";
	private Vector2 offset = Vector2.zero;
	public GUISkin skin;

	public float padding = 20f;
	public float opacity = 0.0f;

	void Start() {
		skin = (GUISkin) Resources.Load<GUISkin>("TooltipSkin");
		StartCoroutine(ShowTip ());
	}

	IEnumerator ShowTip() {
		float progress = 0;
		while(progress < 1.0f) {
			progress += Time.deltaTime * 1.5f;
			opacity = progress;
			offset.y = Mathfx.Berp(-100f, 0f, progress);
			yield return null;
		}
	}

	IEnumerator HideTip() {
		float progress = 1;
		while(progress > 0.0f) {
			progress -= Time.deltaTime * 1.5f;
			opacity = progress;
			offset.y = Mathfx.Sinerp(-100f, 0f, progress);
			yield return null;
		}

		Destroy (this);
	}

	void OnGUI() {
		GUI.skin = skin;

		Vector2 size = GUI.skin.label.CalcSize(new GUIContent(tip));
		Vector2 screenPosition = Camera.main.WorldToScreenPoint (transform.position);

		screenPosition += offset;
		screenPosition.x = Mathf.Clamp(screenPosition.x, padding, Screen.width - size.x - padding);
		screenPosition.y = Mathf.Clamp(Screen.height - screenPosition.y, padding, Screen.height - size.y - padding);

		float angle = Vector3.Angle (Camera.main.transform.forward, transform.position - Camera.main.transform.position);
		float distance = Vector3.Distance (Camera.main.transform.position, transform.position);
		float falloff = 10f / Mathf.Abs (angle) + 0.00001f;
		falloff *= Mathf.Lerp (1f, 0, distance/10f);

		Rect position = new Rect(screenPosition.x, screenPosition.y, size.x, size.y);

		skin.label.normal.textColor = new Color (0, 0, 0, 0.5f * falloff * opacity);
		GUI.Label (new Rect(screenPosition.x, screenPosition.y + 3f, size.x, size.y), tip);

		skin.label.normal.textColor = new Color (1f, 1f, 1f, 1f * falloff * opacity);
		GUI.Label (position, tip);
	}

	public void End() {
		StartCoroutine (HideTip ());
	}

	public static Tooltip Make(GameObject go, string text = "I AM ERROR") {
		Tooltip t = go.AddComponent<Tooltip> ();
		t.tip = text;
		return t;
	}
}
