Shader "Custom/HighlightShader" {
	Properties {
		_MainTexture ("Base", 2D) = "white" {}
		_BlendTexture ("Blend Texture", 2D) = "white" {}
		_Mask ("Mask", 2D) = "clear" {}
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTexture;
	sampler2D _BlendTexture;
	sampler2D _Mask;
	
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	half4 frag(v2f i) : COLOR 
	{ 
		float4 base = tex2D(_MainTexture, i.uv);
		float4 blended = tex2D(_BlendTexture, i.uv);
		float4 mask = tex2D(_Mask, i.uv);

		if(mask.a > 0.1) {
			blended = float4(0,0,0,0);
		}

		return lerp(base + blended, blended, blended.a);
	}

	ENDCG 
	
	Subshader {
	Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest 
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 3.0
      ENDCG
	}
	
	}
	FallBack Off

}
