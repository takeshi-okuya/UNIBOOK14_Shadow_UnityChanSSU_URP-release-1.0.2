using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomShadowMapFeature : ScriptableRendererFeature
{
    class CustomShadowMapRenderPass : ScriptableRenderPass
    {
        public override void Execute(ScriptableRenderContext context,
                                     ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("CustomShadowMap");

            foreach (var renderer in CustomShadowMapRenderer.Instances)
            {
                renderer.renderCasters(cmd);
            }

            // ViewProjectionMatrices‚ÆRenderTarget‚ðŒ³‚É–ß‚·
            var camera = renderingData.cameraData.camera;
            cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix,
                                          camera.projectionMatrix);
            cmd.SetRenderTarget(renderingData.cameraData.renderer.cameraColorTarget);

            context.ExecuteCommandBuffer(cmd);
            context.Submit();
        }
    }

    CustomShadowMapRenderPass pass;

    public override void Create()
    {
        pass = new CustomShadowMapRenderPass();
        pass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
                                         ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}