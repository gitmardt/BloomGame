Shader "PostProcessing/TwitchFullscreen"
{
    Properties
    {
        _BlitTexture ("Texture", 2D) = "white" {}
        _Offset ("Offset", Vector) = (0, 0, 0, 0)
        _Color1 ("Color1", Color) = (1, 1, 1, 1)
        _EchoAmount ("EchoAmount", Int) = 1
    }
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
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

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

            TEXTURE2D(_Vignette);
            SAMPLER(sampler_Vignette);
            /////////

            float2 _Offset;
            float4 _Color1;
            int _EchoAmount;
            float _MaskMultiplier;
            float _Speed;
            float2 _Tiling;

            half4 frag (Varyings input) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);

                //Masking
                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.texcoord).r;
                float envMask = SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, input.texcoord).r;
                float depth = SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, input.texcoord).r;

                float offset = _Time.x * _Speed;

                float2 noiseTexcoord = input.texcoord * _Tiling;

                noiseTexcoord.y += offset;

                float2 noiseTexcoord2 =  input.texcoord * _Tiling;
                noiseTexcoord2.y -= offset;
                noiseTexcoord2.x += 0.25;

                float2 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseTexcoord).xy;
                float2 noise2 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseTexcoord2).xy; 

                float2 noiseValue = noise * noise2;

                if(envMask < _MaskMultiplier)
                {
                    int clampedEchoAmount = clamp(_EchoAmount, 1, 20);
                    float vignetteMask = SAMPLE_TEXTURE2D(_Vignette, sampler_Vignette, input.texcoord).r;

                    _Offset.x *= (envMask * vignetteMask * noiseValue.r);
                    _Offset.y *= (envMask * vignetteMask * noiseValue.r);

                    for(int i = 0; i < clampedEchoAmount; i++)
                    {
                        col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord + (_Offset / 100) * i) * _Color1;
                        col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord - (_Offset / 100) * i) * (1 - _Color1);
                    }

                    col /= (_EchoAmount + 1);
                }

                return col;
            }
            ENDHLSL
        }
    }
}