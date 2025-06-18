//  S_TesseractCore v5‑Mini (URP Unlit Glowing Core simplificado)

Shader "S_TesseractCore"
{
    Properties
    {
        // ====== Core / Glow ======
        _CoreColor          ("Core Color",              Color)        = (0.3, 0.9, 1, 1)
        _EmissionIntensity  ("Emission Intensity",      Range(0,10)) = 5
        _LayerCount         ("Glow Layer Count",        Range(0,8))  = 3

        // ====== Vertex‑Displacement ======
        _DisplaceStrength   ("Displace Strength",       Range(0,1))  = 0.2
        _DisplaceNoiseScale ("Displace Noise Scale",    Range(0.1,5))= 1
        _DisplaceType       ("Displace (0=Radial 1=Noise)", Int)      = 0

        // ====== Textures ======
        _NoiseTex           ("Noise Texture",           2D)          = "white" {}
        _DistortionStrength ("Distortion Strength",     Range(0,1))  = 0.1
        _DistortionScale    ("Distortion Scale",        Range(0,2))  = 1
        _GlowTex            ("Glow Texture",            2D)          = "white" {}
        _GlowMaskStrength   ("Glow Mask Strength",      Range(0,5))  = 1
        _GlowPulseSpeed     ("Glow Pulse Speed",        Range(0.1,10))=2
        _GlobalAlpha        ("Global Alpha",            Range(0,1))  = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 220
        ZWrite Off
        Blend SrcAlpha One

        Pass
        {
            Name "Unlit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ---------- structs ----------
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
            };

            // ---------- uniforms ----------
            float4  _CoreColor;
            float   _EmissionIntensity, _LayerCount;
            float   _DistortionStrength, _DistortionScale;
            float   _GlowMaskStrength, _GlowPulseSpeed, _GlobalAlpha;

            float   _DisplaceStrength, _DisplaceNoiseScale;
            int     _DisplaceType;

            sampler2D _NoiseTex; float4 _NoiseTex_ST;
            sampler2D _GlowTex;  float4 _GlowTex_ST;

            // ---------- helper ----------
            float noise2D(float2 uv) { return tex2Dlod(_NoiseTex, float4(uv,0,0)).r*2-1; }

            // ---------- vertex ----------
            Varyings vert (Attributes IN)
            {
                float3 posOS   = IN.positionOS.xyz;
                float3 normalO = IN.normalOS;

                // Vertex‑displacement
                if (_DisplaceStrength > 0)
                {
                    float3 disp = 0;
                    if (_DisplaceType == 0)          // Radial pulse
                    {
                        float radius = length(posOS);
                        float pulse  = sin((_Time.y + radius)*4)*0.5 + 0.5;
                        disp = normalO * (pulse - 0.5)*2;
                    }
                    else                             // Noise
                    {
                        float2 nUV = posOS.xz * _DisplaceNoiseScale;
                        disp = normalO * noise2D(nUV + _Time.y);
                    }
                    posOS += disp * _DisplaceStrength;
                }

                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(posOS);
                OUT.normalWS    = TransformObjectToWorldNormal(normalO);
                float3 worldPos = TransformObjectToWorld(posOS);
                OUT.viewDirWS   = normalize(_WorldSpaceCameraPos - worldPos);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _NoiseTex);
                return OUT;
            }

            // ---------- fragment ----------
            half4 frag (Varyings IN) : SV_Target
            {
                float3 finalGlow = 0;

                [loop]
                for (int i = 0; i < 8; i++)
                {
                    if (i >= _LayerCount) break;

                    float2 layerUV = IN.uv + float2(i*0.05, i*-0.03);

                    float2 noiseUV = layerUV + float2(_Time.y, 0);
                    float  offset  = tex2D(_NoiseTex, noiseUV).g
                                   * step(frac(noiseUV.y*10), 0.1)
                                   * _DistortionStrength * _DistortionScale;

                    float noise = tex2D(_NoiseTex, layerUV + offset).r;

                    float hue  = sin(_Time.y*(2+i*0.2))*0.5 + 0.5;
                    float3 col = lerp(_CoreColor.rgb, 1, hue);

                    finalGlow += col * noise * _EmissionIntensity;
                }

                finalGlow /= max(1, _LayerCount);

                // Máscara + pulso
                float glowMask = tex2D(_GlowTex, TRANSFORM_TEX(IN.uv, _GlowTex)).r;
                float pulse    = saturate(sin(_Time.y * _GlowPulseSpeed));
                finalGlow += _CoreColor.rgb * glowMask * _EmissionIntensity
                             * _GlowMaskStrength * pulse;

                return float4(finalGlow, _GlobalAlpha);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}
