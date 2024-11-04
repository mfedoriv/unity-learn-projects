using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Shaders.VFX.Dithering.Ordered
{
    public class OrderedDitheringRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class DitherSettings
        {
            public Material ditherMaterial = null;
            public int redColorCount = 4;
            public int greenColorCount = 4;
            public int blueColorCount = 4;
            public int bayerLevel = 3;
            public float spread = 0.1f;
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRendering;
        }

        public DitherSettings settings = new DitherSettings();
        private OrderedDitheringRendererPass ditherPass;

        public override void Create()
        {
            ditherPass = new OrderedDitheringRendererPass(settings.ditherMaterial)
            {
                renderPassEvent = settings.renderPassEvent
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.ditherMaterial == null) return;

            // Передаём renderingData в Setup
            ditherPass.Setup(renderingData.cameraData.cameraTargetDescriptor,
                settings.redColorCount,
                settings.greenColorCount,
                settings.blueColorCount,
                settings.bayerLevel,
                settings.spread,
                renderingData); // Передаем renderingData

            renderer.EnqueuePass(ditherPass);
        }
    }
}