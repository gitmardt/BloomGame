Shader "Hidden/PostProcessingExample"
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

            half4 frag (Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);
                //invert the color
                color.rgb = 1.0 - color.rgb;
				return color;
            }
            ENDHLSL
        }
    }
}