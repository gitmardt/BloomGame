using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWarpFeature : BaseRendererFeature
{
    public override void SetBasePass()
    {
        m_RenderPass = new ScreenWarpPass(profilerName);
    }
}
