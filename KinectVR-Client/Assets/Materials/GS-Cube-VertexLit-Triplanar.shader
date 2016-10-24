// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

 Shader "Custom/GS-Cube-VertexLit-Triplanar" 
 {        
     Properties 
     {
	_Color ("Main Color", Color) = (1,1,1,1)

     	_CubeScale ("Cube Scale", Range (0.0, 10.0)) = 1
     	_TriplanarScale ("Triplanar Scale", Range (0.0, 10.0)) = 1
     	_GeometryShading ("Geometry Shading", Range (0.0, 1)) = 0.5
     	
     	
		_MainTex ("Top Diffuse (RGB)", 2D) = "white" {}
		_SideTex ("Side Diffuse (RGB)", 2D) = "white" {}
		_BottomTex ("Bottom Diffuse (RGB)", 2D) = "white" {}

     }
     
     SubShader 
     {
         LOD 200
         
		Pass {
			Tags { "LightMode" = "Vertex" }
			Lighting On
             CGPROGRAM
 
             #pragma only_renderers d3d11
             #pragma target 5.0
             #include "UnityCG.cginc"
 
             #pragma vertex   myVertexShader
             #pragma geometry myGeometryShader
             #pragma fragment myFragmentShader
             
             #define TAM 36
             

			                   
             struct vIn // Into the vertex shader
             {
                 float4 vertex : POSITION;
                 float3 normal : NORMAL;
             };
             
             struct gIn // OUT vertex shader, IN geometry shader
             {
                 float4 pos : SV_POSITION;
             
                 float3 worldPos : NORMAL;
                 float4 vertWorldNormals : TANGENT;
                 fixed4 diff : COLOR0;
                 	
             };
             
              struct v2f // OUT geometry shader, IN fragment shader 
             {
                 float4 pos           : SV_POSITION;
                 float2 uv_MainTex : TEXCOORD0;
             	float2 uv_SideTex : TEXCOORD1;
             	float2 uv_BottomTex : TEXCOORD2;
                 float3 worldPos : NORMAL;
                 float4 vertWorldNormals : TANGENT;
                 fixed4 diff : COLOR0;
                 fixed4 aodiff : COLOR1;
                 
             };
        float _CubeScale; 
        float4 _MainTex_ST;
        float4 _SideTex_ST;
        float4 _BottomTex_ST;
        
		sampler2D _MainTex;
		sampler2D _SideTex;
		sampler2D _BottomTex;

		float _TriplanarScale;

		
		float _GeometryShading;

             
             
uniform float4 _Color;
  
  
             // ----------------------------------------------------
             gIn myVertexShader(appdata_full v)
             {
                 gIn o; // Out here, into geometry shader
                 // Passing on color to next shader (using .r/.g there as tile coordinate)
                 // Passing on center vertex (tile to be built by geometry shader from it later)
                 o.pos = v.vertex;
                 o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                 o.vertWorldNormals = mul( unity_ObjectToWorld, float4( v.normal, 0.0 ) );	
                 // Caclulate center vertex light diffuse color and send it to Geometry Shader as COLOR0
				float4 lighting = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true),1);
				o.diff = lighting * _Color;
                 return o;
             }
             
             

             // ----------------------------------------------------
             
             [maxvertexcount(TAM)] 
             // ----------------------------------------------------
             
             void myGeometryShader(triangleadj gIn vert[6], inout TriangleStream<v2f> triStream)
             {                            
                 float f = _CubeScale/20.0f; //half size
                 float d = 1.0f;
                 
                 const float4 vc[TAM] = { float4( -f,  f,  f, 0.0f), float4(  f,  f,  f, 0.0f), float4(  f,  f, -f, 0.0f),    //Top                                 
                                          float4(  f,  f, -f, 0.0f), float4( -f,  f, -f, 0.0f), float4( -f,  f,  f, 0.0f),    //Top
                                          
                                          float4(  f,  f, -f, 0.0f), float4(  f,  f,  f, 0.0f), float4(  f, -f,  f, 0.0f),     //Right
                                          float4(  f, -f,  f, 0.0f), float4(  f, -f, -f, 0.0f), float4(  f,  f, -f, 0.0f),     //Right
                                          
                                          float4( -f,  f, -f, 0.0f), float4(  f,  f, -f, 0.0f), float4(  f, -f, -f, 0.0f),     //Front
                                          float4(  f, -f, -f, 0.0f), float4( -f, -f, -f, 0.0f), float4( -f,  f, -f, 0.0f),     //Front
                                          
                                          float4( -f, -f, -f, 0.0f), float4(  f, -f, -f, 0.0f), float4(  f, -f,  f, 0.0f),    //Bottom                                         
                                          float4(  f, -f,  f, 0.0f), float4( -f, -f,  f, 0.0f), float4( -f, -f, -f, 0.0f),     //Bottom
                                          
                                          float4( -f,  f,  f, 0.0f), float4( -f,  f, -f, 0.0f), float4( -f, -f, -f, 0.0f),    //Left
                                          float4( -f, -f, -f, 0.0f), float4( -f, -f,  f, 0.0f), float4( -f,  f,  f, 0.0f),    //Left
                                          
                                          float4( -f,  f,  f, 0.0f), float4( -f, -f,  f, 0.0f), float4(  f, -f,  f, 0.0f),    //Back
                                          float4(  f, -f,  f, 0.0f), float4(  f,  f,  f, 0.0f), float4( -f,  f,  f, 0.0f)     //Back
                                          };
                 
                 const float3 top_normal = float3(0,1,0);
                 const float3 right_normal = float3(1,0,0);
                const float3 front_normal = float3(0,0,-1);
                 const float3 bottom_normal = float3(0,-1,0);
                 const float3 left_normal = float3(-1,0,0);
                 const float3 back_normal = float3(0,0,1);                 
                                                                              
                 const float3 nc[TAM] = { top_normal,top_normal,top_normal,    //Top                                 
                                          top_normal,top_normal,top_normal,    //Top
                                          
                                         right_normal,right_normal,right_normal,     //Right
                                          right_normal,right_normal,right_normal,     //Right
                                          
                                         front_normal,front_normal,front_normal,     //Front
                                         front_normal,front_normal,front_normal,    //Front
                                          
                                          bottom_normal,bottom_normal,bottom_normal,    //Bottom                                         
                                          bottom_normal,bottom_normal,bottom_normal,    //Bottom
                                          
                                          left_normal,left_normal,left_normal,    //Left
                                          left_normal,left_normal,left_normal,    //Left
                                          
                                         back_normal,back_normal,back_normal,   //Back
                                          back_normal,back_normal,back_normal    //Back
                                          };
                 
                 // Each cube is properly UV mapped, although this shader uses triplanar mapping.
                 
                 const float2 UV1[TAM] = { float2( 0.0f,    1.0f ), float2( 1.0f,    1.0f ), float2( 1.0f,    0.0f ),         
                                           float2( 1.0f,    0.0f ), float2( 0.0f,    0.0f ), float2( 0.0f,    1.0f ),         
                                           
                                           float2( 0.0f,    1.0f ), float2( 1.0f,    1.0f ), float2( 1.0f,    0.0f ),         
                                           float2( 1.0f,    0.0f ), float2( 0.0f,    0.0f ), float2( 0.0f,    1.0f ),   
                                           
                                           float2( 0.0f,    1.0f ), float2( 1.0f,    1.0f ), float2( 1.0f,    0.0f ),         
                                           float2( 1.0f,    0.0f ), float2( 0.0f,    0.0f ), float2( 0.0f,    1.0f ),   
                                           
                                           float2( 0.0f,    1.0f ), float2( 1.0f,    1.0f ), float2( 1.0f,    0.0f ),         
                                           float2( 1.0f,    0.0f ), float2( 0.0f,    0.0f ), float2( 0.0f,    1.0f ),   
                                           
                                           float2( 0.0f,    1.0f ), float2( 1.0f,    1.0f ), float2( 1.0f,    0.0f ),         
                                           float2( 1.0f,    0.0f ), float2( 0.0f,    0.0f ), float2( 0.0f,    1.0f ),   
                                           
                                           float2( 0.0f,    1.0f ), float2( 1.0f,    1.0f ), float2( 1.0f,    0.0f ),         
                                           float2( 1.0f,    0.0f ), float2( 0.0f,    0.0f ), float2( 0.0f,    1.0f )                                         
                                             };    
                                                             
                 const int TRI_STRIP[TAM]  = {  0, 1, 2,  3, 4, 5,
                                                6, 7, 8,  9,10,11,
                                               12,13,14, 15,16,17,
                                               18,19,20, 21,22,23,
                                               24,25,26, 27,28,29,
                                               30,31,32, 33,34,35  
                                               }; 
                                               
                         
                 v2f v[TAM];
                 int i;
                 
                 // Assign new vertices positions 
                 for (i=0;i<TAM;i++) {
                 
                   v[i].pos = vert[0].pos + vc[i];   
                   	
                   	
                   	//pass through vertex shader variables to fragment shader
	                     v[i].worldPos = vert[0].worldPos+(mul(unity_ObjectToWorld, v[i].pos).xyz)/ _TriplanarScale;
	                     v[i].vertWorldNormals = float4(nc[i],0);
	                     //pass through center vertex light
	                     v[i].aodiff = vert[0].diff;

 						//calculate lighting per each new vertex created in Geometry Shader using only the main light.
				            float3 normalDirection = nc[i];
				            float3 lightDirection = normalize(mul(unity_LightPosition[0],UNITY_MATRIX_IT_MV).xyz);
				 
				            float3 diffuseReflection = unity_LightColor[0].rgb * _Color.rgb * max(0.0, dot(normalDirection, lightDirection));
				 			float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
          
                    //send final new light based on cube normal
                     v[i].diff = float4(lightColor + diffuseReflection, 1.0); 

            	             	
                    
                        }
               
 
                 // Assign UV values
                 for (i=0;i<TAM;i++)	{
                  v[i].uv_MainTex = TRANSFORM_TEX(UV1[i],_MainTex);
                  v[i].uv_SideTex = TRANSFORM_TEX(UV1[i],_SideTex);
                  v[i].uv_BottomTex = TRANSFORM_TEX(UV1[i],_BottomTex); 
                 
                   }

                 
                 // Position in view space
                 for (i=0;i<TAM;i++) { 
                 v[i].pos = mul(UNITY_MATRIX_MVP, v[i].pos); 
                 
                 }
                     
                 // Build the cube tile by submitting triangle strip vertices
                 for (i=0;i<TAM/3;i++)
                 { 
                     triStream.Append(v[TRI_STRIP[i*3+0]]);
                     triStream.Append(v[TRI_STRIP[i*3+1]]);
                     triStream.Append(v[TRI_STRIP[i*3+2]]);    
                                     
                     triStream.RestartStrip();
                 }
              }
              float length(float3 v)
				{
				  return sqrt(dot(v,v));
				}
              // ----------------------------------------------------
             float4 myFragmentShader(v2f IN) : COLOR
             {

			
			
			// Get normal direction
			float3 NormalizedWorldNormals = abs(IN.vertWorldNormals)*abs(IN.vertWorldNormals);
			float wnY = NormalizedWorldNormals.y;
			float wnZ = NormalizedWorldNormals.z;
			float wnX = NormalizedWorldNormals.x;

			// Project textures using world space
			half4 topXZ = tex2D (_MainTex, IN.worldPos.xz);	
			half4 bottomXZ = tex2D (_BottomTex, IN.worldPos.xz);	
			half4 sideXY = tex2D (_SideTex, IN.worldPos.xy );
			half4 sideYZ = tex2D (_SideTex, IN.worldPos.yz );

			
			half4 tri;
			
			
			//if top
			if (IN.vertWorldNormals.y > 0){
				tri.rgb = topXZ.rgb*wnY + sideXY.rgb*wnZ + sideYZ.rgb*wnX;
			}
			else{
			//if bottom
			tri.rgb = bottomXZ.rgb*wnY + sideXY.rgb*wnZ + sideYZ.rgb*wnX;
			}
			
			//mix final Directional light per each new vertex from Geometry Shader with regular Vertex lighting per center vertex.
			half4 finalShading = IN.diff * (lerp(IN.diff,IN.aodiff,  _GeometryShading));
			tri = float4(tri.rgb*finalShading.rgb,1);

			return tri;

			
                 
                 
             }
 
             ENDCG
         }
     } 
 }