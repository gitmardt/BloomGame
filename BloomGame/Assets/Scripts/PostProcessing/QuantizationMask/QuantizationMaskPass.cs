using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class QuantizationMaskPass : BasePass
{
    public QuantizationMaskPass(string name)
    {
        m_ProfilingSampler = new ProfilingSampler(name);
    }

    public override void Blit(CommandBuffer cmd)
    {
        QuantizationMaskSettings quantizationSettings = m_Settings as QuantizationMaskSettings;
        if (quantizationSettings == null) return;

        m_Material.SetInt("_NumColors", quantizationSettings.numberOfColors.value);
        m_Material.SetFloat("_ScreenWidth", m_Descriptor.width);
        m_Material.SetFloat("_ScreenHeight", m_Descriptor.height);
        m_Material.SetTexture("_NoiseTex", quantizationSettings.noiseTexture.value);

        //Masking
        m_Material.SetTexture("_MaskTex", quantizationSettings.maskRenderTexture.value);
        m_Material.SetTexture("_DepthTex", quantizationSettings.depthTexture.value);
        m_Material.SetTexture("_EnvTex", quantizationSettings.environmentTexture.value);
        /////
        ///
        m_Material.SetFloat("_EnvMultiplier", quantizationSettings.envMultiplier.value);

        Blitter.BlitCameraTexture(cmd, m_Renderer.cameraColorTargetHandle, source, m_Material, 0);
        Blitter.BlitCameraTexture(cmd, source, m_Renderer.cameraColorTargetHandle);
    }

    public override void GetSettings() => m_Settings = GetStackComponent<QuantizationMaskSettings>();
}
