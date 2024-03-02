public class TwitchFeature : BaseRendererFeature
{
    public override void SetBasePass()
    {
        m_RenderPass = new TwitchPass(profilerName);
    }
}
