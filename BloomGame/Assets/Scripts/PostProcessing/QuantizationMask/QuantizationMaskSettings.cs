using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/QuantizationMask", typeof(UniversalRenderPipeline))]
public class QuantizationMaskSettings : BaseSettings
{
    public IntParameter numberOfColors = new IntParameter(256);
    public TextureParameter noiseTexture = new TextureParameter(null);
    public FloatParameter envMultiplier = new FloatParameter(1);

    public override bool IsActive() => active && numberOfColors.value > 0;
}
