Shader "PostProcessing/NoiseBloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #define E 2.71828f

        uniform float4 _BlitScaleBias;

        struct Attributes
        {
            uint vertexID : SV_VertexID;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 texcoord   : TEXCOORD0;
        };

        Varyings Vert(Attributes input)
        {
            Varyings output;

            float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
            float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);

            output.positionCS = pos;
            output.texcoord   = uv * _BlitScaleBias.xy + _BlitScaleBias.zw;
            return output;
        }

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        TEXTURE2D(_NoiseTex);
        SAMPLER(sampler_NoiseTex);

        TEXTURE2D(_MaskTex);
        SAMPLER(sampler_MaskTex);
        float2 _MaskTiling;

        TEXTURE2D(_BlitTexture);
        SAMPLER(sampler_BlitTexture);
        float4 _BlitTexture_TexelSize;

        float _Strength;
        float _Range;
        float _Threshold;
        float _ThresholdRange;
        float _Speed;
        float _MaskSpeed;
        float _MaskBlend;
        float _Saturation;

        half4 EncodeHDR(half3 color)
        {
    #if _USE_RGBM
            half4 outColor = EncodeRGBM(color);
    #else
            half4 outColor = half4(color, 1.0);
    #endif

    #if UNITY_COLORSPACE_GAMMA
            return half4(sqrt(outColor.xyz), outColor.w); // linear to γ
    #else
            return outColor;
    #endif
        }

        half3 DecodeHDR(half4 color)
        {
    #if UNITY_COLORSPACE_GAMMA
            color.xyz *= color.xyz; // γ to linear
    #endif

    #if _USE_RGBM
            return DecodeRGBM(color);
    #else
            return color.xyz;
    #endif
        }

        ENDHLSL

        Pass
        {
            Name "Blur"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag_blur

            half4 frag_blur (Varyings input) : SV_Target
            {
                float2 offset = _Time.x * _Speed;
                float strength = _Range * SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.texcoord + offset).r;

                float2 uv = input.texcoord;
                float2 uv00 = uv + (float2(-_BlitTexture_TexelSize.x, -_BlitTexture_TexelSize.y) * strength);
                float2 uv10 = uv + (float2(0, -_BlitTexture_TexelSize.y) * strength);
                float2 uv20 = uv + (float2(_BlitTexture_TexelSize.x, -_BlitTexture_TexelSize.y) * strength);
                float2 uv01 = uv + (float2(-_BlitTexture_TexelSize.x, 0) * strength);
                float2 uv11 = uv + float2(0, 0);
                float2 uv21 = uv + (float2(_BlitTexture_TexelSize.x, 0) * strength);
                float2 uv02 = uv + (float2(-_BlitTexture_TexelSize.x, _BlitTexture_TexelSize.y) * strength);
                float2 uv12 = uv + (float2(0, _BlitTexture_TexelSize.y) * strength);
                float2 uv22 = uv + (float2(_BlitTexture_TexelSize.x, _BlitTexture_TexelSize.y) * strength);

                float3 normalColor = DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv11));

                float3 color = DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv00) * 1);
                color += DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv10) * 2);
                color += DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv20) * 1);
                color += DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv01) * 2);
                color += DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv11) * 4);
                color += DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv21) * 2);
                color += DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv02) * 1);
                color += DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv12) * 2);
                color += DecodeHDR(SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv22) * 1);

				return EncodeHDR(color / 16);
            }

            ENDHLSL
        }

        Pass
        {
            Name "Bloom"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag_bloom

            half4 frag_bloom (Varyings input) : SV_Target
            {
                float3 col = DecodeHDR(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord));
                float3 brightness = max(max(col.r, col.g), col.b); // Compute the maximum brightness among RGB

                // Define a range around the threshold for smooth transition
                float lowerBound = _Threshold - _ThresholdRange;
                float upperBound = _Threshold + _ThresholdRange;

                // Use smoothstep to smoothly interpolate between 0 and 1 based on brightness
                float weight = smoothstep(lowerBound, upperBound, brightness);

                // Apply the weight to the original color
                float3 blendedCol = col * weight;

                return EncodeHDR(blendedCol * _Strength);
            }

            ENDHLSL
        }

        Pass
        {
            Name "ScreenOverlay"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment frag_screen

            half4 frag_screen (Varyings input) : SV_Target
            {
                float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord);
                float3 screen = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);

                // Multiply time by a factor to speed up the rate of change
                float fastTime = _Time.x * _MaskSpeed; // Adjust the multiplier to control the frequency

                float3 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.texcoord * _MaskTiling);
                float3 invertedMask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, float2(1 - input.texcoord.x, input.texcoord.y) * _MaskTiling);

                float3 maskedCol = float3(0,0,0);

                // Use a smaller value for more frequent toggling
                if (fmod(fastTime, 1.0) < 0.5)
                {
                    maskedCol = screen * mask;
                }
                else
                {
                    maskedCol = screen * invertedMask;
                }

                screen = lerp(screen, maskedCol, _MaskBlend);

                //col = 1 - col;
                //screen = 1 - screen;
                float3 blendedCol = col + screen;
                //col = 1 - blendedCol;

                return float4(lerp(blendedCol,col,_Saturation),1.0);
            }

            ENDHLSL
        }
    }
}