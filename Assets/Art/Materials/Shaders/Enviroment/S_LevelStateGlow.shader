Shader "S_LevelStateGlow"
{
    Properties
    {
        _BaseColor     ("Base Color", Color) = (0.2,0.2,0.2,1)
        [HDR]_EmissionColor ("Emission Color (HDR)", Color) = (1,0.35,0,1)
        _EmissionPower ("Emission Power", Float) = 10
        _FadeStart     ("Fade Start Y", Float) = -0.5
        _FadeEnd       ("Fade End Y",   Float) =  0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float3 positionOS : POSITION; };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
            };

            float4 _BaseColor;
            float4 _EmissionColor;
            float  _EmissionPower;
            float  _FadeStart;
            float  _FadeEnd;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.positionWS  = TransformObjectToWorld(IN.positionOS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float fade = saturate((IN.positionWS.y - _FadeStart) / (_FadeEnd - _FadeStart));
                float3 emission = _EmissionColor.rgb * _EmissionPower * fade;
                float3 finalCol = _BaseColor.rgb + emission;
                return half4(finalCol, 1);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}
