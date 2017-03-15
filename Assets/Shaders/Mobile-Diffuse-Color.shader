
Shader "Mobile/DiffuseColor" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color ("Tint", Color) = (1,1,1,1)
	[Toggle(ADD_COLOR)] _UseAddColor("Use AddColor", Float) = 0
}
SubShader {
	// inside SubShader
	Tags{ "Queue" = "Transparent" }
	LOD 150
	// inside Pass
	Fog{ Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha

	CGPROGRAM
	#pragma multi_compile __ ADD_COLOR
	#pragma surface surf NoLighting keep alpha

	sampler2D _MainTex;
	fixed4	_Color;

	fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo * _LightColor0.rgb;
		c.a = s.Alpha;
		return c;

	}

	struct Input {
		float2 uv_MainTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

		#ifdef ADD_COLOR
		o.Albedo = c.rgb + _Color.rgb;
		#else
		o.Albedo = c.rgb * _Color.rgb;
		#endif
		
		o.Alpha = c.a * _Color.a;
	}
	ENDCG
}

Fallback "Mobile/VertexLit"
}