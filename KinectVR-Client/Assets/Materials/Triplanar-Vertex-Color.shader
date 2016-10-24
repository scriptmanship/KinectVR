// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "AAA-Scriptmanship/Triplanar-Vertex-Color" {
	Properties {
	_VertexAOMultiplier ("Vertex AO Multiplier", Range (0.0, 1.0)) = 0.5
	
		_MainTex ("Top Diffuse (RGB)", 2D) = "white" {}
		_SideTex ("Side Diffuse (RGB)", 2D) = "white" {}
		_BottomTex ("Bottom Diffuse (RGB)", 2D) = "white" {}
		//_BumpTex ("Cliff (RGB)", 2D) = "white" {}
		_MainScale ("Top Scale", Range (0.0, 100.0)) = 1
		_SideScale ("Side Scale", Range (0.0, 100.0)) = 1
		_BottomScale ("Bottom Scale", Range (0.0, 100.0)) = 1
		
		
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		sampler2D _SideTex;
		sampler2D _BottomTex;
		//sampler2D _BumpTex;
		float _MainScale;
		float _SideScale;
		float _BottomScale;
		float _VertexAOMultiplier;

		struct Input {
			float3 vertWorldNormals;
			//float3 vertLocalNormals;
			float4 vertColors;
			float3 worldPos;
		};
		
		
		void vert (inout appdata_full v, out Input o)
		{
		UNITY_INITIALIZE_OUTPUT(Input,o);
		
		
			o.vertWorldNormals = normalize(mul( unity_ObjectToWorld, float4( v.normal, 0.0 ) ).xyz);
			
			
			o.vertColors = v.color;
			//o.vertLocalNormals = normalize(v.normal);
			
			
			//o.normals = mul((float3x3)UNITY_MATRIX_IT_MV, input.normal);
		}
		

		void surf (Input IN, inout SurfaceOutput o) {

			float3 NormalizedWorldNormals = abs(IN.vertWorldNormals)*abs(IN.vertWorldNormals);
			float wnY = NormalizedWorldNormals.y;
			float wnZ = NormalizedWorldNormals.z;
			float wnX = NormalizedWorldNormals.x;



			half4 topXZ = tex2D (_MainTex, IN.worldPos.xz / _MainScale);	
			half4 bottomXZ = tex2D (_BottomTex, IN.worldPos.xz / _BottomScale);	
			half4 sideXY = tex2D (_SideTex, IN.worldPos.xy / _SideScale);
			half4 sideYZ = tex2D (_SideTex, IN.worldPos.yz / _SideScale);


			
			//half4 wnYZ = tex2D (_BumpTex, IN.worldPos.yz * _SideScale);
			//half4 wnXY = tex2D (_BumpTex, IN.worldPos.xy * _SideScale);	
			
			half4 tri;
			
			if (IN.vertWorldNormals.y > 0)
			tri.rgb = topXZ.rgb*wnY + sideXY.rgb*wnZ + sideYZ.rgb*wnX;
			else
			tri.rgb = bottomXZ.rgb*wnY + sideXY.rgb*wnZ + sideYZ.rgb*wnX;
			

			//o.Albedo =  IN.vertColors;
			//o.Albedo = lerp(pow(IN.vertColors,_VertexAOMultiplier)*tri.rgb, tri.rgb, 0.5);
			o.Albedo = pow(IN.vertColors,_VertexAOMultiplier)*tri.rgb;
			//o.Normal = 1*wnZ + wnYZ.rgb*wnX + wnXY.rgb*wnY;
			//o.Specular = 1*wnZ + wnYZ.rgb*wnX + wnXY.rgb*wnY;
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
