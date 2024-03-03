using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public abstract class BasePass : ScriptableRenderPass
{
    protected ProfilingSampler m_ProfilingSampler;
    protected Material m_Material;
    internal BaseSettings m_Settings;
    protected ScriptableRenderer m_Renderer;
    protected RenderTextureDescriptor m_Descriptor;

    protected RTHandle source;

    public T GetStackComponent<T>() where T : BaseSettings
    {
        VolumeStack stack = VolumeManager.instance.stack;
        return stack.GetComponent<T>();
    }

    public abstract void GetSettings();

    public abstract void Blit(CommandBuffer cmd);

    internal bool Setup(ref ScriptableRenderer renderer, ref Material material, RenderPassEvent renderPass)
    {
        m_Material = material;
        m_Renderer = renderer;

        renderPassEvent = renderPass;

        GetSettings();

        return m_Material != null && m_Settings.IsActive();
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;

        RenderTextureDescriptor descriptor = cameraTargetDescriptor;

        m_Descriptor = descriptor;

        descriptor.msaaSamples = 1;
        descriptor.depthBufferBits = 0;

        //Downsample
        descriptor.width /= m_Settings.downSampler.value;
        descriptor.height /= m_Settings.downSampler.value;
        
        //Reallocate RTHandles
        SetupDownsampledHandles(descriptor);
        
        //Upsample
        descriptor.width *= m_Settings.downSampler.value;
        descriptor.height *= m_Settings.downSampler.value;

        RenderingUtils.ReAllocateIfNeeded(ref source, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_SourceTex");

        //Reallocate RTHandles
        SetupUpsampledHandles(descriptor);

        ConfigureTarget(m_Renderer.cameraColorTargetHandle, m_Renderer.cameraDepthTargetHandle);
        ConfigureClear(ClearFlag.None, Color.white);
    }

    public virtual void SetupDownsampledHandles(RenderTextureDescriptor descriptor) { }
    public virtual void SetupUpsampledHandles(RenderTextureDescriptor descriptor) { }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (m_Material == null)
        {
            Debug.LogErrorFormat("{0}.Execute(): Missing material. ScreenSpaceAmbientOcclusion pass will not execute. Check for missing reference in the renderer resources.", GetType().Name);
            return;
        }

        if (m_Settings == null || !m_Settings.IsActive() || m_Renderer == null) return;

        //if not game or scene view
        if (renderingData.cameraData.cameraType != CameraType.SceneView && renderingData.cameraData.cameraType != CameraType.Game) return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            Blit(cmd);
        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }

    public virtual void Dispose() 
    {
        source?.Release();
    }
}