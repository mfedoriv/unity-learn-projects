Shader "Custom/OrderedDithering"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Определение текстуры
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Входные данные вершинного шейдера
            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Промежуточные данные для передачи между вершинным и фрагментным шейдером
            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Ordered Dithering Matrix (4x4)
            float ditherMatrix[4][4] = {
                { 0.0,  8.0,  2.0, 10.0 },
                { 12.0, 4.0, 14.0, 6.0 },
                { 3.0,  11.0, 1.0, 9.0 },
                { 15.0, 7.0, 13.0, 5.0 }
            };

            // Декларация текстуры и её сэмплера
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Вершинный шейдер
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.position = TransformObjectToHClip(IN.vertex); // Преобразование позиции в координаты экрана
                OUT.uv = IN.uv; // Передаем UV координаты
                return OUT;
            }

            // Функция для применения матрицы Ordered Dithering
            float ApplyDither(float2 uv, float3 color)
            {
                // Преобразуем UV в индекс матрицы 4x4
                int x = int(frac(uv.x * 4.0) * 4); // Получаем значение от 0 до 3
                int y = int(frac(uv.y * 4.0) * 4); // Получаем значение от 0 до 3

                // Достаем пороговое значение из матрицы
                float threshold = ditherMatrix[x][y] / 16.0;

                // Применяем дизеринг: если интенсивность цвета больше порога, оставляем белый пиксель, иначе черный
                return (color.r > threshold) ? 1.0 : 0.0; // Применение на канал "R" как пример
            }

            // Фрагментный шейдер
            float4 frag(Varyings IN) : SV_Target
            {
                // Получаем цвет текстуры, используя текстуру и сэмплер
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // Применяем Ordered Dithering
                float dithered = ApplyDither(IN.uv, texColor.rgb);

                // Возвращаем результат дизеринга (на каждый канал RGB)
                return float4(dithered, dithered, dithered, 1.0);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
