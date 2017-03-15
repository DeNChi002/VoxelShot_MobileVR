//
// Diffuseと、法線情報のスケーリングで頂点アウトラインだすやつ
//
Shader "Custom/Silhouetted Diffuse" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_SpecColor("Spec Color", Color) = (1,1,1,1)
		_Emission("Emmisive Color", Color) = (0,0,0,0)
		_Shininess("Shininess", Range(0.01, 1)) = 0.7
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (0.0, 0.03)) = .005
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_UseShiruetto("Use Shiruetto", Float) = 0
	}
	 
	CGINCLUDE
	#include "UnityCG.cginc"
	 
	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	 
	struct v2f {
		float4 pos : POSITION;
		float4 color : COLOR;
	};
	 
	uniform float _Outline;
	uniform float4 _OutlineColor;
	uniform float _UseShiruetto;
	
	ENDCG
 
	SubShader {
		Tags { "Queue" = "Transparent" }

		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Off
			ZWrite Off
			// 他のモデルに遮蔽された場合にシルエットを全面描画する際は外しとく
			//ZTest Always
			ColorMask RGB
 
			Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			//Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
 
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			 
			v2f vert(appdata v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				if (_UseShiruetto > 0.0f) {
					float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
					float2 offset = TransformViewToProjection(norm.xy);

					o.pos.xy += offset * o.pos.z * _Outline;
					o.pos.z += 0.001f;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex + (float4(v.normal, 0) * _Outline));
					o.color = _OutlineColor;
				}
				else {
					o.color.a = 0.0f;
				}
				return o;
			}

			half4 frag(v2f i) :COLOR{
				return i.color;
			}

			ENDCG
		}
 
		Pass {
			Name "BASE"
			ZWrite On
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			Material {
				Diffuse [_Color]
				Ambient [_Color]
				Shininess[_Shininess]
				Specular[_SpecColor]
				Emission[_Emission]
			}
			Lighting On
			SetTexture [_MainTex] {
				ConstantColor [_Color]
				Combine texture * constant
			}
			SetTexture [_MainTex] {
				Combine previous * primary DOUBLE
			}
		}
	}
	Fallback "Diffuse"
}