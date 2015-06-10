using UnityEngine;
using System.Collections;

public class SimpleHighlight : MonoBehaviour {

	private Shader[] cachedShaders;
	public Color color;

	void Start () {
		/*color.a = 0.8f;
		cachedShaders = new Shader[renderer.materials.Length];

		for(int i = 0; i < renderer.materials.Length; i++) {
			cachedShaders[i] = renderer.materials[i].shader;
			renderer.materials[i].shader = Shader.Find ("Custom/SimpleOutlineShader");
			renderer.materials[i].SetColor("_OutlineColor", color);
		}

*/
	}

	void OnDestroy() {
		/*
		for(int i = 0; i < renderer.materials.Length; i++) {
			renderer.materials[i].shader = cachedShaders[i];
		}
		*/
	}
}
