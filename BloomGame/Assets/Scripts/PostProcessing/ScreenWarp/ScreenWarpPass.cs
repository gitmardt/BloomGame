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
        ScreenWarpSettings screenWarpSettings = m_Settings as ScreenWarpSettings;
        if (screenWarpSettings == null) return;

        m_Material.SetFloat("_Intensity", screenWarpSettings.intensity.value);
        m_Material.SetTexture("_NoiseTex", screenWarpSettings.noiseTexture.value);
        m_Material.SetTexture("_MaskTex", screenWarpSettings.renderTexture.value);
        m_Material.SetFloat("_Speed", screenWarpSettings.speed.value);
        m_Material.SetFloat("_NoiseScale", screenWarpSettings.noiseScale.value);
        m_Material.SetVector("_Tiling", screenWarpSettings.noiseTiling.value);

        Blitter.BlitCameraTexture(cmd, m_Renderer.cameraColorTargetHandle, source, m_Material, 0);
        Blitter.BlitCameraTexture(cmd, source, m_Renderer.cameraColorTargetHandle);
    }

    public override void GetSettings() => m_Settings = GetStackComponent<ScreenWarpSettings>();
}
