Shader "M8/ProBuilder/Unlit Vertex Color Outline" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	_OutlineColor ("Outline Color", Color) = (0,1,0,1) 
    _Outline ("Outline width", Range (0.002, 0.03)) = 0.01 
  }
  SubShader {
    Tags { "RenderType" = "Opaque" }
	Lighting Off Fog { Mode Off }
    ColorMask RGB
	
	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = v.color;
			o.texcoord = v.texcoord;
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
			return col;
		}

		ENDCG
	}
	Pass {
		Name "OUTLINE" 
         Tags { "LightMode" = "Always" } 
          
         CGPROGRAM
         #pragma vertex vert
		 #pragma fragment frag_mult
		 #pragma fragmentoption ARB_precision_hint_fastest
		 #include "UnityCG.cginc"

         struct appdata { 
             float4 vertex : POSITION; 
             float3 normal : NORMAL;
         }; 

         struct v2f { 
            float4 pos : POSITION; 
            float4 color : COLOR;
         }; 
		 
         float _Outline; 
         float4 _OutlineColor; 

         v2f vert(appdata v) { 
            v2f o; 
            o.pos = UnityObjectToClipPos(v.vertex); 
            float3 norm = UnityObjectToViewPos(v.normal);
            norm.x *= UNITY_MATRIX_P[0][0]; 
            norm.y *= UNITY_MATRIX_P[1][1]; 
            o.pos.xy += norm.xy * o.pos.z * _Outline;
			o.pos.z += 0.001;
    
            o.color = _OutlineColor; 
            return o; 
         }
		 
		 fixed4 frag_mult(v2f i) : COLOR
		{
			return i.color;
		}
         ENDCG 
          
         Cull Front 
         ZWrite On 
         ColorMask RGB 
         Blend SrcAlpha OneMinusSrcAlpha 
         //? -Note: I don't remember why I put a "?" here 
         SetTexture [_MainTex] { combine primary } 
	}
  }
  Fallback "Diffuse"
}
