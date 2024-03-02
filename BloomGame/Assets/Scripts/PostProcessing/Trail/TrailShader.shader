Shader "PostProcessing/Trail"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TrailTex ("Trail Texture", 2D) = "white" {}
        _Distance ("Distance", Range(0.0, 0.5)) = 0.01
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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_TrailTex);
            SAMPLER(sampler_TrailTex);

            float _Distance;

            half4 frag (Varyings input) : SV_Target
            {
                float4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord);
                
                float4 trailCol = SAMPLE_TEXTURE2D(_TrailTex, sampler_TrailTex, input.texcoord);

                float4 finalCol = lerp(trailCol, main, _Distance);

				return finalCol;
            }
            ENDHLSL
        }
    }
}