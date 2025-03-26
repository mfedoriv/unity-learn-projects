Shader "Custom/DarkWater"
{
    Properties
    {
        _WaterColor ("Water Color", Color) = (0, 0.1, 0.2, 1)
        _DepthColor ("Depth Color", Color) = (0, 0, 0, 1)
        _NormalMap1 ("Normal Map 1", 2D) = "bump" {}
        _NormalMap2 ("Normal Map 2", 2D) = "bump" {}
        _Speed ("Wave Speed", Float) = 0.5
        _Intensity ("Wave Intensity", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _NormalMap1, _NormalMap2;
            float4 _WaterColor, _DepthColor;
            float _Speed, _Intensity;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base color based on depth
                float depthFactor = saturate(i.worldPos.y * 0.1);
                fixed4 baseColor = lerp(_WaterColor, _DepthColor, depthFactor);

                // Normal maps for waves
                float2 uv1 = i.uv + float2(_Time.y * _Speed, 0);
                float2 uv2 = i.uv + float2(0, _Time.y * -_Speed);
                float3 normal1 = tex2D(_NormalMap1, uv1).rgb * 2 - 1;
                float3 normal2 = tex2D(_NormalMap2, uv2).rgb * 2 - 1;
                float3 normal = normalize(normal1 + normal2);

                // Fake reflection (optional)
                float reflection = dot(normal, float3(0, 1, 0)) * _Intensity;

                return baseColor + reflection;
            }
            ENDCG
        }
    }
}
