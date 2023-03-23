Shader "Custom/SmoothBlendFast" {
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
                float4 _MainTex_TexelSize;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 center = tex2D(_MainTex, i.uv);

                // Compute the size of the box filter based on the blend radius
                float2 radiusPixels = _BlendRadius / _MainTex_TexelSize.xy;
                int numPixels = int((2.0 * radiusPixels.x + 1.0) * (2.0 * radiusPixels.y + 1.0));

                // Sum up the color and alpha values in a box around the center pixel
                float4 sum = center;
                for (int x = -int(radiusPixels.x); x <= int(radiusPixels.x); x++) {
                    for (int y = -int(radiusPixels.y); y <= int(radiusPixels.y); y++) {
                        float2 offset = float2(x, y) * _MainTex_TexelSize.xy;
                        sum += tex2D(_MainTex, i.uv + offset);
                    }
                }

                // Divide by the number of pixels in the box to get the average color and alpha values
                fixed4 average = sum / numPixels;

                return average;
            }
            ENDCG
        }
        }
}