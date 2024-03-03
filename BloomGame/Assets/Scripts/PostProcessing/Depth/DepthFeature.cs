using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFeature : BaseRendererFeature
{
    public override void SetBasePass()
    {
        m_RenderPass = new DepthPass(profilerName);
    }
}
