using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/NoiseBloom", typeof(UniversalRenderPipeline))]
public class NoiseBloomSettings : BaseSettings
{
    [Tooltip("Standard deviation (spread) of the blur. Grid size is approx. 3x larger.")]
    public ClampedFloatParameter strength = new(0f, 0f, 15f);
    public ClampedFloatParameter threshold = new(0f, 0f, 1f);
    public ClampedFloatParameter thresholdRange = new(0f, 0f, 1f);
    public ClampedFloatParameter range = new(0f, 0f, 50f);
    public TextureParameter noiseTexture = new(null);
    public FloatParameter noiseSpeed = new(0f);
    public Vector2Parameter maskTiling = new(new Vector2(1f, 1f));
    public ClampedFloatParameter maskBlend = new(0.5f, 0f, 1f);
    public FloatParameter maskSpeed = new(0f);
    public ClampedFloatParameter fade = new(1f, 0f, 1f);


    public override bool IsActive() => strength.value > 0f && active;
}
