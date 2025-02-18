using UnityEngine.Rendering;
using UnityEngine;
public class TwitchFullscreenPass : BasePass
{
    public TwitchFullscreenPass(string name)
    {
        m_ProfilingSampler = new ProfilingSampler(name);
    }

    public override void Blit(CommandBuffer cmd)
    {
        TwitchFullscreenSettings twitchSettings = m_Settings as TwitchFullscreenSettings;
        if (twitchSettings == null) return;

        m_Material.SetVector("_Offset", twitchSettings.offset.value);
        m_Material.SetColor("_Color1", twitchSettings.tintColor.value);
        m_Material.SetInt("_EchoAmount", twitchSettings.echoAmount.value);
        m_Material.SetFloat("_MaskMultiplier", twitchSettings.maskMultiplier.value);
        //Masking
        m_Material.SetTexture("_MaskTex", twitchSettings.maskRenderTexture.value);
        m_Material.SetTexture("_DepthTex", twitchSettings.depthTexture.value);
        m_Material.SetTexture("_EnvTex", twitchSettings.environmentTexture.value);
        m_Material.SetTexture("_Vignette", twitchSettings.vignetteTexture.value);
        /////
        m_Material.SetTexture("_NoiseTex", twitchSettings.noiseTexture.value);
        m_Material.SetFloat("_Speed", twitchSettings.speed.value);
        m_Material.SetVector("_Tiling", twitchSettings.tiling.value);

        Blitter.BlitCameraTexture(cmd, m_Renderer.cameraColorTargetHandle, source, m_Material, 0);
        Blitter.BlitCameraTexture(cmd, source, m_Renderer.cameraColorTargetHandle);
    }

    public override void GetSettings() => m_Settings = GetStackComponent<TwitchFullscreenSettings>();
}
