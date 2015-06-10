Shader "Custom/SolidColorShader" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		ZTest Off
		Blend One OneMinusSrcAlpha
		Fog { Mode off }
		
		CGPROGRAM
      	#pragma surface surf Lambert
      	      	
      	struct Input {
          float4 color : COLOR;
      	};
      	
      	void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = float3(1,1,1);
          o.Alpha = 1;
      	}
      	ENDCG
	} 
	FallBack Off
}
