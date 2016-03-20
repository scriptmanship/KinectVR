Shader "Kinect/Depth2ColorShader" {
    Properties {
        _BodyTex ("Body (RGB)", 2D) = "white" {}
        _ColorTex ("Color (RGB)", 2D) = "white" {}
    }
    
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
		
			CGPROGRAM
			#pragma target 5.0
			//#pragma enable_d3d11_debug_symbols

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _BodyTex;
			uniform sampler2D _ColorTex;
			
			uniform float _ColorResX;
			uniform float _ColorResY;
			uniform float _DepthResX;
			uniform float _DepthResY;

			StructuredBuffer<float2> _ColorCoords;

			struct v2f {
				float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				
				return o;
			}

			float4 frag (v2f i) : COLOR
			{
				float player = tex2D(_BodyTex, i.uv).r;
				
				if (player != 0)
				{
					int dx = (int)(i.uv.x * _DepthResX);
					int dy = (int)(i.uv.y * _DepthResY);
					int di = (int)(dx + dy * _DepthResX);
					
					if (!isinf(_ColorCoords[di].x) && !isinf(_ColorCoords[di].y))
					{
						float ci_index, ci_length;
						ci_index = _ColorCoords[di].x + (_ColorCoords[di].y * _ColorResX);
						ci_length = _ColorResX * _ColorResY;
					
						if(ci_index >= 0 && ci_index < ci_length)
						{
							float2 ci_uv;
							ci_uv.x = _ColorCoords[di].x / _ColorResX;
							ci_uv.y = _ColorCoords[di].y / _ColorResY;
							
							float4 clr = tex2D (_ColorTex, ci_uv);
							clr.w = player < 0.8 ? player : 1;
							return clr;
						}
					}
				}
				
				return float4(0, 0, 0, 0);
			}

			ENDCG
		}
	}

	Fallback Off
}