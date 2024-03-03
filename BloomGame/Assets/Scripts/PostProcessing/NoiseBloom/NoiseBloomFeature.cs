using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NoiseBloomFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader m_Shader;

    private NoiseBloomPass m_NoiseBloomPass = null;
    private Material m_Material = null;
    [SerializeField] private RenderPassEvent m_RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    [SerializeField] protected bool onlyGameView = false;

    public override void Create()
    {
        m_NoiseBloomPass ??= new NoiseBloomPass();
        m_NoiseBloomPass.renderPass = m_RenderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (onlyGameView) if (renderingData.cameraData.cameraType != CameraType.Game) return;
        if (!renderingData.cameraData.postProcessEnabled) return;

        if (!GetMaterials())
        {
            Debug.LogErrorFormat("{0}.AddRenderPasses(): Missing material. {1} render pass will not be added.", GetType().Name, name);
            return;
        }

        bool shouldAdd = m_NoiseBloomPass.Setup(ref renderer, ref m_Material);
        if (shouldAdd) renderer.EnqueuePass(m_NoiseBloomPass);
    }

    private bool GetMaterials()
    {
        if (m_Material == null && m_Shader != null)
            m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        return m_Material != null;
    }

    protected override void Dispose(bool disposing)
    {
        m_NoiseBloomPass?.Dispose();
        m_NoiseBloomPass = null;
        CoreUtils.Destroy(m_Material);
    }

    private class NoiseBloomPass : ScriptableRenderPass
    {
        private Material m_Material = null;
        private ScriptableRenderer m_Renderer = null;
        private RTHandle blurTexture0, blurTexture1, blurTexture2, blurTexture3;
        private RTHandle bloomTex;
        private RTHandle blendedTex;
        private RTHandle target;
        private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("NoiseBloom");
        private NoiseBloomSettings m_Settings;
        private GraphicsFormat hdrFormat;
        public RenderPassEvent renderPass = RenderPassEvent.BeforeRenderingPostProcessing;

        internal bool Setup(ref ScriptableRenderer renderer, ref Material material)
        {
            m_Material = material;
            m_Renderer = renderer;

            renderPassEvent = renderPass;

            const FormatUsage usage = FormatUsage.Linear | FormatUsage.Render;
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, usage)) // HDR fallback
            {
                hdrFormat = GraphicsFormat.B10G11R11_UFloatPack32;
            }
            else
            {
                hdrFormat = QualitySettings.activeColorSpace == ColorSpace.Linear
                    ? GraphicsFormat.R8G8B8A8_SRGB
                    : GraphicsFormat.R8G8B8A8_UNorm;
            }

            VolumeStack stack = VolumeManager.instance.stack;
            m_Settings = stack.GetComponent<NoiseBloomSettings>();

            return m_Material != null && m_Settings.strength.value > 0f;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;

            cameraTargetDescriptor.graphicsFormat = hdrFormat;
            cameraTargetDescriptor.msaaSamples = 1;
            cameraTargetDescriptor.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref blurTexture0, cameraTargetDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_KawaseBlurTex0");

            RenderTextureDescriptor desc = cameraTargetDescriptor;

            desc.width /= 2;
            desc.height /= 2;
            RenderingUtils.ReAllocateIfNeeded(ref blurTexture1, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_KawaseBlurTex1");
            desc.width /= 2;
            desc.height /= 2;
            RenderingUtils.ReAllocateIfNeeded(ref blurTexture2, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_KawaseBlurTex2");
            desc.width /= 2;
            desc.height /= 2;
            RenderingUtils.ReAllocateIfNeeded(ref blurTexture3, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_KawaseBlurTex3");

            RenderingUtils.ReAllocateIfNeeded(ref bloomTex, cameraTargetDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_BlurTexBloom");
            RenderingUtils.ReAllocateIfNeeded(ref blendedTex, cameraTargetDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_BlurTexBlended");
            RenderingUtils.ReAllocateIfNeeded(ref target, cameraTargetDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_BlurTexTarget");

            ConfigureTarget(m_Renderer.cameraColorTargetHandle);
            ConfigureClear(ClearFlag.None, Color.white);

            //base.OnCameraSetup(cmd, ref renderingData);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_Material == null)
            {
                Debug.LogErrorFormat("{0}.Execute(): Missing material. ScreenSpaceAmbientOcclusion pass will not execute. Check for missing reference in the renderer resources.", GetType().Name);
                return;
            }

            //if not game or scene view
            if (renderingData.cameraData.cameraType != CameraType.SceneView && renderingData.cameraData.cameraType != CameraType.Game) return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                m_Material.SetFloat("_Strength", m_Settings.strength.value);
                m_Material.SetFloat("_Threshold", m_Settings.threshold.value);
                m_Material.SetFloat("_ThresholdRange", m_Settings.thresholdRange.value);
                m_Material.SetFloat("_Range", m_Settings.range.value);
                m_Material.SetTexture("_NoiseTex", m_Settings.noiseTexture.value);
                //Masking
                m_Material.SetTexture("_MaskTex", m_Settings.maskRenderTexture.value);
                m_Material.SetTexture("_DepthTex", m_Settings.depthTexture.value);
                m_Material.SetTexture("_EnvTex", m_Settings.environmentTexture.value);
                /////
                m_Material.SetVector("_MaskTiling", m_Settings.maskTiling.value);
                m_Material.SetFloat("_Speed", m_Settings.noiseSpeed.value);
                m_Material.SetFloat("_MaskSpeed", m_Settings.maskSpeed.value);
                m_Material.SetFloat("_MaskBlend", m_Settings.maskBlend.value);
                m_Material.SetFloat("_Saturation", m_Settings.fade.value);

                m_Material.SetTexture("_MainTex", m_Renderer.cameraColorTargetHandle);

                //Screen color to first blur texture
                Blitter.BlitCameraTexture(cmd, m_Renderer.cameraColorTargetHandle, bloomTex);

                Blitter.BlitCameraTexture(cmd, bloomTex, blurTexture0, m_Material, 1);

                ////Downscaled blits
                Blitter.BlitCameraTexture(cmd, blurTexture0, blurTexture1, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_Material, 0);
                Blitter.BlitCameraTexture(cmd, blurTexture1, blurTexture2, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_Material, 0);
                Blitter.BlitCameraTexture(cmd, blurTexture2, blurTexture3, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_Material, 0);

                ////Upscaled blits
                Blitter.BlitCameraTexture(cmd, blurTexture3, blurTexture2, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_Material, 0);
                Blitter.BlitCameraTexture(cmd, blurTexture2, blurTexture1, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_Material, 0);
                Blitter.BlitCameraTexture(cmd, blurTexture1, blurTexture0, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_Material, 0);

                Blitter.BlitCameraTexture(cmd, blurTexture0, blendedTex, m_Material, 2);

                Blitter.BlitCameraTexture(cmd, blendedTex, target);

                Blitter.BlitCameraTexture(cmd, target, m_Renderer.cameraColorTargetHandle);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            blurTexture0?.Release();
            blurTexture1?.Release();
            blurTexture2?.Release();
            blurTexture3?.Release();
            bloomTex?.Release();
            blendedTex?.Release();
            target?.Release();
        }
    }
}
