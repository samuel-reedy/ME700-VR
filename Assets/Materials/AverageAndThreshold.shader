Shader "Custom/AverageAndThreshold" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Threshold("Threshold", Range(0,1)) = 0.5
        _BlurSize("Blur Size", Range(0.001, 20)) = 2
        _BackgroundColor("Background Color", Color) = (0,0,0,1)
        _ForegroundColor("Foreground Color", Color) = (1,1,1,1)
        _Speed("Speed", Range(0, 1)) = 0.1

    }

        SubShader{
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float _Threshold;
                int _BlurSize;
                float4 _BackgroundColor;
                float4 _ForegroundColor;
                float _Speed;

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float2 texelSize = 1.0 / _ScreenParams.xy;
                    float4 sum = float4(0, 0, 0, 0);

                    // Perform box blur
                    for (int x = -_BlurSize; x < _BlurSize; x++) {
                        for (int y = -_BlurSize; y < _BlurSize; y++) {
                            float2 offset = float2(x, y) * texelSize;
                            sum += tex2D(_MainTex, i.uv + offset);
                        }
                    }
                    float4 blurredColor = sum / ((_BlurSize * 2 + 1) * (_BlurSize * 2 + 1));

                    // Apply threshold
                    float grayScale = dot(blurredColor.rgb, float3(0.299, 0.587, 0.114));
                    return grayScale < _Threshold ? _BackgroundColor : _ForegroundColor;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}