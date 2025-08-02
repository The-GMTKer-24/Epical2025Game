Shader "UI/BuildRippleTint"
{
    Properties
    {
        _TintColor("Tint Color", Color) = (0, 0.5, 1, 0.5)
        _Radius("Radius", Range(0, 2)) = 0.0
        _Feather("Feather", Range(0.01, 1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _TintColor;
            float _Radius;
            float _Feather;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float alpha = smoothstep(_Radius, _Radius - _Feather, dist);
                return fixed4(_TintColor.rgb, _TintColor.a * alpha);
            }
            ENDCG
        }
    }
}
