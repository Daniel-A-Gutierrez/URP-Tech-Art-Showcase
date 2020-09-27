using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Blooom : ScriptableRendererFeature
{

    [System.Serializable]
    public struct BloomSettings
    {
        public RenderPassEvent Event;
        public ComputeShader computeShader;
        public float threshold;
        public float intensity;
    }

    public BloomSettings settings = new BloomSettings();
    BloomPass bloomPass;
    public override void Create()
    {
        bloomPass = new BloomPass(settings.Event,settings.computeShader,settings.intensity,settings.threshold);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        if (settings.computeShader == null)
        {
            Debug.LogWarningFormat("Missing compute shader. {0}  pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }

        bloomPass.Setup(renderer.cameraColorTarget);                                                //hopefully this is the right thing
        renderer.EnqueuePass(bloomPass);
    }
}
