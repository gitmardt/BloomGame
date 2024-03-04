using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DepthFogPass : BasePass
{
    public DepthFogPass(string name)
    {
        m_ProfilingSampler = new ProfilingSampler(name);
    }

    public override void Blit(CommandBuffer cmd)
    {
        DepthFogSettings settings = m_Settings as DepthFogSettings;
        if (settings == null) return;

        m_Material.SetFloat("_Intensity", settings.intensity.value);
        m_Material.SetTexture("_EnvTex", settings.environmentTexture.value);
        m_Material.SetTexture("_DepthTex", settings.depthTexture.value);
        m_Material.SetColor("_ShadowColor", settings.shadowColor.value);

        Blitter.BlitCameraTexture(cmd, m_Renderer.cameraColorTargetHandle, source, m_Material, 0);
        Blitter.BlitCameraTexture(cmd, source, m_Renderer.cameraColorTargetHandle);
    }

    public override void GetSettings() => m_Settings = GetStackComponent<DepthFogSettings>();
}
