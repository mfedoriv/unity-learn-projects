using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Shaders.VFX.Dithering.Ordered
{
    public class OrderedDitheringRendererPass : ScriptableRenderPass
    {
        private Material ditherMaterial;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;

        public float spread;
        public int redColorCount, greenColorCount, blueColorCount, bayerLevel;
        private RenderTextureDescriptor cameraTextureDescriptor;
        private RenderingData renderingData; // Добавляем поле для хранения renderingData

        public OrderedDitheringRendererPass(Material material)
        {
            this.ditherMaterial = material;
            tempTexture.Init("_TemporaryDitherTexture");
        }

        public void Setup(RenderTextureDescriptor cameraTextureDescriptor, int redColorCount, int greenColorCount,
            int blueColorCount, int bayerLevel, float spread, RenderingData renderingData)
        {
            this.cameraTextureDescriptor = cameraTextureDescriptor;
            this.redColorCount = redColorCount;
            this.greenColorCount = greenColorCount;
            this.blueColorCount = blueColorCount;
            this.bayerLevel = bayerLevel;
            this.spread = spread;
            this.renderingData = renderingData;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (ditherMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("DitherEffect");

            // Получаем cameraColorTarget здесь
            RenderTargetIdentifier cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
            source = cameraColorTarget;

            // Создаем временный буфер с правильным разрешением и параметрами
            cameraTextureDescriptor.depthBufferBits = 0; // Обязательно отключаем буфер глубины, если его не используем
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);

            // Настройки шейдера
            ditherMaterial.SetFloat("_Spread", spread);
            ditherMaterial.SetInt("_RedColorCount", redColorCount);
            ditherMaterial.SetInt("_GreenColorCount", greenColorCount);
            ditherMaterial.SetInt("_BlueColorCount", blueColorCount);
            ditherMaterial.SetInt("_BayerLevel", bayerLevel);

            // Применение шейдера к текстуре
            Blit(cmd, source, tempTexture.Identifier(), ditherMaterial, 0);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // Очищаем временные текстуры
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }
}