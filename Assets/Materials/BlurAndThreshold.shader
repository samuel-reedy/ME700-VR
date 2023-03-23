Shader "Custom/BlurAndThreshold" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Range(1, 10)) = 1
        _Threshold("Threshold", Range(0, 1)) = 0.5
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _BlurSize;
                float _Threshold;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                float4 frag(v2f i) : SV_Target {
                    float2 texelSize = 1.0 / _ScreenParams.xy;
                    float2 blurDir = texelSize * _BlurSize;

                    // Sample the texture at the current pixel
                    float4 color = tex2D(_MainTex, i.uv);

                    // Blur the texture
                    color += tex2D(_MainTex, i.uv + float2(-1, -1) * blurDir) * 0.1;
                    color += tex2D(_MainTex, i.uv + float2(-1, 0) * blurDir) * 0.1;
                    color += tex2D(_MainTex, i.uv + float2(-1, 1) * blurDir) * 0.1;
                    color += tex2D(_MainTex, i.uv + float2(0, -1) * blurDir) * 0.1;
                    color += tex2D(_MainTex, i.uv + float2(0, 0) * blurDir) * 0.2;
                    color += tex2D(_MainTex, i.uv + float2(0, 1) * blurDir) * 0.1;
                    color += tex2D(_MainTex, i.uv + float2(1, -1) * blurDir) * 0.1;
                    color += tex2D(_MainTex, i.uv + float2(1, 0) * blurDir) * 0.1;
                    color += tex2D(_MainTex, i.uv + float2(1, 1) * blurDir) * 0.1;

                    // Threshold the texture
                    color = (color.r + color.g + color.b) / 3 < _Threshold ? float4(0, 0, 0, 1) : float4(1, 1, 1, 1);
                    return color;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}