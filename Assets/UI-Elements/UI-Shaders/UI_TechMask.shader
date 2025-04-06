Shader "UI/TechMask"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _LineColor ("Line Color", Color) = (0, 1, 1, 1)
        _Speed ("Speed", Float) = 1
        _Thickness ("Line Thickness", Float) = 0.1
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Lighting Off ZWrite Off Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MainColor;
            float4 _LineColor;
            float _Speed;
            float _Thickness;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float mask = tex2D(_MaskTex, i.uv).a;
                if (mask < 0.1) discard;

                float t = fmod(_Time.y * _Speed + i.uv.y, 1.0);
                float lineMask = smoothstep(0.0, _Thickness, abs(t - 0.5));

                fixed4 baseColor = _MainColor;
                fixed4 lineColor = _LineColor * (1.0 - lineMask);

                return baseColor + lineColor;
            }
            ENDCG
        }
    }
}
