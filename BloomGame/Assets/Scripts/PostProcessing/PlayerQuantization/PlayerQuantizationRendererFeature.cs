using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQuantizationSettingsRendererFeature : BaseRendererFeature
{
    public override void SetBasePass()
    {
        m_RenderPass = new PlayerQuantizationPass(profilerName);
    }
}
