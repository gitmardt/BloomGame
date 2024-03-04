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
            float4 _BlitTexture_TexelSize;

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            
            TEXTURE2D(_DepthTex);
            SAMPLER(sampler_DepthTex);
            float4 _DepthTex_TexelSize;
            
            TEXTURE2D(_EnvTex);
            SAMPLER(sampler_EnvTex);

            float _Intensity;
            float4 _ShadowColor;
            float _Speed;
            float _MinDepth;
            float _MaxDepth;
            float _Range;

            half4 frag (Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);

                float strength = _Range;
                float2 uv = input.texcoord;
                float2 uv00 = uv + (float2(-_DepthTex_TexelSize.x, -_DepthTex_TexelSize.y) * strength);
                float2 uv10 = uv + (float2(0, -_DepthTex_TexelSize.y) * strength);
                float2 uv20 = uv + (float2(_DepthTex_TexelSize.x, -_DepthTex_TexelSize.y) * strength);
                float2 uv01 = uv + (float2(-_DepthTex_TexelSize.x, 0) * strength);
                float2 uv11 = uv + float2(0, 0);
                float2 uv21 = uv + (float2(_DepthTex_TexelSize.x, 0) * strength);
                float2 uv02 = uv + (float2(-_DepthTex_TexelSize.x, _DepthTex_TexelSize.y) * strength);
                float2 uv12 = uv + (float2(0, _DepthTex_TexelSize.y) * strength);
                float2 uv22 = uv + (float2(_DepthTex_TexelSize.x, _DepthTex_TexelSize.y) * strength);

                float normalDepth = SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv11).r;
                float depth = SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv00).r;
                depth += SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv10).r * 2;
                depth += SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv20).r * 1;
                depth += SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv01).r * 2;
                depth += SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv11).r * 4;
                depth += SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv21).r * 2;
                depth += SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv02).r * 1;
                depth += SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv12).r * 2;
                depth += SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv22).r * 1;

                float normalDepthEnv = SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv11).r;
                float depthEnv = SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv00).r;
                depthEnv += SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv10).r * 2;
                depthEnv += SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv20).r * 1;
                depthEnv += SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv01).r * 2;
                depthEnv += SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv11).r * 4;
                depthEnv += SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv21).r * 2;
                depthEnv += SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv02).r * 1;
                depthEnv += SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv12).r * 2;
                depthEnv += SAMPLE_TEXTURE2D(_EnvTex, sampler_EnvTex, uv22).r * 1;

                depth *= depthEnv;

                float normalizedDepth = (depth - _MinDepth) / (_MaxDepth - _MinDepth);
				float blurredDepth = (normalizedDepth / 16);

                float2 offset = _Time.x * _Speed;
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.texcoord + offset).r;

                blurredDepth *= _Intensity * noise;

                return lerp(color, _ShadowColor, blurredDepth);
            }
            ENDHLSL
        }
    }
}