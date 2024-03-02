using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/Twitch", typeof(UniversalRenderPipeline))]
public class TwitchSettings : BaseSettings
{
    public ColorParameter tintColor = new(Color.red);
    public Vector2Parameter offset = new(new Vector2(0.5f, 0.5f));
    public ClampedIntParameter echoAmount = new(1, 1, 20);
    public TextureParameter renderTexture = new(null);

    public override bool IsActive() => offset.value.x > 0f && active || offset.value.y > 0f && active || echoAmount == 1;
}