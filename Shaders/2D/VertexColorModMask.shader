// unlit, vertex colour only, premultiplied alpha blend multiplied by color mod, with masking
// 2D stuff
Shader "M8/2D/VertexColorModMask"
{
	Properties 
	{
		_ColorMod("Color Mod", Color) = (1,1,1,1)
		_StencilComp("Stencil Comparison",Float) = 0 //use this for masking: 0 = disabled, 3 = equal (visible inside mask), 6 = not equal (visible outside mask)
		[HideInInspector] _RendererColor("RendererColor",Color) = (1,1,1,1)		
	}
	
	SubShader
	{
		Tags{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		Pass 
		{
			Stencil{
				Ref 1
				Comp[_StencilComp] //Disabled, Equal, NotEqual
				Pass Keep
			}
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult
			#include "UnityCG.cginc"

			fixed4 _ColorMod;
			#ifndef UNITY_INSTANCING_ENABLED
			fixed4 _RendererColor;
			#endif

			struct vin_vct 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color * _ColorMod * _RendererColor;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : SV_Target
			{
				fixed4 c = i.color;
				c.rgb *= c.a;
				return c;
			}
			
			ENDCG
		} 
	}
}
