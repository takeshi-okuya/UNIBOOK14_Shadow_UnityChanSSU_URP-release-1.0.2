Shader "CustomShadowCaster"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float depth : TEXCOORD0;
            };

            v2f vert (appdata input)
            {
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                v2f output;
                output.positionCS = vertexInput.positionCS;
                output.depth = -vertexInput.positionVS.z;

                return output;
            }

            float frag(v2f input) : SV_Target
            {
                return input.depth;
            }
            ENDHLSL
        }
    }
}