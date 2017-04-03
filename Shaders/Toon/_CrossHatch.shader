Shader "Hidden/CrossHatch"
{
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
		_HatchTex ("Hatching Texture", 2D) = "white" {}

		_outlineColor("Outline Color", Color) = (0,0,0,1)
		_outlineThickness ("Outline Thickness", Range(0.0, 0.025)) = 0.01
		_outlineThickness(" ", Float) = 0.01
		_outlineShift ("Outline Light Shift", Range(0.0, 0.025)) = 0.01
		_outlineShift(" ", Float) = 0.01
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		//UsePass "M8/Toon/Outline/OUTLINE"
	
		Pass {
			Tags { "LightMode"="ForwardBase" }
			
			ZWrite On
			
			CGPROGRAM
			
			#pragma vertex vtx
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			
			float4 _LightColor0; 

			sampler2D _MainTex;
			sampler2D _HatchTex;
			fixed4 _Color;

			struct Vert {
				float4 vertex 	: 	POSITION;
				fixed2 texCoord : 	TEXCOORD0;
				fixed4 normal 	:	NORMAL;
			};
			
			struct Pix {
				float4 pos				  :	SV_POSITION;
				fixed2 texCoord			  :	TEXCOORD0;
				float3 normal   		  :	TEXCOORD1;
				float3 lightDir			  : TEXCOORD2;
				half4 ambientOrLightmapUV :	TEXCOORD3;
				
				LIGHTING_COORDS(4,5)
			};
			
			Pix vtx(Vert v) {
				Pix o;
																								
				o.pos = UnityObjectToClipPos(v.vertex);
				o.texCoord = v.texCoord;
				o.normal = normalize(v.normal).xyz;
				
				o.lightDir = normalize(ObjSpaceLightDir(v.vertex));
								
				//spherical harmonics and vertex lights
				float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
				float3 normalWorld = UnityObjectToWorldNormal(v.normal);
				
				TRANSFER_VERTEX_TO_FRAGMENT(o);

				// Static lightmaps
				#ifndef LIGHTMAP_OFF
					o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					o.ambientOrLightmapUV.zw = 0;
				// Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
				#elif UNITY_SHOULD_SAMPLE_SH
					#if UNITY_SAMPLE_FULL_SH_PER_PIXEL
						o.ambientOrLightmapUV.rgb = 0;
					#elif (SHADER_TARGET < 30)
						o.ambientOrLightmapUV.rgb = ShadeSH9(half4(normalWorld, 1.0));
					#else
						// Optimization: L2 per-vertex, L0..L1 per-pixel
						o.ambientOrLightmapUV.rgb = ShadeSH3Order(half4(normalWorld, 1.0));
					#endif
					// Add approximated illumination from non-important point lights
					#ifdef VERTEXLIGHT_ON
						o.ambientOrLightmapUV.rgb += Shade4PointLights (
							unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
							unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
							unity_4LightAtten0, posWorld, normalWorld);
					#endif
				#endif

				#ifdef DYNAMICLIGHTMAP_ON
					o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif
				
				return o;
			}
			
			half4 frag(Pix i): COLOR { 
				half3 normal = normalize(i.normal);
				half3 lightDir = normalize(i.lightDir);
				
				half atten = LIGHT_ATTENUATION(i);
				
				half NL = max(0, dot(normal, lightDir));
				
				half3 light = _LightColor0.rgb * NL * atten + i.ambientOrLightmapUV;
				
				half3 diffuse = _Color*UNITY_LIGHTMODEL_AMBIENT.rgb;

				//convert light to hatch
											
				//float l = Luminance(light);
				
				//float3 c = diffuse + light;
				half3 c;
				c = _Color*UNITY_LIGHTMODEL_AMBIENT.rgb*2;
				c += i.ambientOrLightmapUV;
				c += _LightColor0.rgb * NL * (atten*2);
				
				return half4(c, 1);
			}
			
			ENDCG
		}
	}
	
	FallBack "Diffuse" 
}