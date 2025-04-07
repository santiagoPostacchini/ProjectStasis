
Shader "UI/GlitchTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EffectActive ("Effect Active", Float) = 0
        _GlitchStrength ("Glitch Strength", Range(0,1)) = 0.5
        _Speed ("Speed", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _EffectActive;
            float _GlitchStrength;
            float _Speed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_EffectActive <= 0)
                    return tex2D(_MainTex, i.uv);

                float glitch = rand(float2(i.uv.y * _Speed + _Time.y, i.uv.y));
                float2 glitchUV = i.uv + float2((glitch - 0.5) * _GlitchStrength, 0);

                fixed4 col = tex2D(_MainTex, glitchUV);
                col.rgb += (glitch - 0.5) * _GlitchStrength;
                return col;
            }
            ENDCG
        }
    }
}
