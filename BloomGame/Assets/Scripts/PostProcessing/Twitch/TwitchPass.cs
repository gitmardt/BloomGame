using UnityEngine.Rendering;
using UnityEngine;
public class TwitchPass : BasePass
{
    public TwitchPass(string name)
    {
        m_ProfilingSampler = new ProfilingSampler(name);
    }

    public override void Blit(CommandBuffer cmd)
    {
        TwitchSettings twitchSettings = m_Settings as TwitchSettings;
        if (twitchSettings == null) return;

        m_Material.SetVector("_Offset", twitchSettings.offset.value);
        m_Material.SetColor("_Color1", twitchSettings.tintColor.value);
        m_Material.SetInt("_EchoAmount", twitchSettings.echoAmount.value);
        m_Material.SetTexture("_MaskTex", twitchSettings.renderTexture.value);

        Blitter.BlitCameraTexture(cmd, m_Renderer.cameraColorTargetHandle, source, m_Material, 0);
        Blitter.BlitCameraTexture(cmd, source, m_Renderer.cameraColorTargetHandle);
    }

    public override void GetSettings() => m_Settings = GetStackComponent<TwitchSettings>();
}
