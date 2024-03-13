using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/Trail", typeof(UniversalRenderPipeline))]
public class TrailSettings : BaseSettings
{
    public FloatParameter distance = new FloatParameter(0f);
    public override bool IsActive() => distance.value > 0f && active;
}