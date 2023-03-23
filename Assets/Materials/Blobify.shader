Shader "Custom/Blobify" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _BlendRadius("Blend Radius", Range(0.0, 1.0)) = 0.1
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
                float _BlendRadius;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 center = tex2D(_MainTex, i.uv);
                    float2 pixelSize = 1.0 / _ScreenParams.xy;
                    float2 radiusPixels = _BlendRadius / pixelSize;

                    // Sum up the color and alpha values in a box around the center pixel
                    float4 sum = center;
                    for (int x = -radiusPixels.x; x <= radiusPixels.x; x++) {
                        for (int y = -radiusPixels.y; y <= radiusPixels.y; y++) {
                            float2 offset = float2(x, y) * pixelSize;
                            sum += tex2D(_MainTex, i.uv + offset);
                        }
                    }

                    // Divide by the number of pixels in the box to get the average color and alpha values
                    float numPixels = (2.0 * radiusPixels.x + 1.0) * (2.0 * radiusPixels.y + 1.0);
                    fixed4 average = sum / numPixels;

                    return average;
                }
                ENDCG
            }
        }
}