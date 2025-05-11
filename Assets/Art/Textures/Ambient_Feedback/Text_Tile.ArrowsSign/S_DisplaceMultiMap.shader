Shader "Custom/S_DisplaceMultiMap"
{
    Properties
    {
        _AlbedoMap("Albedo", 2D) = "white" {}
        _NormalMap("Normal", 2D) = "bump" {}
        _HeightMap("Height", 2D) = "black" {}
        _RoughMap("Roughness", 2D) = "gray" {}
        _ScrollDir("Scroll Direction", Vector) = (1, 0, 0, 0)
        _Speed("Scroll Speed", Float) = 1.0
        _NormalStrength("Normal Strength", Float) = 1.0
        _HeightStrength("Height Strength", Float) = 0.05
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
            };

            sampler2D _AlbedoMap;
            sampler2D _NormalMap;
            sampler2D _HeightMap;
            sampler2D _RoughMap;

            float4 _ScrollDir;
            float _Speed;
            float _NormalStrength;
            float _HeightStrength;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float3 UnpackNormalMap(float4 packedNormal)
            {
                float3 normal = packedNormal.xyz * 2.0 - 1.0;
                return normalize(normal);
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float timeOffset = _Time.y * _Speed;
                float2 offset = _ScrollDir.xy * timeOffset;
                float2 scrolledUV = IN.uv + offset;

                // Sample Textures
                float4 albedo = tex2D(_AlbedoMap, scrolledUV);
                float height = tex2D(_HeightMap, scrolledUV).r;
                float4 normalSample = tex2D(_NormalMap, scrolledUV);
                float3 normal = UnpackNormalMap(normalSample) * _NormalStrength;
                float roughness = tex2D(_RoughMap, scrolledUV).r;

                // Fake parallax displacement (UV distortion)
                float2 displacedUV = scrolledUV + normal.xy * height * _HeightStrength;
                albedo = tex2D(_AlbedoMap, displacedUV);

                // Final color
                float3 finalColor = albedo.rgb;

                return float4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
