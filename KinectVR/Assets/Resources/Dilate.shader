// Upgrade NOTE: replaced 'texRECT' with 'tex2D'

Shader "Custom/Dilate" {
    Properties
	{
		_MainTex ("_MainTex", 2D) = "white" {}
	}

    SubShader {
        Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
		
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			uniform sampler2D _MainTex;    
			uniform float _TexResX;
			uniform float _TexResY;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

			float4 _MainTex_ST; 

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				   float maxValue = 0.0;
	               
				   fixed4 texColor = tex2D (_MainTex, i.uv);
				   for(int y=-1; y<2; y++)
				     for(int x=-1; x<2; x++)
					 {
						 float val = tex2D (_MainTex, float2(i.uv.x + (float)x/_TexResX, i.uv.y +(float)y/_TexResY)).w;
						 if(val>0.0)
							maxValue=1.0;
					 }
					return fixed4(texColor.r, texColor.b, texColor.b, maxValue);
				  
            }
            ENDCG

        }
    }
}
