// Upgrade NOTE: replaced 'PositionFog()' with multiply of UNITY_MATRIX_MVP by position
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'

Shader "Custom/AlphaRimLight" {
   
    Properties {
        //_Color ("Main Color", Color) = (1,1,1,1)
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
      //  _MainTex ("Alpha Map", 2D) = "white" {}
        _RimPower ("Rim Power", Range(0.0,8.0)) = 0.5
		_Alpha ("Alpha" , Range(0.0,1.0))= 1.0 
    }
   
    SubShader {
       Pass{
         Tags { "RenderType"="Opaque" "Queue"="Geometry-1"}
        	 ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            half4 frag(v2f i) : COLOR {
                return half4(0, 0, 0, 0);
            }

            ENDCG

        }
        Pass {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
           Blend One One

           ZWrite On

           Cull Off
            CGPROGRAM
               
                #pragma vertex vert 
                #pragma fragment frag keepalpha
                #include "UnityCG.cginc"

                struct appdata {
                    fixed4 vertex : POSITION;
                    fixed3 normal : NORMAL;
                    fixed2 texcoord : TEXCOORD0;
                };
               
                struct v2f {
                    float4 pos : SV_POSITION;
                    fixed2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };
               
                uniform fixed4 _MainTex_ST;
                uniform fixed4 _RimColor;
                uniform fixed _RimPower;
				uniform fixed _Alpha;
               
                v2f vert (appdata_base v) {
                    v2f o;
                    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                   
                    fixed3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                    fixed dotProduct = 1 - dot(v.normal, viewDir);
                    fixed rimWidth = 0.7;
                    o.color = smoothstep(1 - _RimPower, 1.0, dotProduct);
                   
                    o.color *= _RimColor;
                   
                    //o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                   
                    return o;
                }
               
               // uniform sampler2D _MainTex;
                uniform fixed4 _Color;
               
                fixed4 frag(v2f i) : SV_Target {
                    //fixed4 alphamap = tex2D(_MainTex, i.uv);
					
                    fixed4 texcol;
                    texcol.rgb = i.color ;//* alphamap.r;
					texcol.a = _Alpha;
                    return texcol;
                }
               
            ENDCG
        }


    }
}