Shader "Custom/PaintShaders/DrawingShader" {
	Properties{
		_Draw("Draw (RGB) Trans (A)", 2D) = "" {}
		_CutTex("Cutout (A)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}

		SubShader
		{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert alpha

			sampler2D _Draw;
			sampler2D _CutTex;
			float _Cutoff;

			struct Input {
				float2 uv_Draw;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_Draw, IN.uv_Draw);
				float ca = tex2D(_CutTex, IN.uv_Draw).a;
				o.Albedo = c.rgb;

				if (ca > 0)
					o.Alpha = c.a;
				else
					o.Alpha = 0;
			}
			ENDCG
		}

		Fallback "Transparent/VertexLit"
}