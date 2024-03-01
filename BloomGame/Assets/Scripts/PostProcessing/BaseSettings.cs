using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public abstract class BaseSettings : VolumeComponent, IPostProcessComponent
{
    public ClampedIntParameter downSampler = new(1, 1, 10);
    public abstract bool IsActive();
    public bool IsTileCompatible() => false;
}
