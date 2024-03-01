Shader "PostProcessing/QuantizationMask"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "QuantizationPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            uniform float4 _BlitScaleBias;

            int _NumColors;
            float _ScreenWidth;
            float _ScreenHeight;
            
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            static const int bayer2[2][2] = {
                { 0, 2 }, 
                { 3, 1 } 
            };
            static const int bayer4[4][4] = { 
                { 0, 8, 2, 10 }, 
                { 12, 4, 14, 6 }, 
                { 3, 11, 1, 9 }, 
                { 15, 7, 13, 5 } 
            };

            static const int bayer8[8][8] = {
				{  0, 32,  8, 40,  2, 34, 10, 42 },
				{ 48, 16, 56, 24, 50, 18, 58, 26 },
				{ 12, 44,  4, 36, 14, 46,  6, 38 },
				{ 60, 28, 52, 20, 62, 30, 54, 22 },
				{  3, 35, 11, 43,  1, 33,  9, 41 },
				{ 51, 19, 59, 27, 49, 17, 57, 25 },
				{ 15, 47,  7, 39, 13, 45,  5, 37 },
				{ 63, 31, 55, 23, 61, 29, 53, 21 }
			};

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

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            half4 frag (Varyings input) : SV_Target
            {
                //2X2   
                //float2 screenPos = input.texcoord * float2(_ScreenWidth, _ScreenHeight);
                //int x = int(screenPos.x) % 2;
                //int y = int(screenPos.y) % 2;
                //float ditherThreshold = bayer2[y][x] / 4.0; // Adjust for 2x2 matrix size

                //4X4
                //float2 screenPos = input.texcoord * float2(_ScreenWidth, _ScreenHeight);
                //int x = int(screenPos.x) % 4;
                //int y = int(screenPos.y) % 4;
                //float ditherThreshold = bayer4[y][x] / 16.0;

                //8X8
                float2 screenPos = input.texcoord * float2(_ScreenWidth, _ScreenHeight);
                int x = int(screenPos.x) % 8;
                int y = int(screenPos.y) % 8;
                float ditherThreshold = bayer8[y][x] / 64.0; // Adjust for 8x8 matrix size

                // Sample the noise texture
                float noiseValue = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.texcoord).r;

                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);

                float4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.texcoord);
                
                if (mask.r > 0.5)
                {
                    // Apply dithering before quantization
                    color.rgb += (noiseValue * ditherThreshold) * (1.0 / _NumColors) - (1.0 / (_NumColors * 2.0));
                
                    // Quantize color
                    color.rgb = floor(color.rgb * _NumColors) / (_NumColors - 1);
                }

                return color;
            }
            ENDHLSL
        }
    }
}