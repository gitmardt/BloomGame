using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public abstract class BaseRendererFeature : ScriptableRendererFeature
{
    [SerializeField] protected Shader m_Shader;
    [SerializeField] protected RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    [SerializeField] protected string profilerName = "CustomRenderer";

    protected Material m_Material;
    protected BasePass m_RenderPass = null;

    public abstract void SetBasePass();

    public override void Create() => SetBasePass();

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!GetMaterials())
        {
            Debug.LogErrorFormat("{0}.AddRenderPasses(): Missing material. {1} render pass will not be added.", GetType().Name, name);
            return;
        }

        bool shouldAdd = m_RenderPass.Setup(ref renderer, ref m_Material, renderPassEvent);
        if (shouldAdd) renderer.EnqueuePass(m_RenderPass);
    }

    private bool GetMaterials()
    {
        if (m_Material == null && m_Shader != null)
            m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        return m_Material != null;
    }

    protected override void Dispose(bool disposing)
    {
        m_RenderPass?.Dispose();
        m_RenderPass = null;
        CoreUtils.Destroy(m_Material);
    }
}