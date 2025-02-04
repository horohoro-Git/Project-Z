Shader "Custom/HologramShader"
{
   Properties
    {
        _Color ("Color", Color) = (0,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
     
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
        };

     
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Emission = _Color.rgb ;
            float nDotV = dot(o.Normal, IN.viewDir) ;
            float reverse = 1 - nDotV;
            float holo = 0.5 + 0.5 * sin(_Time.y * 3.0 + IN.worldPos.g * 0.1); 

            o.Alpha = saturate((reverse + holo) * 0.5 + 0.5);

            o.Albedo = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
