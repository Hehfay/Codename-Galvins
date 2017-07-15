
Shader "test/testShader"
{
	Properties {
		_MainTex("Texture", 2D) = "white" { }
		_OverlayTex("Overlay Texture", 2D) = "white" {}
		_Outline ("Outline Thickness", float) = 0.0
		_OutlineColor ("Outline Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}

	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// TODO: make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			uniform float4 _Color;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _OverlayTex;

			uniform float4 _LightColor0;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};
			struct vertexOutput {
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};

			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				
				float3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				float3 lightDirection;
				float atten = 1.0;

				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuseReflection = atten * _LightColor0.xyz * _Color.rgb * max(0.0, dot(normalDirection, lightDirection));
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.col = float4(0.0, 0.0, 0.6, 0.25 * _SinTime.w + 0.25);
				float4 f = float4(diffuseReflection, 1.0);
				o.col = f;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//o.uv = v.uv;

				return o;
			}

			float4 frag(vertexOutput i) : SV_Target {
				//fixed4 mainCol = tex2D(_MainTex, i.uv);
				int lod = 1;
				fixed4 mainCol = tex2D(_MainTex, float4(i.uv.xy, 0.0, 0.0));
				fixed4 overlayCol = tex2D(_OverlayTex, i.uv);
				i.col = mainCol; //lerp(mainCol, overlayCol, 0.4 * sin(_Time.w) + 0.4);
				return i.col;
			}
			ENDCG
		}

		//Fallback "Diffuse"
	}
}
