using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/Depth", typeof(UniversalRenderPipeline))]
public class DepthSettings : BaseSettings
{
    public FloatParameter intensity = new(1f);
    public override bool IsActive() => active && intensity.value > 0;
}
