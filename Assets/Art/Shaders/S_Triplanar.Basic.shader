Shader "Custom/S_Triplanar.Basic"
{
    Properties
    {
        _MainTex        ("Diffuse Texture",   2D)    = "white" {}
        _BumpMap        ("Normal Map",        2D)    = "bump"  {}
        _RoughMap       ("Roughness Map",     2D)    = "white" {}
        _Color          ("Color Multiplier",  Color) = (1,1,1,1)
        _Tile           ("Diffuse Tiling",    Float) = 1.0
        _BumpTile       ("Normal Tiling",     Float) = 1.0
        _Roughness      ("Roughness Mult",    Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }
            Cull Off
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float _Tile;
                float _BumpTile;
                float _Roughness;
                float4 _Color;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            TEXTURE2D(_RoughMap);
            SAMPLER(sampler_RoughMap);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS  = TransformObjectToHClip(IN.positionOS);
                OUT.worldPos     = TransformObjectToWorld(IN.positionOS);
                OUT.worldNormal  = normalize(TransformObjectToWorldNormal(IN.normalOS));
                return OUT;
            }

            // Retorna solo el canal .r del mapa de roughness proyectado dominado por eje
            float SampleTriplanarRoughness(float3 worldPos, float3 worldNormal)
            {
                float3 absN = abs(worldNormal);
                float2 uv;
                if (absN.x > absN.y && absN.x > absN.z)
                    uv = worldPos.yz * _Tile;
                else if (absN.y > absN.z)
                    uv = worldPos.xz * _Tile;
                else
                    uv = worldPos.xy * _Tile;

                return SAMPLE_TEXTURE2D(_RoughMap, sampler_RoughMap, uv).r;
            }

            float4 SampleTriplanarTexture(float3 worldPos, float3 worldNormal)
            {
                float3 absN = abs(worldNormal);
                if (absN.x > absN.y && absN.x > absN.z)
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, worldPos.yz * _Tile);
                else if (absN.y > absN.z)
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, worldPos.xz * _Tile);
                else
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, worldPos.xy * _Tile);
            }

            float3 SampleTriplanarNormal(float3 worldPos, float3 worldNormal)
            {
                float3 absN = abs(worldNormal);
                float2 uv;
                if (absN.x > absN.y && absN.x > absN.z)
                    uv = worldPos.yz * _BumpTile;
                else if (absN.y > absN.z)
                    uv = worldPos.xz * _BumpTile;
                else
                    uv = worldPos.xy * _BumpTile;

                return UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv));
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // Base color
                float4 color = SampleTriplanarTexture(IN.worldPos, IN.worldNormal) * _Color;

                // Roughness calculado y escalado
                float rough = SampleTriplanarRoughness(IN.worldPos, IN.worldNormal) * _Roughness;

                // Normal map (para futuras operaciones PBR)
                float3 bumpNormal = SampleTriplanarNormal(IN.worldPos, IN.worldNormal);

                // Aplicar niebla si está habilitada
                #ifdef UNITY_FOG_ENABLED
                    color.rgb = ApplyFog(color.rgb, IN.positionHCS.z);
                #endif

                // Empaquetamos roughness en el canal alfa para depuración/PBR
                color.a = rough;
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Unlit"
}
