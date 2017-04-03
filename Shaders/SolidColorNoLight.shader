Shader "M8/SolidColorNoLight" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Lighting Off
		LOD 200
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			half4 _Color;

			struct vin_vct 
			{
				float4 vertex : POSITION;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag_mult(v2f_vct i) : COLOR
			{
				half4 col = _Color;
				return col;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}