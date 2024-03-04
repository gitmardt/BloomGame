Shader "PostProcessing/DepthFog"
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
            
            TEXTURE2D(_DepthTex);
            SAMPLER(sampler_DepthTex);
            
            TEXTURE2D(_EnvTex);
            SAMPLER(sampler_EnvTex);

            float _Intensity;
            float4 _ShadowColor;

            half4 frag (Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);
                float depth = SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, input.texcoord).r * SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, input.texcoord).r;
                depth *= _Intensity;

                return lerp(color, _ShadowColor, depth);
            }
            ENDHLSL
        }
    }
}