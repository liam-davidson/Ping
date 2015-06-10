using UnityEngine;
using System.Collections;

public class TimeHelper : MonoBehaviour {
	public static float ExpLerpCoefficient(float percentage) {
		return 1 - Mathf.Exp(-percentage * Time.deltaTime);
	}
}
