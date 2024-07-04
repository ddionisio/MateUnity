//Only use for sprites
//Updated shader from https://github.com/Nrjwolf/unity-shader-sprite-radial-fill
Shader "M8/Sprite/ClipAngle"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

		_Angle("Angle", Range(0, 360)) = 0
        _AngleMin("Angle Min", Range(0, 360)) = 0
        _AngleMax("Angle Max", Range(0, 360)) = 0
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
        Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment SpriteFrag_Angle
			#pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
			
			float _Angle;
			float _AngleMin;
			float _AngleMax;
			float2 _UVOfs;

			fixed4 SpriteFrag_Angle(v2f IN) : COLOR
			{
				// sector start/end angles
                float startAngle = _Angle - _AngleMin;
                float endAngle = _Angle + _AngleMax;

                // check offsets
                float offset0 = clamp(0, 360, startAngle + 360);
                float offset360 = clamp(0, 360, endAngle - 360);

                // convert uv to atan coordinates
                float2 atan2Coord = float2(lerp(-1, 1, IN.texcoord.x + _UVOfs.x), lerp(-1, 1, IN.texcoord.y + _UVOfs.y));
                float atanAngle = atan2(atan2Coord.y, atan2Coord.x) * 57.3; // angle in degrees

                // convert angle to 360 system
                if(atanAngle < 0) atanAngle = 360 + atanAngle;

                if(atanAngle >= startAngle && atanAngle <= endAngle) discard;
                if(atanAngle <= offset360) discard;
                if(atanAngle >= offset0) discard;
				
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
