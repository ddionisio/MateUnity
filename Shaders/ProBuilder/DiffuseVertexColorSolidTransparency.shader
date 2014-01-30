Shader "M8/ProBuilder/Diffuse Vertex Color Solid Transparency" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
  }
  SubShader {
    Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}

    //ZWrite Off // on might hide behind pixels, off might miss order
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask RGB

    CGPROGRAM
    #pragma surface surf Lambert

    fixed4 _Color;

    struct Input {
        float4 color : COLOR; // interpolated vertex color
        float2 uv_MainTex;
    };

    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 c = _Color * IN.color;
        o.Albedo = c.rgb;
        o.Alpha = c.a;
    }
    ENDCG
  }
  Fallback "Transparent/Diffuse"
}
