using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LBLibraryUnityFog
{
    public class FogPass : ScriptableRenderPass
    {
        private Color _fogColor;
        private float _fogDensity;
        private float _fogStart;
        private float _fogEnd;
        private float _fogDeepStart;
        private float _fogDeepEnd;

        private bool _enableFarFog, _enableDeepFog;

        private static int _fogColorID = Shader.PropertyToID("_FogColor");
        private static int _fogDensityID = Shader.PropertyToID("_FogDensity");
        private static int _fogStartID = Shader.PropertyToID("_FogStart");
        private static int _fogEndID = Shader.PropertyToID("_FogEnd");
        private static int _fogDeepStartID = Shader.PropertyToID("_FogDeepStart");
        private static int _fogDeepEndID = Shader.PropertyToID("_FogDeepEnd");
        private static int _ClipToWorldMatrix = Shader.PropertyToID("_ClipToWorldMatrix");
        private static int _CameraForward = Shader.PropertyToID("_CameraForward");
        private static int _NearPlane = Shader.PropertyToID("_NearPlane");

        private static int _CameraWorldPos = Shader.PropertyToID("_CameraWorldPos");
        
        public FogPass()
        {
        }


        const string m_ProfilerTag = "FogPass";
        Material m_BlitMaterial;

        private const string DEEP_KEY = "_ENABLE_DEEP_FOG";
        private const string FAR_KEY = "_ENALBE_FAR_FOG";
        // private const string ORTHO_CAM = "_ORTHO_CAM";

        void CreateMaterial()
        {
            if (!m_BlitMaterial)
            {
                m_BlitMaterial = new Material(Shader.Find("PostProcess/Fog"));
            }

            m_BlitMaterial.SetColor(_fogColorID, _fogColor);
            m_BlitMaterial.SetFloat(_fogDensityID, _fogDensity);
            m_BlitMaterial.SetFloat(_fogStartID, _fogStart);
            m_BlitMaterial.SetFloat(_fogEndID, _fogEnd);
            m_BlitMaterial.SetFloat(_fogDeepStartID, _fogDeepStart);
            m_BlitMaterial.SetFloat(_fogDeepEndID, _fogDeepEnd);

            if (_enableDeepFog)
            {
                m_BlitMaterial.EnableKeyword(DEEP_KEY);
            }
            else
            {
                m_BlitMaterial.DisableKeyword(DEEP_KEY);
            }

            if (_enableFarFog)
            {
                m_BlitMaterial.EnableKeyword(FAR_KEY);
            }
            else
            {
                m_BlitMaterial.DisableKeyword(FAR_KEY);
            }
        }

        /// <summary>
        /// Configure the pass
        /// </summary>
        /// <param name="baseDescriptor"></param>
        /// <param name="colorHandle"></param>
        public void Setup(Color fogColor, float fogDensity, float fogStart, float fogEnd, bool enableFarFog,
            bool enableDeepFog, float fogDeepStart, float fogDeepEnd)
        {
            _fogColor = fogColor;
            _fogDensity = fogDensity;
            _fogStart = fogStart;
            _fogEnd = fogEnd;
            _fogDeepStart = fogDeepStart;
            _fogDeepEnd = fogDeepEnd;
            _enableDeepFog = enableDeepFog;
            _enableFarFog = enableFarFog;

            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
            CreateMaterial();
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Note: We need to get the cameraData.targetTexture as this will get the targetTexture of the camera stack.
            // Overlay cameras need to output to the target described in the base camera while doing camera stack.
            ref CameraData cameraData = ref renderingData.cameraData;
            bool isSceneViewCamera = cameraData.isSceneViewCamera;
            if (isSceneViewCamera)
            {
                return;
            }

            if (m_BlitMaterial == null)
            {
                Debug.LogErrorFormat(
                    "Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.",
                    m_BlitMaterial, GetType().Name);
                return;
            }


            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);


            Camera camera = cameraData.camera;
            m_BlitMaterial.SetVector(_CameraForward, camera.transform.forward);
            Shader.SetGlobalMatrix(_ClipToWorldMatrix, camera.cameraToWorldMatrix * camera.projectionMatrix.inverse);
            m_BlitMaterial.SetFloat(_NearPlane, camera.nearClipPlane);
            Shader.SetGlobalVector(_CameraWorldPos, camera.transform.position);

            // if (camera.orthographic)
            //     m_BlitMaterial.EnableKeyword(ORTHO_CAM);
            // else
            // {
            //     m_BlitMaterial.DisableKeyword(ORTHO_CAM);
            // }

            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.SetViewport(camera.pixelRect);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_BlitMaterial);
            cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);


            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}