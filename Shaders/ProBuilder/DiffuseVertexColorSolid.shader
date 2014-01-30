Shader "M8/ProBuilder/Diffuse Vertex Color Solid" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
  }
  SubShader {
    Tags { "RenderType" = "Opaque" }

    ColorMask RGB

    CGPROGRAM
    #pragma surface surf Lambert addshadow fullforwardshadows

    fixed4 _Color;

    struct Input {
        float4 color : COLOR; // interpolated vertex color
        float2 uv_MainTex;
    };

    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 c = _Color * IN.color;
        o.Albedo = c.rgb;
    }
    ENDCG
  }
  Fallback "Diffuse"
}
