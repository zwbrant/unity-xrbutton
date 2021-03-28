Shader "Custom/SurfShader_01"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
				_TransitionTex("Transition Texture", 2D) = "white" {}
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Cutoff("Cutoff", Range(0, 1)) = 0
		[MaterialToggle] _Distort("Distort", Float) = 0
		_Fade("Fade", Range(0, 1)) = 0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200


			 CGPROGRAM
					// Physically based Standard lighting model, and enable shadows on all light types
					#pragma surface surf Standard fullforwardshadows

					// Use shader model 3.0 target, to get nicer looking lighting
					#pragma target 3.0

					sampler2D _MainTex;

					struct Input
					{
						float2 uv_MainTex;
					};

					half _Glossiness;
					half _Metallic;
					fixed4 _Color;

					// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
					// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
					// #pragma instancing_options assumeuniformscaling
					UNITY_INSTANCING_BUFFER_START(Props)
						// put more per-instance properties here
					UNITY_INSTANCING_BUFFER_END(Props)

					void surf(Input IN, inout SurfaceOutputStandard o)
					{
						// Albedo comes from a texture tinted by color
						fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
						o.Albedo = c.rgb;
						// Metallic and smoothness come from slider variables
						o.Metallic = _Metallic;
						o.Smoothness = _Glossiness;
						o.Alpha = c.a;
					}
					ENDCG

						Pass
					{
						CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag

						#include "UnityCG.cginc"
						// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
						// put more per-instance properties here
						UNITY_INSTANCING_BUFFER_END(Props)
										struct appdata
										{
											float4 vertex : POSITION;
											float2 uv : TEXCOORD0;
										};

										struct v2f
										{
											float2 uv : TEXCOORD0;
											float2 uv1 : TEXCOORD1;
											float4 vertex : SV_POSITION;
										};

										float4 _MainTex_TexelSize;

										//v2f simplevert(appdata v)
										//{
										//	v2f o;
										//	o.vertex = UnityObjectToClipPos(v.vertex);
										//	o.uv = v.uv;
										//	return o;
										//}

										v2f vert(appdata v)
										{
											v2f o;
											o.vertex = UnityObjectToClipPos(v.vertex);
											o.uv = v.uv;
											o.uv1 = v.uv;

											#if UNITY_UV_STARTS_AT_TOP
											if (_MainTex_TexelSize.y < 0)
												o.uv1.y = 1 - o.uv1.y;
											#endif

											return o;
										}

										sampler2D _TransitionTex;
										int _Distort;
										float _Fade;

										sampler2D _MainTex;
										float _Cutoff;
										fixed4 _Color;

										fixed4 frag(v2f i) : SV_Target
										{
											fixed4 transit = tex2D(_TransitionTex, i.uv1);

											fixed2 direction = float2(0,0);
											if (_Distort)
												direction = normalize(float2((transit.r - 0.5) * 2, (transit.g - 0.5) * 2));

											fixed4 col = tex2D(_MainTex, i.uv + _Cutoff * direction);

											if (transit.b < _Cutoff)
												return col = lerp(col, _Color, _Fade);

											return col;
										}




											ENDCG
					}
		}
}
