using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/DepthFog", typeof(UniversalRenderPipeline))]
public class DepthFogSettings : BaseSettings
{
    public FloatParameter intensity = new(1f);
    public ColorParameter shadowColor = new(UnityEngine.Color.green);
    public override bool IsActive() => active && intensity.value > 0;
}
