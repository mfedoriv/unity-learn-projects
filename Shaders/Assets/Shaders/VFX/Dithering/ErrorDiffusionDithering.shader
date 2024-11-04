// Шейдер для Error Diffusion Dithering.
Shader "Fullscreen/ErrorDiffusionDithering"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Исходная текстура.
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" } // Тип рендеринга.
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Структура входных данных вершинного шейдера.
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Структура выходных данных вершинного шейдера и входных данных фрагментного шейдера.
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Вершинный шейдер.
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // Преобразуем вершину в клиповое пространство.
                o.uv = v.uv; // Передаем текстурные координаты.
                return o;
            }

            sampler2D _MainTex; // Исходная текстура.
            int _DiffusionMatrixSize; // Размер матрицы диффузии.


            // Фрагментный шейдер.
            fixed4 frag (v2f i) : SV_Target
            {
                // Получаем цвет пикселя из текстуры.
                fixed4 col = tex2D(_MainTex, i.uv);

                // Вычисляем ошибку квантования (для простоты, в оттенках серого).
                float error = col.r - floor(col.r + 0.5); 
                
                // Распространяем ошибку в зависимости от выбранной матрицы.
                if (_DiffusionMatrixSize == 2) { // Floyd-Steinberg
                    // "Записываем" ошибку в соседние пиксели, изменяя значения в текстуре.
                    // Обратите внимание, что это симуляция доступа к соседним пикселям, а не настоящий доступ.
                    tex2D(_MainTex, i.uv + float2(1.0/ _ScreenParams.x, 0.0)) += error * 7.0/16.0; // Вправо
                    tex2D(_MainTex, i.uv + float2(-1.0/ _ScreenParams.x, 1.0/ _ScreenParams.y)) += error * 3.0/16.0; // Влево, вниз
                    tex2D(_MainTex, i.uv + float2(0.0, 1.0/ _ScreenParams.y)) += error * 5.0/16.0; // Вниз
                    tex2D(_MainTex, i.uv + float2(1.0/ _ScreenParams.x, 1.0/ _ScreenParams.y)) += error * 1.0/16.0; // Вправо, вниз
                }
                // Здесь можно добавить другие матрицы диффузии (Stucki, Burkes и т.д.).
                
                // Квантуем цвет до черно-белого.
                return fixed4(floor(col.rgb + 0.5), 1.0); 
            }
            ENDCG
        }
    }
}