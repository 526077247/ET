using System.Collections;
using System.Collections.Generic;
using LBLibraryUnityFog;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DepthFogRenderFeature : ScriptableRendererFeature
{
    public Color fogColor = Color.white;
   [Range(0,1f)] public float fogDensity = 0.5f;


    [Header("Far Fog")] public bool enableFarFog = true;
    [Range(0, 1f)] public float fogStart = 0;
    [Range(0, 1f)] public float fogEnd = 1;
    [Header("Deep Fog")] public bool enableDeepFog = true;
    public float fogDeepStart = 0;
    public float fogDeepEnd = 1;

    private FogPass _fogPass;

    public override void Create()
    {
        if (_fogPass == null)
        {
            _fogPass = new FogPass();
        }
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_fogPass != null)
        {
            _fogPass.Setup(fogColor, fogDensity, fogStart, fogEnd, enableFarFog, enableDeepFog, fogDeepStart,
                fogDeepEnd);

            renderer.EnqueuePass(_fogPass);
        }
    }
}