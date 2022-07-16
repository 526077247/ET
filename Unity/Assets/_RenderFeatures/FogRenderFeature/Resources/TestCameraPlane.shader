Shader "PostProcess/TestCameraPlane"
{
    Properties
    {
                _Range("Range", Range(-1,1)) = 0.5

    }

    SubShader
    {

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        ENDHLSL
        Blend SrcAlpha OneMinusSrcAlpha
//        Blend One Zero
//        ZTest Off        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _  _ENABLE_DEEP_FOG
            #pragma multi_compile _ _ENALBE_FAR_FOG
            #pragma multi_compile _ _ORTHO_CAM

            float4x4 _ClipToWorldMatrix;
            float _Range;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                float4 pos = mul(_ClipToWorldMatrix, float4((input.uv * 2 - 1) , _Range, 1));
            
                output.positionCS = TransformWorldToHClip(pos.xyz/pos.w);
                output.uv = input.uv;
               
                return output;
            }

            half4 frag(Varyings s):SV_Target
            {
                return float4(s.uv,0,.1);
            }
            ENDHLSL

        }

    }

}