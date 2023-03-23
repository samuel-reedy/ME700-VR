Shader "Custom/PerlinNoiseThreshold" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Threshold("Threshold", Range(0.0, 1.0)) = 0.5
        _Scale("Scale", Range(1, 10)) = 1
        _Speed("Speed", Range(-1, 1)) = 0
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float _Threshold;
                float _Scale;
                float _Speed;

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                float PerlinNoise2D(float2 p) {
                    return (1.0 + sin(dot(p, float2(27.618, 57.583))) * 43758.5453123);
                }

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float2 p = i.uv * _Scale;
                    p += _Speed * _Time.y;
                    float noise = PerlinNoise2D(p);
                    float threshold = _Threshold;
                    float4 color;
                    if (noise < threshold) {
                        color = float4(0, 0, 0, 1);
                    }
     else {
      color = float4(1, 1, 1, 1);
  }
  return color;
}
ENDCG
}
        }
            FallBack "Diffuse"
}