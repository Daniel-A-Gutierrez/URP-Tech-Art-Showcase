using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//franfndz

public class BlitRenderPass : ScriptableRenderPass
{
    static readonly string k_RenderTag = "Blit Stages"; // Add tag for Frame Debugger
    static readonly int TempTargetId = Shader.PropertyToID("_TempBlit");

    RenderTargetIdentifier currentTarget;
    Material blitRenderMaterial;

    public BlitRenderPass(RenderPassEvent evt, Shader shader)
    {
        renderPassEvent = evt;
        if (shader == null)
        {
            Debug.Log("No Shader");
            return;
        }

        blitRenderMaterial = CoreUtils.CreateEngineMaterial(shader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (blitRenderMaterial == null)
        {
            Debug.LogError("Material not Created");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled) return;

        CommandBuffer cmd = CommandBufferPool.Get(k_RenderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Setup(RenderTargetIdentifier currentTarget)                                             //what even calls this? its not in docs
    {
        this.currentTarget = currentTarget;
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        CameraData cameraData = renderingData.cameraData;
        RenderTargetIdentifier source = currentTarget;
        int destination = TempTargetId;
        int shaderPass = 0;

        int w = cameraData.camera.scaledPixelWidth >> 3;
        int h = cameraData.camera.scaledPixelHeight >> 3;

        cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);        //this isnt even used!
        //cmd.Blit(source, destination, blitRenderMaterial, shaderPass);
        cmd.Blit(source, source, blitRenderMaterial, shaderPass);

        cmd.ReleaseTemporaryRT(destination);
    }
}