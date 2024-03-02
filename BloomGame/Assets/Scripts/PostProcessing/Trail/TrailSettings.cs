using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/Trail", typeof(UniversalRenderPipeline))]
public class TrailSettings : BaseSettings
{
    public ClampedFloatParameter distance = new ClampedFloatParameter(0f, 0f, 0.5f);
    public override bool IsActive() => distance.value > 0f && active;
}