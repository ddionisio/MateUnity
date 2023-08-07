// unlit, vertex colour only, premultiplied alpha blend multiplied by color mod
// 2D stuff
Shader "M8/2D/VertexColorAlphaModColor" 
{
	Properties 
	{
		_ColorMod("Color Mod", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		LOD 110
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			fixed4 _ColorMod;

			struct vin_vct 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color * _ColorMod;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				fixed4 col = i.color;
				return col;
			}
			
			ENDCG
		} 
	}
}
