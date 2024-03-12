public class TwitchFullscreenFeature : BaseRendererFeature
{
    public override void SetBasePass()
    {
        m_RenderPass = new TwitchFullscreenPass(profilerName);
    }
}
