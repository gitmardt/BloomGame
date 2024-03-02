Shader "PostProcessing/Twitch"
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

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            float2 _Offset;
            float4 _Color1;
            int _EchoAmount;

            half4 frag (Varyings input) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);

                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.texcoord).r;

                if(mask > 0.1 && mask < 0.5)
                {
                    int clampedEchoAmount = clamp(_EchoAmount, 1, 20);

                    for(int i = 0; i < clampedEchoAmount; i++)
                    {
                        col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord + (_Offset / 100) * i) * _Color1;
                        col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord - (_Offset / 100) * i) * (1 - _Color1);
                    }

                    col /= _EchoAmount;
                }

                return col;
            }
            ENDHLSL
        }
    }
}