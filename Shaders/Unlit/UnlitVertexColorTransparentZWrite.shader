Shader "M8/Unlit/Vertex Color Transparent ZWrite" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		
		Pass {
            ZWrite On
            ColorMask 0
        }

        UsePass "M8/Unlit/Vertex Color Transparent/FORWARD"
	}

	FallBack "M8/Unlit/Vertex Color Transparent"
}
