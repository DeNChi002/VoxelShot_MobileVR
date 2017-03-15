// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Mobile/DiffuseRim" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color ("Tint", Color) = (1,1,1,1)
	_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	_RimPower("Rim Power", Range(0.5,80.0)) = 3.0
}
SubShader {
	// inside SubShader
	Tags{ "RenderType" = "Opaque" }

	// inside Pass
	Blend SrcAlpha OneMinusSrcAlpha

	CGPROGRAM
	#pragma surface surf Lambert keep alpha

	sampler2D _MainTex;
	fixed4	_Color;
	float4 _RimColor;
	float _RimPower;

	struct Input {
		float2 uv_MainTex;
		float3 viewDir;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb * _Color.rgb;

		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
		o.Emission = _RimColor.rgb * pow(rim, _RimPower);

		o.Alpha = c.a * _Color.a;
	}
	ENDCG
}

Fallback "Mobile/VertexLit"
}
