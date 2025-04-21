Shader "Custom/GridShader"
{
    Properties
    {
        _GridColor("Grid Color", Color) = (1,1,1,1)
        _BackgroundColor("Background Color", Color) = (0,0,0,1)
        _GridSpacing("Grid Spacing", Float) = 2.0
        _LineWidth("Line Width", Float) = 2
        _EnableGrid("Enable Grid", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            fixed4 _GridColor;
            fixed4 _BackgroundColor;
            float _GridSpacing;
            float _LineWidth;
            float _EnableGrid;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if(_EnableGrid < 0.5)
                {
                    return _BackgroundColor;   
                }

                float2 gridUV = i.worldPos.xz / _GridSpacing;
                float2 grid = abs(frac(gridUV - 0.5) - 0.5) / fwidth(gridUV);

                float gridLineStrength  = min(grid.x, grid.y);
                float gridLine = smoothstep(0.0, _LineWidth, gridLineStrength);

                return lerp(_GridColor, _BackgroundColor, gridLine);
            }
            ENDCG
        }
    }
}
