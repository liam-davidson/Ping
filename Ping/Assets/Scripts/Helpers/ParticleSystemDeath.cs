using UnityEngine;
using System.Collections;

public class ParticleSystemDeath : MonoBehaviour {
	IEnumerator Start () {
		GetComponent<ParticleSystem>().Emit (40);
		yield return new WaitForSeconds(GetComponent<ParticleSystem>().startLifetime);
		Destroy (gameObject);
	}
}
