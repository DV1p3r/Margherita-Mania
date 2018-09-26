Shader "Custom/AnimatedUV_B" 
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.5
		_ScrollY_B("Y Scroll", Range(0, 1)) = 1
		_ScrollX_B("X Scroll", Range(0, 1)) = 1
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
			#pragma surface surf Standard fullforwardshadows

			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};

			fixed _ScrollY_B;
			fixed _ScrollX_B;
			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				fixed2 scrolledUV = IN.uv_MainTex;
				scrolledUV += fixed2(1 - _ScrollX_B, 1 - _ScrollY_B);

				fixed4 c = tex2D(_MainTex, scrolledUV) * _Color;
				o.Albedo = c.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
