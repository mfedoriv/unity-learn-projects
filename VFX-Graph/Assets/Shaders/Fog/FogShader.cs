using UnityEngine;

[ExecuteInEditMode]
public class FogEffect : MonoBehaviour
{
    public Material fogMaterial; // Материал с шейдером
    public Color fogColor = Color.gray;
    public float fogDensity = 0.1f;
    public float fogOffset = 0.0f;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (fogMaterial != null)
        {
            // Передаём параметры шейдеру
            fogMaterial.SetColor("_FogColor", fogColor);
            fogMaterial.SetFloat("_FogDensity", fogDensity);
            fogMaterial.SetFloat("_FogOffset", fogOffset);

            // Применяем шейдер к картинке
            Graphics.Blit(src, dest, fogMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}