using UnityEngine.Rendering;

public class DepthSettings : BaseSettings
{
    public FloatParameter intensity = new(1f);
    public override bool IsActive() => active && intensity.value > 0;
}
