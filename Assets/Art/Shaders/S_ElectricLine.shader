Shader "Custom/S_ElectricLine"
{
    Properties
    {
        _Color ("Color Principal", Color) = (1,1,1,1)
        _NoiseTex ("Mapa de Ruido", 2D) = "white" {}
        _Intensity ("Intensidad", Range(0,10)) = 1
        _Speed ("Velocidad", Float) = 1
        _Tiling ("Repetición UV", Float) = 1
        _Distortion ("Fuerza de Distorsión", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float4 _Color;
            float _Intensity;
            float _Speed;
            float _Tiling;
            float _Distortion;
            // Usamos la variable integrada _Time para la animación

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float4 col : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex) * float2(_Tiling, 1);
                o.col = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Uso de _Time.y para animar el desplazamiento del ruido
                float noise = tex2D(_NoiseTex, float2(i.uv.x + _Speed * _Time.y, i.uv.y)).r;
                float distort = (noise - 0.5) * _Distortion;
                float alpha = noise * _Intensity;
                fixed4 col = i.col;
                col.rgb += distort;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}
