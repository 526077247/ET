using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ET
{
    [ObjectSystem]
    public class CameraManagerComponentAwakeSystem : AwakeSystem<CameraManagerComponent>
    {
        public override void Awake(CameraManagerComponent self)
        {
            self.Awake();
        }
    }
    public class CameraManagerComponent:Entity
    {
        public static CameraManagerComponent Instance;
        GameObject m_scene_main_camera_go;
        Camera m_scene_main_camera;
        public void Awake()
        {
            Instance = this;

        }
        //在场景loading开始时设置camera statck
        //loading时场景被销毁，这个时候需要将UI摄像机从overlay->base
        public void SetCameraStackAtLoadingStart()
        {
            var ui_camera = UIManagerComponent.Instance.GetUICamera();
            ui_camera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;
            ResetSceneCamera();
        }

        public void  ResetSceneCamera()
        {
            m_scene_main_camera_go = null;
            m_scene_main_camera = null;
        }
        public void SetCameraStackAtLoadingDone()
        {
            m_scene_main_camera_go = GameObject.Find("Main Camera");
            m_scene_main_camera = m_scene_main_camera_go.GetComponent<Camera>();
            var ui_camera = UIManagerComponent.Instance.GetUICamera();
            m_scene_main_camera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;
            __AddOverlayCamera(m_scene_main_camera, ui_camera);
        }


        void __AddOverlayCamera(Camera baseCamera, Camera overlayCamera)
        {
            overlayCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            baseCamera.GetUniversalAdditionalCameraData().cameraStack.Add(overlayCamera);
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            Instance = null;
        }
    }
}
