using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantizationMaskRenderFeature : BaseRendererFeature
{
    public override void SetBasePass()
    {
        m_RenderPass = new QuantizationMaskPass(profilerName);
    }
}
