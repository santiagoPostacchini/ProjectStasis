// Shader: S_TesseractCore (URP Unlit - Glowing Core Shader con capas y rotación interna)

Shader "S_TesseractCore"
{
    Properties
    {
        _CoreColor ("Core Color", Color) = (0.3, 0.9, 1.0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 5.0
        _GlowSpeed ("Glow Scroll Speed", Range(0, 2)) = 0.2
        _GlowScale ("Glow UV Scale", Range(0.5, 5)) = 1.0
        _FresnelFactor ("Fresnel Edge Factor", Range(0, 1)) = 0.5
        _LayerCount ("Glow Layer Count", Int) = 3
        _RotationSpeed ("Rotation Speed", Range(0, 10)) = 1.0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.1
        _ContourDarkness ("Contour Darkness", Range(0, 1)) = 0.4
        _GlowTex ("Glow Texture", 2D) = "white" {}
        _GlowMaskStrength ("Glow Mask Strength", Range(0, 5)) = 1.0
        _GlowPulseSpeed ("Glow Pulse Speed", Range(0.1, 10)) = 2.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        ZWrite Off
        Blend SrcAlpha One

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
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
            };

            float4 _CoreColor;
            float _EmissionIntensity;
            float _GlowSpeed;
            float _GlowScale;
            float _FresnelFactor;
            int _LayerCount;
            float _RotationSpeed;
            float _ContourDarkness;
            float _DistortionStrength;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            sampler2D _GlowTex;
            float4 _GlowTex_ST;
            float _GlowMaskStrength;
            float _GlowPulseSpeed;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                float angle = _Time.y * _RotationSpeed;
                float2x2 rot = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));
                float2 scaledUV = mul(IN.uv * _GlowScale, rot);
                scaledUV += float2(_Time.y * _GlowSpeed, _Time.y * _GlowSpeed * 0.5);
                OUT.uv = TRANSFORM_TEX(scaledUV, _NoiseTex);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 finalGlow = float3(0, 0, 0);

                for (int i = 0; i < _LayerCount; i++)
                {
                    float2 noiseUV = IN.uv + float2(i * 0.03, _Time.y);
                    float offsetDistort = tex2D(_NoiseTex, noiseUV).g * step(frac(noiseUV.y * 10.0), 0.1);
                    float2 layerUV = IN.uv + float2(i * 0.05, i * -0.03) + offsetDistort * _DistortionStrength;
                    float noise = tex2D(_NoiseTex, layerUV).r;

                    // Fresnel invertido
                    float fresnel = pow(1.0 - saturate(dot(IN.viewDirWS, IN.normalWS)), 3.0);

                    // Color shift con el tiempo
                    float hueShift = sin(_Time.y * (2.0 + i * 0.2)) * 0.5 + 0.5;
                    float3 shiftedColor = lerp(_CoreColor.rgb, float3(1, 1, 1), hueShift);

                    float3 glow = shiftedColor * noise * _EmissionIntensity;
                    glow *= lerp(1.0, fresnel, _FresnelFactor);

                    finalGlow += glow;
                }

                finalGlow /= max(1, _LayerCount); // Promediar capas

                // Contorno negro hacia el centro usando fresnel
                float fresnelDark = pow(1.0 - saturate(dot(IN.viewDirWS, IN.normalWS)), 3.0);
                float3 contour = float3(0, 0, 0) * fresnelDark * _ContourDarkness;

                finalGlow = lerp(contour, finalGlow, 1.0 - fresnelDark);
                float glowMask = tex2D(_GlowTex, IN.uv).r;
                float pulse = saturate(sin(_Time.y * _GlowPulseSpeed));
                finalGlow += _CoreColor.rgb * glowMask * _EmissionIntensity * _GlowMaskStrength * pulse;
                return float4(finalGlow, 1);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
