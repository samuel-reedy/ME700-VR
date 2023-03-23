Shader "Custom/Isosurface" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Threshold("Threshold", Range(0.0, 1.0)) = 0.5
        _BlobSize("Blob Size", Range(0.0, 1.0)) = 0.1
        _UpscaleFactor("Upscale Factor", Range(1, 8)) = 2
    }

        SubShader{
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
                float _Threshold;
                float _BlobSize;
                int _UpscaleFactor;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    // Upscale the texture by the specified factor
                    float2 texelSize = 1.0 / _ScreenParams.xy;
                    float2 uv = i.uv / _UpscaleFactor;
                    float4 color = tex2D(_MainTex, uv);
                    for (int x = 1; x < _UpscaleFactor; x++) {
                        color += tex2D(_MainTex, uv + float2(texelSize.x * x, 0));
                    }
                    color /= _UpscaleFactor;
                    for (int y = 1; y < _UpscaleFactor; y++) {
                        color += tex2D(_MainTex, uv + float2(0, texelSize.y * y));
                    }
                    color /= _UpscaleFactor;

                    // Compute the distance from the threshold value
                    float distance = abs(color.r - _Threshold);

                    // Convert the distance to an alpha value
                    float alpha = smoothstep(0.0, _BlobSize, distance);

                    // Output the color with the computed alpha value
                    return float4(color.rgb, alpha);
                }
                ENDCG
            }
        }
}