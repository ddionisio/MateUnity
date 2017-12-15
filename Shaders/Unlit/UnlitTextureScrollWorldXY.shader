Shader "M8/Unlit/Texture Scroll World XY"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        speedX("Speed X", Float) = 1
        speedY("Speed Y", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Lighting Off Fog{ Mode Off }
        ColorMask RGB

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_vct
            #pragma fragment frag_mult
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float speedX;
            float speedY;

            struct vin_vct
            {
                float4 vertex : POSITION;
            };

            struct v2f_vct
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f_vct vert_vct(vin_vct v)
            {
                float4 wVtx = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));

                v2f_vct o;
                o.vertex = mul(UNITY_MATRIX_VP, wVtx);

                o.texcoord = TRANSFORM_TEX(wVtx.xy, _MainTex);
                o.texcoord.x += speedX * _Time.y;
                o.texcoord.y += speedY * _Time.y;

                return o;
            }

            fixed4 frag_mult(v2f_vct i) : COLOR{
                return tex2D(_MainTex, i.texcoord) * _Color;
            }

            ENDCG
        }
    }
}
