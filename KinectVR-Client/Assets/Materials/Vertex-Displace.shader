// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "AAA-Scriptmanship/Vertex-Displace" {
	Properties {
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5	
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Diffuse (RGB)", 2D) = "white" {}
		_HeightmapTex ("Displace (RGB)", 2D) = "white" {}


		_Displacement ("Displacement", Range(-50,50)) = -5.0
		_Scale ("Scale", Range(1,5)) = 1.0
		_SpeedX ("Speed X", Range(-5,5)) = 0.0
		_SpeedY ("Speed Y", Range(-5,5)) = 0.5		


	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _HeightmapTex;
		float _Displacement;
		float _Scale;
		float _SpeedX;
		float _SpeedY;

		struct Input {
			float2 uv_MainTex;
			float2 uv_HeightmapTex;
			float3 worldPos;
			//half4 diff;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Speed;
		
		void vert (inout appdata_full v, out Input o)
		{
		UNITY_INITIALIZE_OUTPUT(Input,o);
		
		float4 uv = float4(v.texcoord.xy*_Scale,0,0);
		uv.x += _Time*_SpeedX;
		uv.y += _Time*_SpeedY;
		
		float4 tex = tex2Dlod (_HeightmapTex, uv);
		
		float4 vertworld = mul(unity_ObjectToWorld, v.vertex);
		
		float avgcol = (tex.r + tex.g + tex.b)/3;
		
		vertworld.y += avgcol*_Displacement;
		v.vertex = mul(unity_WorldToObject, vertworld);
		

		
		}
		
		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
