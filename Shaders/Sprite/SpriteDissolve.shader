Shader "M8/Sprite/Dissolve"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
        _DissolveTex ("Base (RGB)", 2D) = "white" {}
        _EmissionColor("Emission Color", color) = (1,0,0,1)
        _EmissionThickness ("Emission Thickness", Range (0, 1)) = 0.1
        _DissolvePower ("Dissolve Power", Range (0, 1)) = 1
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			            
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
            sampler2D _DissolveTex;
            fixed4 _EmissionColor;
            fixed _EmissionThickness;
            fixed _DissolvePower; //start at 1 then gradually to 0

			fixed4 frag(v2f IN) : COLOR
			{
                fixed4 clr = tex2D(_MainTex, IN.texcoord) * IN.color;
                fixed mask = tex2D(_DissolveTex, IN.texcoord).r;

                fixed4 blend = fixed4(0,0,0,0);
                if (mask < _DissolvePower + _EmissionThickness)
				    blend = fixed4(_EmissionColor.r, _EmissionColor.g, _EmissionColor.b, clr.a);
			    if (mask <= _DissolvePower)
				    blend = clr;

				return blend;
			}
		ENDCG
		}
	}
}
