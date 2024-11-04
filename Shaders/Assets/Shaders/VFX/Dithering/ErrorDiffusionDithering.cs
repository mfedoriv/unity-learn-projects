using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Этот скрипт реализует полноэкранный Error Diffusion Dithering в URP.
public class ErrorDiffusionDithering : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        // Событие рендеринга, на котором будет выполняться dithering.
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        // Материал, содержащий шейдер dithering.
        public Material ditherMaterial = null;
        // Размер матрицы диффузии (1 - без диффузии, 2 - Floyd-Steinberg, 3 - Stucki, 4 - Burkes).
        [Range(1, 4)] public int diffusionMatrixSize = 2; 
    }

    // Настройки эффекта.
    public Settings settings = new Settings();

    // Класс, реализующий сам проход рендеринга.
    class DitheringPass : ScriptableRenderPass
    {
        private Settings settings;
        // Идентификатор исходной текстуры (камеры).
        private RenderTargetIdentifier source;
        // Временная текстура для выполнения dithering.
        private RenderTargetHandle tempTexture;
        // Тег профайлера для отслеживания производительности.
        private string profilerTag;


        public DitheringPass(Settings settings, string profilerTag)
        {
            this.settings = settings;
            this.profilerTag = profilerTag;
            // Инициализируем временную текстуру.
            tempTexture.Init("_TempDitherTexture");
        }


        public void Setup(RenderTargetIdentifier source)
        {
            // Устанавливаем исходную текстуру.
            this.source = source;
        }


        // Основной метод, выполняющий dithering.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Проверяем, назначен ли материал.
            if (settings.ditherMaterial == null)
            {
                Debug.LogWarning("Dither Material is not assigned.");
                return;
            }

            // Получаем CommandBuffer для отправки команд GPU.
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            // Получаем дескриптор текстуры камеры.
            RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            // Создаем временную текстуру с теми же параметрами, что и текстура камеры.
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDesc);

            // Копируем содержимое исходной текстуры во временную.
            Blit(cmd, source, tempTexture.id);

            // Устанавливаем размер матрицы диффузии в шейдере.
            settings.ditherMaterial.SetInt("_DiffusionMatrixSize", settings.diffusionMatrixSize);
            // Выполняем dithering, применяя материал к временной текстуре и записывая результат в исходную текстуру.
            Blit(cmd, tempTexture.id, source, settings.ditherMaterial);

            // Освобождаем временную текстуру.
            cmd.ReleaseTemporaryRT(tempTexture.id);
            // Выполняем команды на GPU.
            context.ExecuteCommandBuffer(cmd);
            // Возвращаем CommandBuffer в пул.
            CommandBufferPool.Release(cmd);
        }

        // Очистка после завершения кадра.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }



    DitheringPass m_ScriptablePass;

    // Создает экземпляр DitheringPass.
    public override void Create()
    {
        m_ScriptablePass = new DitheringPass(settings, name);
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    // Добавляет проход рендеринга в очередь.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Устанавливаем исходную текстуру для прохода.
        m_ScriptablePass.Setup(renderer.cameraColorTarget);
        // Добавляем проход в очередь рендеринга.
        renderer.EnqueuePass(m_ScriptablePass);
    }
}