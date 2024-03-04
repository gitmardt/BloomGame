using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/DepthFog", typeof(UniversalRenderPipeline))]
public class DepthFogSettings : BaseSettings
{
    public FloatParameter intensity = new(1f);
    public Vector2Parameter valueRange = new(new UnityEngine.Vector2(0,1));
    public ClampedFloatParameter blurRange = new(0f, 0f, 50f);
    public ColorParameter shadowColor = new(UnityEngine.Color.green);
    public TextureParameter noiseTexture = new(null);
    public FloatParameter speed = new(1f);
    public override bool IsActive() => active && intensity.value > 0;
}
