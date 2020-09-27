using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomPass : ScriptableRenderPass
{
    readonly ComputeShader computeShader;
    RenderTargetIdentifier targetID;
    RenderTargetHandle tempCopy;
    RenderTargetHandle tempCopy2;
    RenderTargetHandle cameraCopy;
    RenderTextureDescriptor copySettings;
    float intensity;
    float threshold;
    //downres is fixed at .5
    string profilerTag = "BloomPass";

    public BloomPass(RenderPassEvent renderPassEvent , ComputeShader computeShader, float intensity, float threshold)
    {
        this.renderPassEvent = renderPassEvent;
        this.computeShader = computeShader;
        this.intensity = intensity;
        this.threshold = threshold;
        tempCopy.Init("tempCopy_BloomPass");
        tempCopy2.Init("tempCopy2_BloomPass");
        copySettings = new RenderTextureDescriptor();       
    }
    public void Setup(RenderTargetIdentifier cameraTarget)
    {
        targetID = cameraTarget;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor camtex)                  //called before execute. if not overriden renders to camera target?
    {
        copySettings = camtex;
        copySettings.enableRandomWrite = true;
        //copySettings.width/=2;
        //copySettings.height/=2;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        //kawase uses 1.5,2.5,2.5,3.5
        int thresholdKernel = computeShader.FindKernel("ThresholdStep");
        int KBlurKernel = computeShader.FindKernel("KBlur");
        int AddKernel = computeShader.FindKernel("Add");
        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
        cmd.GetTemporaryRT(cameraCopy.id, copySettings);                                                        //this one is full res
        copySettings.width /= 2;                                                                                //this is a struct so this should be ok and not fuck with the others
        copySettings.height /= 2;
        cmd.SetComputeFloatParam(computeShader, "intensity", intensity);
        cmd.SetComputeFloatParam(computeShader, "threshold", threshold);
        cmd.SetComputeFloatParam(computeShader, "skip", 1.5f);
        cmd.SetComputeIntParam(computeShader, "width", copySettings.width);
        cmd.SetComputeIntParam(computeShader, "height", copySettings.height);
        cmd.GetTemporaryRT(tempCopy.id, copySettings);
        cmd.GetTemporaryRT(tempCopy2.id, copySettings);
        cmd.SetComputeTextureParam(computeShader, thresholdKernel, "Source", targetID);
        cmd.SetComputeTextureParam(computeShader, thresholdKernel, "Result", tempCopy.Identifier());
        cmd.SetComputeTextureParam(computeShader, AddKernel, "Source", tempCopy.Identifier());
        cmd.SetComputeTextureParam(computeShader, AddKernel, "CameraTex", targetID);
        cmd.SetComputeTextureParam(computeShader, AddKernel, "Final", cameraCopy.Identifier());

        cmd.SetComputeTextureParam(computeShader, KBlurKernel, "Source", tempCopy.Identifier());
        cmd.SetComputeTextureParam(computeShader, KBlurKernel, "Result", tempCopy2.Identifier());

        cmd.DispatchCompute(computeShader, thresholdKernel, copySettings.width / 8, copySettings.height / 8, 1);      //threshold tempcopy
        cmd.DispatchCompute(computeShader, KBlurKernel, copySettings.width / 8, copySettings.height / 8, 1);          //kblur populates tempcopy2

        cmd.SetComputeTextureParam(computeShader, KBlurKernel, "Source", tempCopy2.Identifier());             //reverse textures and do second pass
        cmd.SetComputeTextureParam(computeShader, KBlurKernel, "Result", tempCopy.Identifier());
        cmd.SetComputeFloatParam(computeShader, "skip", 2.5f);
        cmd.DispatchCompute(computeShader, KBlurKernel, copySettings.width / 8, copySettings.height / 8, 1);

        cmd.SetComputeTextureParam(computeShader, KBlurKernel, "Source", tempCopy.Identifier());             //reverse textures and do 3rd pass
        cmd.SetComputeTextureParam(computeShader, KBlurKernel, "Result", tempCopy2.Identifier());
        cmd.DispatchCompute(computeShader, KBlurKernel, copySettings.width / 8, copySettings.height / 8, 1);

        cmd.SetComputeTextureParam(computeShader, KBlurKernel, "Source", tempCopy2.Identifier());             //reverse textures and do 4th pass
        cmd.SetComputeTextureParam(computeShader, KBlurKernel, "Result", tempCopy.Identifier());                  //textures wind up in tempCopy
        cmd.SetComputeFloatParam(computeShader, "skip", 3.5f);
        cmd.DispatchCompute(computeShader, KBlurKernel, copySettings.width / 8, copySettings.height / 8, 1);

        cmd.SetComputeIntParam(computeShader, "width", copySettings.width*2);                                   
        cmd.SetComputeIntParam(computeShader, "height", copySettings.height*2);
        cmd.DispatchCompute(computeShader, AddKernel, copySettings.width / 4, copySettings.height / 4, 1);
        //cmd.Blit(targetID, tempCopy.Identifier());
        cmd.Blit(cameraCopy.Identifier(), targetID);
        
        
        cmd.ReleaseTemporaryRT(tempCopy.id);
        cmd.ReleaseTemporaryRT(tempCopy2.id);
        cmd.ReleaseTemporaryRT(cameraCopy.id);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
        
    }
}
