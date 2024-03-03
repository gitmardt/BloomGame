using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/ScreenWarp", typeof(UniversalRenderPipeline))]
public class ScreenWarpSettings : BaseSettings
{
    public ClampedFloatParameter intensity = new(1f, 0f, 20f);
    public FloatParameter speed = new(1f);
    public TextureParameter noiseTexture = new(null);
    public FloatParameter noiseScale = new(1);
    public Vector2Parameter noiseTiling = new(new Vector2(1,1));

    public override bool IsActive() => active && noiseScale.value > 0;
}
