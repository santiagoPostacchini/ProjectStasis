// Shader: S_GlassPanel (URP Unlit Transparent Glass Shader)
// Descripción: Shader básico para vidrio con color y transparencia

Shader "S_GlassPanel"
{
    Properties
    {
        _GlassColor ("Glass Color", Color) = (0.6, 0.9, 1.0, 0.2)
        _Opacity ("Opacity", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _GlassColor;
            float _Opacity;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return float4(_GlassColor.rgb, _Opacity);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
