Shader "M8/Sprite/GradientVertical"
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
		
		[Header(Blending)]
    		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Source", Int) = 5
    		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Destination", Int) = 10

        [Header(Control)]
        _Offset ("Offset", Float) = 0
        _Scale("Scale", Float) = 1
        _ColorStart ("Color Start", Color) = (0,0,0,1)
        _ColorEnd ("Color End", Color) = (1,1,1,1)
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
        Blend [_BlendSrc] [_BlendDst]

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag_Gradient
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            float _Offset;
            float _Scale;
            fixed4 _ColorStart;
            fixed4 _ColorEnd;

            fixed4 SpriteFrag_Gradient(v2f IN) : SV_Target
            {
                float2 texUV = IN.texcoord;

                float t = (texUV.y + _Offset) * _Scale;

                fixed4 gradClr = lerp(_ColorStart, _ColorEnd, clamp(t, 0, 1));

                fixed4 c = SampleSpriteTexture (texUV) * IN.color * gradClr;
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
