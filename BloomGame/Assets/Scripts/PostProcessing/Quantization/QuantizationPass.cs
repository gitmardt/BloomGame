
using UnityEngine.Rendering;

public class QuantizationPass : BasePass
{
    public QuantizationPass(string name)
    {
        m_ProfilingSampler = new ProfilingSampler(name);
    }

    public override void Blit(CommandBuffer cmd)
    {
        QuantizationSettings quantizationSettings = m_Settings as QuantizationSettings;
        if (quantizationSettings == null) return;

        m_Material.SetInt("_NumColors", quantizationSettings.numberOfColors.value);
        m_Material.SetFloat("_ScreenWidth", m_Descriptor.width);
        m_Material.SetFloat("_ScreenHeight", m_Descriptor.height);
        m_Material.SetTexture("_NoiseTex", quantizationSettings.noiseTexture.value);

        Blitter.BlitCameraTexture(cmd, m_Renderer.cameraColorTargetHandle, source, m_Material, 0);
        Blitter.BlitCameraTexture(cmd, source, m_Renderer.cameraColorTargetHandle);
    }

    public override void GetSettings() => m_Settings = GetStackComponent<QuantizationSettings>();
}
