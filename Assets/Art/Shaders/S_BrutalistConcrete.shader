// Shader: S_BrutalistConcrete (URP Compatible - Instanced & Layered Detail Shader)
// Descripción: Shader para simular concreto brutalista con iluminación física, normal map, parallax mapping y color por altura

Shader "S_BrutalistConcrete"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.5, 0.5, 0.5, 1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _GrimeTex ("Grime Texture", 2D) = "black" {}
        _CurvatureMap ("Curvature Map", 2D) = "white" {}
        _WearMask ("Wear Mask", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _HeightMap ("Height Map (Parallax)", 2D) = "black" {}
        _Tiling ("Texture Tiling", Vector) = (1,1,0,0)
        _WearTiling ("Wear Mask Tiling", Vector) = (1,1,0,0)
        _HeightTiling ("Height Map Tiling", Vector) = (1,1,0,0)

        _GrimeAmount ("Grime Amount", Range(0,1)) = 0.5
        _EdgeSharpness ("Edge Sharpness", Range(0.01, 10)) = 1.0
        _ColorVariationIntensity ("Color Variation by Noise", Range(0, 1)) = 0.2

        _MossColor ("Moss/Humidity Color", Color) = (0.1, 0.2, 0.1, 1)
        _EnableMoss ("Enable Moss", Float) = 0
        _MossAmount ("Moss Amount", Range(0,1)) = 0.5
        _MossIntensity ("Moss Intensity", Range(0,1)) = 0.5

        _LowColor ("Color at Bottom", Color) = (0.2, 0.2, 0.2, 1)
        _HighColor ("Color at Top", Color) = (1, 1, 1, 1)
        _HeightBlendRange ("Height Blend Range", Float) = 5.0

        _Smoothness ("Smoothness", Range(0,1)) = 0.4
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _AO ("Ambient Occlusion", Range(0,1)) = 1.0
        _ParallaxStrength ("Parallax Strength", Range(0, 0.1)) = 0.02
        _UseTriplanar ("Use Triplanar", Float) = 0
        _DistanceFadeStart ("Detail Fade Start", Float) = 20.0
        _DistanceFadeEnd ("Detail Fade End", Float) = 40.0
    }

    SubShader
    {
        Tags {"RenderType"="Opaque"}
        LOD 300

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };

            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_GrimeTex); SAMPLER(sampler_GrimeTex);
            TEXTURE2D(_CurvatureMap); SAMPLER(sampler_CurvatureMap);
            TEXTURE2D(_WearMask); SAMPLER(sampler_WearMask);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURE2D(_HeightMap); SAMPLER(sampler_HeightMap);

            float4 _BaseColor;
            float4 _MossColor;
            float _EnableMoss;
            float _MossAmount;
            float _MossIntensity;
            float4 _Tiling;
            float4 _WearTiling;
            float4 _HeightTiling;

            float _GrimeAmount;
            float _EdgeSharpness;
            float _ColorVariationIntensity;

            float4 _LowColor;
            float4 _HighColor;
            float _HeightBlendRange;

            float _Smoothness;
            float _Metallic;
            float _AO;
            float _ParallaxStrength;
            float _UseTriplanar;
            float _DistanceFadeStart;
            float _DistanceFadeEnd;

            Varyings vert(Attributes IN) {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv * _Tiling.xy;
                OUT.worldPos = worldPos;
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDir = normalize(GetCameraPositionWS() - worldPos);
                return OUT;
            }

            float2 ParallaxOffset(float2 uv, float3 viewDirTS, TEXTURE2D_PARAM(heightMap, sampler_HeightMap))
            {
                float2 heightUV = uv * _HeightTiling.xy;
                float height = SAMPLE_TEXTURE2D(heightMap, sampler_HeightMap, heightUV).r;
                return heightUV + viewDirTS.xy * (height - 0.5) * _ParallaxStrength;
            }

            float3 ApplyFadeDetail(float3 baseColor, float3 worldPos)
            {
                float dist = distance(GetCameraPositionWS(), worldPos);
                float fade = saturate((_DistanceFadeEnd - dist) / (_DistanceFadeEnd - _DistanceFadeStart));
                return lerp(baseColor, float3(0.5,0.5,0.5), 1 - fade);
            }

            float3 ApplyHeightColor(float3 baseColor, float3 worldPos)
            {
                float heightFactor = saturate((worldPos.y - _LowColor.a) / _HeightBlendRange);
                return lerp(_LowColor.rgb, _HighColor.rgb, heightFactor) * baseColor;
            }

            // Utilidades necesarias para el Triplanar
float2 GetTriplanarUV(float3 worldPos, float3 normal, int axis)
{
    if (axis == 0) return worldPos.yz * _Tiling.xy;
    if (axis == 1) return worldPos.xz * _Tiling.xy;
    return worldPos.xy * _Tiling.xy;
}

float4 SampleTriplanar(TEXTURE2D_PARAM(tex, samp), float3 worldPos, float3 normal)
{
    float3 blend = pow(abs(normal), 4.0);
    blend /= (blend.x + blend.y + blend.z);

    float4 x = SAMPLE_TEXTURE2D(tex, samp, GetTriplanarUV(worldPos, normal, 0));
    float4 y = SAMPLE_TEXTURE2D(tex, samp, GetTriplanarUV(worldPos, normal, 1));
    float4 z = SAMPLE_TEXTURE2D(tex, samp, GetTriplanarUV(worldPos, normal, 2));

    return x * blend.x + y * blend.y + z * blend.z;
}

float3 SampleTriplanarNormal(TEXTURE2D_PARAM(tex, samp), float3 worldPos, float3 normal)
{
    float3 blend = pow(abs(normal), 4.0);
    blend /= (blend.x + blend.y + blend.z);

    float3 x = UnpackNormal(SAMPLE_TEXTURE2D(tex, samp, GetTriplanarUV(worldPos, normal, 0)));
    float3 y = UnpackNormal(SAMPLE_TEXTURE2D(tex, samp, GetTriplanarUV(worldPos, normal, 1)));
    float3 z = UnpackNormal(SAMPLE_TEXTURE2D(tex, samp, GetTriplanarUV(worldPos, normal, 2)));

    return normalize(x * blend.x + y * blend.y + z * blend.z);
}

half4 frag(Varyings IN) : SV_Target {
                float3 viewDir = normalize(IN.viewDir);
                float2 uv = IN.uv;
                float2 parallaxUV = ParallaxOffset(uv, viewDir, TEXTURE2D_ARGS(_HeightMap, sampler_HeightMap));
                float2 wearUV = IN.uv * _WearTiling.xy;

                float curvature = (_UseTriplanar > 0.5) ? SampleTriplanar(TEXTURE2D_ARGS(_CurvatureMap, sampler_CurvatureMap), IN.worldPos, IN.worldNormal).r : SAMPLE_TEXTURE2D(_CurvatureMap, sampler_CurvatureMap, parallaxUV).r;
                float4 noise = (_UseTriplanar > 0.5) ? SampleTriplanar(TEXTURE2D_ARGS(_NoiseTex, sampler_NoiseTex), IN.worldPos, IN.worldNormal) : SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, parallaxUV);
                float4 grime = (_UseTriplanar > 0.5) ? SampleTriplanar(TEXTURE2D_ARGS(_GrimeTex, sampler_GrimeTex), IN.worldPos, IN.worldNormal) : SAMPLE_TEXTURE2D(_GrimeTex, sampler_GrimeTex, parallaxUV);
                float4 wear = (_UseTriplanar > 0.5) ? SampleTriplanar(TEXTURE2D_ARGS(_WearMask, sampler_WearMask), IN.worldPos, IN.worldNormal) : SAMPLE_TEXTURE2D(_WearMask, sampler_WearMask, wearUV);
                float3 normalTS = (_UseTriplanar > 0.5) ? SampleTriplanarNormal(TEXTURE2D_ARGS(_NormalMap, sampler_NormalMap), IN.worldPos, IN.worldNormal) : UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, parallaxUV));

                float3 baseColor = _BaseColor.rgb + noise.r * _ColorVariationIntensity;
                float grimeFactor = pow(curvature, _EdgeSharpness) * _GrimeAmount;
                baseColor = lerp(baseColor, grime.rgb, grimeFactor);

                if (_EnableMoss > 0.5)
                {
                    float mossMask = saturate((1 - curvature) * 2);
                    mossMask *= _MossAmount;
                    baseColor = lerp(baseColor, _MossColor.rgb, mossMask * _MossIntensity);
                }

                baseColor = lerp(baseColor, grime.rgb, wear.r);
                baseColor = ApplyFadeDetail(baseColor, IN.worldPos);
                baseColor = ApplyHeightColor(baseColor, IN.worldPos);

                float3 lightDir = normalize(_MainLightPosition.xyz);
                float NdotL = max(dot(normalTS, lightDir), 0);
                float3 litColor = baseColor * NdotL;

                return float4(litColor, 1.0);
            }
            ENDHLSL
        }
    }
}
