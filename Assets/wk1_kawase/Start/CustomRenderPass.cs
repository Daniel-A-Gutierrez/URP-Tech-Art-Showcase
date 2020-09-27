//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

//class CustomRenderPass : ScriptableRenderPass
//{
//    public ComputeShader computeShader;
//    public string commandBufferName;
//    public RenderTargetIdentifier source;
//    RenderTexture target;
//    RenderTargetIdentifier targetIdentifier;

//    /*
//     This method is called by the renderer before executing the render pass. Override this method if you need to to configure render targets and their clear state,
//     and to create temporary render target textures. If a render pass doesn't override this method, this render pass renders to the active Camera's render target.
//    */
//    // args : cmd = command buffer to put stuff in, cameratexturedescriptor is the render target of the camera
//    // this implementation just makes a rendertexture target and sets up an identifier for it..
//    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
//    {
//        if (target != null)
//            target.Release();

//        target = new RenderTexture(cameraTextureDescriptor)                                                 //copies camera's render texture
//        {
//            enableRandomWrite = true
//        };
//        target.Create();

//        targetIdentifier = new RenderTargetIdentifier(target);
//    }

//    public void Setup(RenderTargetIdentifier source)
//    {
//        this.source = source;
//    }

//    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//    {
//        CommandBuffer cmd = CommandBufferPool.Get(commandBufferName);                                       //makes a new command buffer named name

//        //Setting the compute shader's texture to target
//        cmd.SetComputeTextureParam(computeShader, 0, "Result", targetIdentifier);                           //"result" is the internal name of the buffer in shader code. The shader outputs to the setup rt        

//        //Setting the StructuredBuffer<float4> _Colors
//        ComputeBuffer buffer = new ComputeBuffer(1, 4 * sizeof(float));
//        buffer.SetData(new Vector4[] { new Vector4(1, 0, 0, 1) });
//        cmd.SetComputeBufferParam(computeShader, 0, "_Colors", buffer);

//        //Setting an example float4 _Color
//        cmd.SetComputeVectorParam(computeShader, "_Color", new Vector4(1, 0, 0, 1));

//        //Dispatching compute shader
//        cmd.DispatchCompute(
//            computeShader,
//            0,
//            Mathf.CeilToInt(Screen.width / 8f),
//            Mathf.CeilToInt(Screen.height / 8f),
//            1
//        );

//        //source = new RenderTargetIdentifier(renderingData.cameraData.targetTexture);                      //i  wrote this but idk man
//        //Pasting texture on the camera's
//        cmd.Blit(targetIdentifier, source);

//        context.ExecuteCommandBuffer(cmd);
//        buffer.Release();
//        CommandBufferPool.Release(cmd);
//    }
//}

////create a compute shader that takes the scene, computes the bloom, and blits it onto the ca
