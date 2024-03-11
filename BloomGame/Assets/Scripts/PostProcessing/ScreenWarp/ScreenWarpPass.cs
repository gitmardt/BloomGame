using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenWarpPass : BasePass
{
    public ScreenWarpPass(string name)
    {
        m_ProfilingSampler = new ProfilingSampler(name);
    }

    public override void Blit(CommandBuffer cmd)
    {
        ScreenWarpSettings settings = m_Settings as ScreenWarpSettings;
        if (settings == null) return;

        m_Material.SetFloat("_Intensity", settings.intensity.value);
        m_Material.SetTexture("_NoiseTex", settings.noiseTexture.value);
        m_Material.SetFloat("_Speed", settings.speed.value);
        m_Material.SetFloat("_NoiseScale", settings.noiseScale.value);
        m_Material.SetVector("_Tiling", settings.noiseTiling.value);
        m_Material.SetFloat("_MaskMultiplier", settings.envEffectRange.value);
        //Masking
        m_Material.SetTexture("_MaskTex", settings.maskRenderTexture.value);
        m_Material.SetTexture("_DepthTex", settings.depthTexture.value);
        m_Material.SetTexture("_EnvTex", settings.environmentTexture.value);
        m_Material.SetTexture("_Vignette", settings.vignetteTexture.value);
        /////

        Blitter.BlitCameraTexture(cmd, m_Renderer.cameraColorTargetHandle, source, m_Material, 0);
        Blitter.BlitCameraTexture(cmd, source, m_Renderer.cameraColorTargetHandle);
    }

    public override void GetSettings() => m_Settings = GetStackComponent<ScreenWarpSettings>();
}
