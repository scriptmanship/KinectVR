// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "AAA-Scriptmanship/SlopeProjection" {
	Properties {
		//_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainScale ("Main Scale", Range (0.0, 10.0)) = 5
		_MainTex ("Main Diffuse (RGB)", 2D) = "white" {}
		//_MainBump ("Main Normal (RGB)", 2D) = "white" {}
		
	
		_SlopeScale ("Slope Scale", Range (0.0, 10.0)) = 5
		_SlopeTex ("Slope Diffuse (RGB)", 2D) = "white" {}
		//_SlopeBump ("Slope Normal (RGB)", 2D) = "white" {}
		
		_DetailScale ("Detail Scale", Range (0.0, 10.0)) = 5
		_DetailMainMix ("Detail Main Mix", Range (0.0, 1.0)) = 0.5
		_DetailSlopeMix ("Detail Slope Mix", Range (0.0, 1.0)) = 0.5		
		_DetailTex ("Detail Diffuse (RGB)", 2D) = "gray" {}

		


		_FadeLength ("Fade Legth", Range (0.0, 1000.0)) = 5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		//sampler2D _MainBump;
		
		sampler2D _SlopeTex;
		//sampler2D _SlopeBump;
		
		sampler2D _DetailTex;
		
		float _MainScale;
		float _SlopeScale;		
		float _DetailScale;
		
		half _DetailMainMix;
		half _DetailSlopeMix;
				
		half _FadeLength;

		//half _Shininess;

		struct Input {
			//float2 uv_MainTex;
			//float2 uv_SlopeBump;
			//float3 vertNormals;
			float3 vertPos;
			float3 worldPos;
			
			//float3 worldRefl; INTERNAL_DATA
		};
		
		
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			//o.vertNormals = v.normal;
			
			
			
			float3 worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
			
			o.vertPos = worldPos;
			
			//if (worldPos.y > 0 && worldPos.y < 5)
			//v.normal = float3(0.1,0.1,0.5);
			
		}
		

		void surf (Input IN, inout SurfaceOutput o) {
			//float3 VertNormals = abs(IN.vertNormals)*abs(IN.vertNormals);
			
			//float3 NormalizedPositions = abs(IN.vertPos)*abs(IN.vertPos);
			float3 NormalizedPositions = IN.vertPos;

			//float vXZ = NormalizedNormals.z;
			//float vYZ = NormalizedNormals.x;
			//float vXY = NormalizedNormals.y;
			
	
			
			half4 MainTex = tex2D (_MainTex, IN.worldPos.xz * _MainScale / 30);
			half4 SlopeTex = tex2D (_SlopeTex, IN.worldPos.xz * _SlopeScale / 30);
			//half4 cXY = tex2D (_SlopeTex, IN.worldPos.xy * _SlopeScale);
			
			
			//half4 MainBump = tex2D (_MainBump, IN.worldPos.xz * _MainScale / 30);
			//half4 SlopeBump = tex2D (_SlopeBump, IN.worldPos.xz * _SlopeScale / 30);
			//half4 nYZ = tex2D (_SlopeBump, IN.worldPos.yz * _SlopeScale);
			//half4 nXY = tex2D (_SlopeBump, IN.worldPos.xy * _SlopeScale);	
			
			half4 DetailTex = tex2D (_DetailTex, IN.worldPos.xz * _DetailScale / 30);
			
			half4 col;
			//half4 bump;
			
			half beginY = 0.01;
			if (NormalizedPositions.y < beginY){
				col = lerp(MainTex,DetailTex,_DetailMainMix);
				//bump = MainBump;
			}
			else if (NormalizedPositions.y < 2.0+_FadeLength){
				float fade = NormalizedPositions.y - beginY;
				fade *= 1/(2+_FadeLength - beginY);
				col.rgb = lerp(SlopeTex,DetailTex,_DetailSlopeMix).rgb*fade + lerp(MainTex,DetailTex,_DetailMainMix).rgb*(1-fade);
				//bump.rgb = SlopeBump.rgb*fade + MainBump.rgb*(1-fade);
			}
			else{
				//col.rgb = (SlopeTex.rgb + cXY.rgb * 0.2) / 1.2 ;
				
				col = lerp(SlopeTex,DetailTex,_DetailSlopeMix);
				//bump = SlopeBump;
			}
			//col.rgb = MainTex.rgb*vXZ + SlopeTex.rgb*vYZ + cXY.rgb*vXY;
			//o.Normal = bump.rgb;
				o.Albedo = col.rgb;

			//o.Alpha = MainTex.a;
			
	//o.Gloss = nYZ.a;
	//o.Specular = _Shininess;
			//o.Specular = 1*vXZ + nYZ.rgb*vYZ + nXY.rgb*vXY;
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
