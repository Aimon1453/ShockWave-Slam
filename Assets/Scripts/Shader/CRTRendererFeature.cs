using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class CRTRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public Settings settings = new Settings();
    CRTPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CRTPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null) return;
        renderer.EnqueuePass(m_ScriptablePass);
    }

    class CRTPass : ScriptableRenderPass
    {
        private Material material;

        private class PassData
        {
            public Material material;
            public TextureHandle source;
        }

        public CRTPass(Material material)
        {
            this.material = material;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle source = resourceData.activeColorTexture;
            if (!source.IsValid()) return;

            TextureDesc desc = renderGraph.GetTextureDesc(source);
            desc.name = "CRT_Temp_Texture";
            desc.clearBuffer = false;
            desc.depthBufferBits = 0;
            TextureHandle temp = renderGraph.CreateTexture(desc);

            // Pass 1: Source -> Temp
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("CRT_Apply_Effect", out var passData))
            {
                passData.material = material;
                passData.source = source;

                builder.UseTexture(source, AccessFlags.Read);

                // 修复点：添加索引 0
                builder.SetRenderAttachment(temp, 0, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // Pass 2: Temp -> Source
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("CRT_Copy_Back", out var passData))
            {
                passData.source = temp;

                builder.UseTexture(temp, AccessFlags.Read);

                // 修复点：添加索引 0
                builder.SetRenderAttachment(source, 0, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0.0f, false);
                });
            }
        }
    }
}