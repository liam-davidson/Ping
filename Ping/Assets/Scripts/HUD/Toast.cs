using UnityEngine;
using System;
using System.Collections.Generic;

public class Toast {
	private const float SEPARATION = 30f;

	public delegate void OnToastDeadHandler(Toast toast);
	public event OnToastDeadHandler onToastDeadEvent;

	public enum Anchor {
		TOP_LEFT,
		TOP_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_RIGHT
	}

	public enum Importance {
		NORMAL,
		PITIFUL,
		IMPORTANT
	}

	public string message = "";
	public Anchor anchor = Anchor.TOP_LEFT;
	public Importance importance = Importance.NORMAL;
	public AudioClip sound;
	public float lifetime = 5.0f;
	public Color color = Color.white;

	public float top = 0;
	private float displayTop = -100f;
	private List<Toast> queue;
	private float bounce;

	public Toast(string message) {
		this.message = message.ToUpper();
	}

	public void Draw(GUIStyle style) {
		Rect container = new Rect (0f, displayTop, Screen.width, Screen.height);
		Rect shadowContainer = new Rect (0f, displayTop + 2f, Screen.width, Screen.height);

		style.fontStyle = FontStyle.Normal;

		if (importance == Importance.IMPORTANT) {
			style.fontStyle = FontStyle.Bold;
		}

		if (importance == Importance.PITIFUL) {
			style.fontStyle = FontStyle.Italic;
		}

		GUIStyle shadowStyle = style;

		shadowStyle.normal.textColor = Color.Lerp (Color.clear, new Color (0, 0, 0, 0.1f), lifetime/0.5f);
		GUI.Label (shadowContainer, message, shadowStyle);
		style.normal.textColor = Color.Lerp (new Color(1f,1f,1f,0), color, lifetime/0.5f);
		GUI.Label (container, message, style);
	}

	public void Update(int position) {
		top = position * SEPARATION;
		displayTop = Mathf.Lerp (displayTop, top, 0.2f);
		lifetime -= Time.deltaTime;
		if(lifetime < 0f) Die();
	}

	public void Die() {
		onToastDeadEvent (this);
	}
}
