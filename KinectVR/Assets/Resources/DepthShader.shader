Shader "Kinect/DepthShader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "black" {}
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

			uniform sampler2D _MainTex;
			uniform float _TexResX;
			uniform float _TexResY;
			uniform float _TotalPoints;
			uniform int _FirstUserIndex;

			StructuredBuffer<float> _DepthBuffer;
			StructuredBuffer<float> _HistBuffer;
			StructuredBuffer<float> _BodyIndexBuffer;

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
				int dx = (int)(i.uv.x * _TexResX);
				int dy = (int)(i.uv.y * _TexResY);
				int di = (int)(dx + dy * _TexResX);
				
				//float player = tex2D(_MainTex, i.uv).r;
				//int playerIndex = (int)(player * 255);
				
				float playerIndex = (int)_BodyIndexBuffer[di];
				
				//if (player != 0)
				if (playerIndex != 255)
				{
					int depth = (int)_DepthBuffer[di];
					float hist = 1 - (_HistBuffer[depth] / _TotalPoints);
					
					if((playerIndex % 8) == _FirstUserIndex)
					{
						return float4(hist, hist, 0, 1);  // yellow
					}
					else
					{
						switch(playerIndex % 4)
						{
							case 0:
								return float4(hist, 0, 0, 0.9);  // red
							case 1:
								return float4(0, hist, 0, 0.9);  // green
							case 2:
								return float4(0, 0, hist, 0.9);  // blue
							case 3:
								return float4(hist, 0, hist, 0.9);  // magenta
						}
					}
					
					return float4(hist, hist, hist, 1); // white
				}
//				else
//				{
//					float depth = _DepthBuffer[di];
//					if(depth == 0) depth = 5000;
//					float hist = 1 - depth / 5000;
//					
//					return float4(hist, hist, hist, 1); // gray
//				}
				
				return float4(0, 0, 0, 0);  // invisible
			}

			ENDCG
		}
	}

	Fallback Off
}