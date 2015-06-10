using UnityEngine;
using System.Collections;

public class ScaleDance : MonoBehaviour {
	public float MinScale = 0.2f;
	public float MaxScale = 4.0f;
	public float SmoothStep = 0.1f;

	private float scale = 0;

	void Update () {
		scale = Mathf.Lerp (scale, Random.Range (MinScale, MaxScale), SmoothStep);
		transform.localScale = Vector3.one * scale;
	}
}
