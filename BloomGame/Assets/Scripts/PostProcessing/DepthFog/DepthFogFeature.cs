using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFogFeature : BaseRendererFeature
{
    public override void SetBasePass()
    {
        m_RenderPass = new DepthFogPass(profilerName);
    }
}
