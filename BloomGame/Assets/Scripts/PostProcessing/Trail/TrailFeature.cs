using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TrailFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader m_Shader;
    [SerializeField] protected RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    [SerializeField] protected string profilerName = "TrailPass";

    private TrailPass m_Pass = null;
    private Material m_Material = null;

    public override void Create() => m_Pass ??= new TrailPass(profilerName);

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!GetMaterials())
        {
            Debug.LogErrorFormat("{0}.AddRenderPasses(): Missing material. {1} render pass will not be added.", GetType().Name, name);
            return;
        }

        bool shouldAdd = m_Pass.Setup(ref renderer, ref m_Material, renderPassEvent);
        if (shouldAdd) renderer.EnqueuePass(m_Pass);
    }

    private bool GetMaterials()
    {
        if (m_Material == null && m_Shader != null)
            m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        return m_Material != null;
    }

    protected override void Dispose(bool disposing)
    {
        m_Pass?.Dispose();
        m_Pass = null;
        CoreUtils.Destroy(m_Material);
    }

    public class TrailPass : ScriptableRenderPass
    {
        private Material m_Material = null;
        private ScriptableRenderer m_Renderer = null;
        private RTHandle trailTexture;
        private RTHandle trailBuffer;
        private RTHandle target;
        private ProfilingSampler m_ProfilingSampler;
        private TrailSettings m_Settings;

        public TrailPass(string profilerName)
        {
            m_ProfilingSampler = new ProfilingSampler(profilerName);
        }

        internal bool Setup(ref ScriptableRenderer renderer, ref Material material, RenderPassEvent renderPass)
        {
            m_Material = material;
            m_Renderer = renderer;

            renderPassEvent = renderPass;

            VolumeStack stack = VolumeManager.instance.stack;
            m_Settings = stack.GetComponent<TrailSettings>();

            return m_Material != null && m_Settings.IsActive();
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;

            //Downsample
            descriptor.width /= m_Settings.downSampler.value;
            descriptor.height /= m_Settings.downSampler.value;

            RenderingUtils.ReAllocateIfNeeded(ref trailTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TrailTex");
            RenderingUtils.ReAllocateIfNeeded(ref trailBuffer, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TrailBuffer");

            //Upsample
            descriptor.width *= m_Settings.downSampler.value;
            descriptor.height *= m_Settings.downSampler.value;

            RenderingUtils.ReAllocateIfNeeded(ref target, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_BlurTexTarget");

            ConfigureTarget(m_Renderer.cameraColorTargetHandle);
            ConfigureClear(ClearFlag.None, Color.white);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_Material == null)
            {
                Debug.LogErrorFormat("{0}.Execute(): Missing material. ScreenSpaceAmbientOcclusion pass will not execute. Check for missing reference in the renderer resources.", GetType().Name);
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                m_Material.SetFloat("_Distance", m_Settings.distance.value);

                m_Material.SetTexture("_MainTex", m_Renderer.cameraColorTargetHandle);
                Blitter.BlitCameraTexture(cmd, m_Renderer.cameraColorTargetHandle, trailTexture, m_Material, 0);
                Blitter.BlitCameraTexture(cmd, trailTexture, trailBuffer);
                m_Material.SetTexture("_TrailTex", trailBuffer);
                Blitter.BlitCameraTexture(cmd, trailBuffer, target);
                Blitter.BlitCameraTexture(cmd, target, m_Renderer.cameraColorTargetHandle);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            trailTexture?.Release();
            trailBuffer?.Release();
            target?.Release();
        }
    }
}
