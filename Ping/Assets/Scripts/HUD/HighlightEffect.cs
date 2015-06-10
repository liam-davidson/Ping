using UnityEngine;
using System.Collections.Generic;

public class HighlightEffect : MonoBehaviour {

	private bool lameMode = false;
	public Material material;

	private Shader solidColorShader;
	private Shader blendTextureShader;
	private Camera highlightCamera;

	private Material blurMaterial;

	public int iterations = 4;
	public float blurSpread = 0.4f;

	void Start () {
		lameMode = !SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures;
		GameObject go = new GameObject ();
		go.name = "Highlight Camera";
		highlightCamera = go.AddComponent<Camera> ();
		highlightCamera.enabled = false;
		solidColorShader = (Shader) Resources.Load<Shader> ("SolidColorShader");
		blendTextureShader = (Shader) Resources.Load<Shader> ("HighlightShader");
		blurMaterial = new Material(Resources.Load<Shader>("BlurEffectConeTaps"));
		material = new Material (blendTextureShader);

		if(lameMode) UI.ToastDebug("Image effects not supported, forgoing highlights on this platform.");
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if(!lameMode && UI.Instance.highlightedObjects.Count > 0) {
			RenderTexture rt = RenderTexture.GetTemporary (src.width, src.height, 24);
			RenderTexture qRt = RenderTexture.GetTemporary (src.width/4, src.height/4, 24); 
			List<GameObject> nullKeys = new List<GameObject> ();

			foreach (GameObject go in nullKeys) {
				UI.Unhighlight(go);
			}

			highlightCamera.CopyFrom (Camera.main);
			highlightCamera.cullingMask = 1<<LayerMask.NameToLayer("Highlighted");
			highlightCamera.targetTexture = rt;
			highlightCamera.clearFlags = CameraClearFlags.Color;
			highlightCamera.backgroundColor = Color.clear;
			highlightCamera.RenderWithShader (solidColorShader, null);

			material.SetTexture ("_Mask", rt);

			DownSample4x (rt, qRt);

			for(int i = 0; i < iterations; i++)
			{
				RenderTexture buffer2 = RenderTexture.GetTemporary(qRt.width, qRt.height, 0);
				FourTapCone (qRt, buffer2, i);
				RenderTexture.ReleaseTemporary(qRt);
				qRt = buffer2;
			}

			material.SetTexture ("_BlendTexture", qRt);
			material.SetTexture ("_MainTexture", src);
			Graphics.Blit(src, dest, material);
			rt.Release ();
			qRt.Release();
		} else {
			Graphics.Blit(src, dest); // Otherwise the rendering pipeline cannot continue.
		}
	}

	void Update() {

	}

	public void FourTapCone (RenderTexture source, RenderTexture dest, int iteration)
	{
		float off = 0.5f + iteration*blurSpread;
		Graphics.BlitMultiTap (source, dest, blurMaterial,
		                       new Vector2(-off, -off),
		                       new Vector2(-off,  off),
		                       new Vector2( off,  off),
		                       new Vector2( off, -off)
		                       );
	}
	
	// Downsamples the texture to a quarter resolution.
	private void DownSample4x (RenderTexture source, RenderTexture dest)
	{
				float off = 1.0f;
				Graphics.BlitMultiTap (source, dest, blurMaterial,
		                       new Vector2 (-off, -off),
		                       new Vector2 (-off, off),
		                       new Vector2 (off, off),
		                       new Vector2 (off, -off)
				);
	}
}