
Shader "Mobile/DiffuseColorZ" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color ("Tint", Color) = (1,1,1,1)
}

SubShader {
	// inside SubShader
	Tags { "RenderType"="Transparent" "Queue"="Transparent+2" }
	ColorMask 0
	LOD 150
	// inside Pass
	Fog{ Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha
	ZWrite On
	ZTest Always
	Cull Off

	CGPROGRAM
	//#pragma multi_compile __ ADD_COLOR
	#pragma surface surf NoLighting keep alpha

	sampler2D _MainTex;
	fixed4	_Color;

	fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}

	struct Input {
		float2 uv_MainTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

		o.Albedo = c.rgb + _Color.rgb;
		
		o.Alpha = c.a * _Color.a;
	}
	ENDCG
}

Fallback "Mobile/VertexLit"
}