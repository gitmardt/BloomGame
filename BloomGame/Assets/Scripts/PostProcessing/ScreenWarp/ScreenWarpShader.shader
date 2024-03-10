Shader "PostProcessing/ScreenWarpShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

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

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            //Masking 
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            TEXTURE2D(_DepthTex);
            SAMPLER(sampler_DepthTex);

            TEXTURE2D(_EnvTex);
            SAMPLER(sampler_EnvTex);
            /////////

            float _Intensity;
            float _Speed;
            float _NoiseScale;
            float2 _Tiling;
            float _MaskMultiplier;

            half4 frag (Varyings input) : SV_Target
            {
                //Masking
                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.texcoord).r;
                float envMask = SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, input.texcoord).r;
                float depth = SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, input.texcoord).r;

                if (mask > 0.2 && mask < 0.5) 
                {
                    if(envMask > depth)
                    {
                        float offset = _Time.x * _Speed;

                        float2 noiseTexcoord = input.texcoord * _Tiling;

                        noiseTexcoord.y += offset;

                        float2 noiseTexcoord2 =  input.texcoord * _Tiling;
                        noiseTexcoord2.y -= offset;
                        noiseTexcoord2.x += 0.25;

                        float2 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseTexcoord).xy;
                        float2 noise2 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseTexcoord2).xy; 

                        float2 noiseValue = noise * noise2 * (mask * _MaskMultiplier);

                        input.texcoord += (noiseValue * _NoiseScale);
                    }
                }

                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);
				return color;
            }
            ENDHLSL
        }
    }
}