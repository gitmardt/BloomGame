using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/TwitchFullscreen", typeof(UniversalRenderPipeline))]
public class TwitchFullscreenSettings : BaseSettings
{
    public ColorParameter tintColor = new(Color.red);
    public Vector2Parameter offset = new(new Vector2(0.5f, 0.5f));
    public ClampedIntParameter echoAmount = new(1, 1, 20);
    public ClampedFloatParameter maskMultiplier = new(1, 0, 5);
    public TextureParameter vignetteTexture = new(null);
    public TextureParameter noiseTexture = new(null);
    public FloatParameter speed = new(1);
    public Vector2Parameter tiling = new(new Vector2(1, 1));

    public override bool IsActive() => offset.value.x > 0f && active || offset.value.y > 0f && active || echoAmount == 1;
}