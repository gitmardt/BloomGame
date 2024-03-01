using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantizationRenderFeature : BaseRendererFeature
{
    public override void SetBasePass()
    {
        m_RenderPass = new QuantizationPass(profilerName);
    }
}
