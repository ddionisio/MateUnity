// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//by Gabe McCauley from http://www.artisticexperiments.com/cg-shaders/cg-shaders-outline

Shader "M8/Toon/Outline"
{
	Properties {
		_outlineColor("Outline Color", Color) = (0,0,0,1)
		_outlineThickness ("Outline Thickness", Range(0.0, 0.025)) = 0.01
		_outlineThickness(" ", Float) = 0.01
		_outlineShift ("Outline Light Shift", Range(0.0, 0.025)) = 0.01
		_outlineShift(" ", Float) = 0.01
	}
	
	SubShader {
		//prepass for outline
		
		Pass {
			Name "OUTLINE"
			ZWrite On
			Tags {"Queue"="Transparent" "RenderType"="Transparent" "LightMode"="ForwardBase"}
			//ZTest Less      
			Cull Front
			
			CGPROGRAM
			#pragma vertex vShader
			#pragma fragment pShader
			#include "UnityCG.cginc"
			
			uniform fixed4 _outlineColor;
			uniform fixed _outlineThickness;
			uniform fixed _outlineShift;
			
			struct app2vert {
				float4 vertex 	: 	POSITION;
				fixed4 normal 	:	NORMAL;	
			};
			
			struct vert2Pixel {
				float4 pos 		: 	SV_POSITION;
			};
			
			vert2Pixel vShader(app2vert IN) {
				vert2Pixel OUT;
				float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
				float4x4 WorldInverseTranspose = unity_WorldToObject; 
				float4x4 World = unity_ObjectToWorld;
				
				float4 deformedPosition = mul(World, IN.vertex);
				fixed3 norm = normalize(mul(  IN.normal.xyz , WorldInverseTranspose ).xyz);	
				
				half3 pixelToLightSource =_WorldSpaceLightPos0.xyz - (deformedPosition.xyz *_WorldSpaceLightPos0.w);
				fixed3 lightDirection = normalize(-pixelToLightSource);
				//fixed diffuse = saturate(ceil(dot(IN.normal, lightDirection)));				
				
				deformedPosition.xyz += ( norm * _outlineThickness) + (lightDirection * _outlineShift);
				
				deformedPosition.xyz = mul(WorldInverseTranspose, float4 (deformedPosition.xyz, 1)).xyz;
					
				
				OUT.pos = mul(WorldViewProjection, deformedPosition);
				
				return OUT;
			}
			
			fixed4 pShader(vert2Pixel IN): COLOR {
				return _outlineColor;
			}
			ENDCG
		}
	}
}