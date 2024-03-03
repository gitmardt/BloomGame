using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public abstract class BaseSettings : VolumeComponent, IPostProcessComponent
{
    //Masking
    public TextureParameter maskRenderTexture = new TextureParameter(null);
    public TextureParameter depthTexture = new TextureParameter(null);
    public TextureParameter environmentTexture = new TextureParameter(null);
    ////////
    public ClampedIntParameter downSampler = new(1, 1, 10);
    public abstract bool IsActive();
    public bool IsTileCompatible() => false;
}
