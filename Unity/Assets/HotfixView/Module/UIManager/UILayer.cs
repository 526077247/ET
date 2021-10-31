using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    //sorting layer名字配置
    public class SortingLayerNames
    {
        public static readonly string Default = "Default";
        public static readonly string Map = "Map";
        public static readonly string Scene = "Scene";
        public static readonly string Charactor = "Charactor";
        public static readonly string UI = "UI";
    }

    public class UILayerAwakeSystem : AwakeSystem<UILayer, UILayerDefine, GameObject>
    {
        public override void Awake(UILayer self, UILayerDefine layer, GameObject go)
        {
            self.Name = layer.Name;
            self.gameObject = go;
            //canvas
            if (!self.gameObject.TryGetComponent(out self.unity_canvas))
            {
                //说明：很坑爹，这里添加UI组件以后transform会Unity被替换掉，必须重新获取
                self.unity_canvas = self.gameObject.AddComponent<Canvas>();
                self.gameObject = self.unity_canvas.gameObject;
            }
            self.unity_canvas.renderMode = RenderMode.ScreenSpaceCamera;
            self.unity_canvas.worldCamera = UIManagerComponent.Instance.UICamera;
            self.unity_canvas.planeDistance = layer.PlaneDistance;
            self.unity_canvas.sortingLayerName = SortingLayerNames.UI;
            self.unity_canvas.sortingOrder = layer.OrderInLayer;

            //scaler
            if (!self.gameObject.TryGetComponent(out self.unity_canvas_scaler))
            {
                self.unity_canvas_scaler = self.gameObject.AddComponent<CanvasScaler>();
            }
            self.unity_canvas_scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            self.unity_canvas_scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            self.unity_canvas_scaler.referenceResolution = UIManagerComponent.Instance.Resolution;
            if (Screen.width / Screen.height > Define.DesignScreen_Width / Define.DesignScreen_Height)
                self.unity_canvas_scaler.matchWidthOrHeight = 1;
            else
                self.unity_canvas_scaler.matchWidthOrHeight = 0;

            //raycaster
            if (!self.gameObject.TryGetComponent(out self.unity_graphic_raycaster))
            {
                self.unity_graphic_raycaster = self.gameObject.AddComponent<GraphicRaycaster>();
            }
            // window order
            self.top_window_order = layer.OrderInLayer;
            self.min_window_order = layer.OrderInLayer;
            self.rectTransform = self.gameObject.GetComponent<RectTransform>();
        }
    }
    public class UILayer : UIBaseContainer
    {
        public string Name;
        public Canvas unity_canvas;
        public CanvasScaler unity_canvas_scaler;
        public GraphicRaycaster unity_graphic_raycaster;
        public RectTransform rectTransform;
        public int top_window_order;
        public int min_window_order;

        //设置canvas的worldCamera
        public void SetCanvasWorldCamera(Camera camera)
        {
            var old_camera = unity_canvas.worldCamera;
            if (old_camera != camera)
            {
                unity_canvas.worldCamera = camera;
            }
        }

        public int GetCanvasLayer()
        {
            return gameObject.layer;
        }

        public int PopWindowOder()
        {
            var cur = top_window_order;
            top_window_order += UIManagerComponent.Instance.MaxOderPerWindow;
            return cur;
        }

        public int PushWindowOrder()
        {
            var cur = top_window_order;
            top_window_order -= UIManagerComponent.Instance.MaxOderPerWindow;
            return cur;
        }

        public int GetMinOrderInLayer()
        {
            return min_window_order;
        }

        public void SetTopOrderInLayer(int order)
        {
            if (top_window_order < order)
            {
                top_window_order = order;
            }
        }

        public int GetTopOrderInLayer()
        {
            return top_window_order;
        }

        public Vector2 GetCanvasSize()
        {
            return rectTransform.rect.size;
        }

        /// <summary>
        /// editor调整canvas scale
        /// </summary>
        /// <param name="flag">是否竖屏</param>
        public void SetCanvasScaleEditorPortrait(bool flag)
        {
            if (flag)
            {
                unity_canvas_scaler.referenceResolution = new Vector2(Define.DesignScreen_Height, Define.DesignScreen_Width);
                unity_canvas_scaler.matchWidthOrHeight = 0;
            }
            else
            {
                unity_canvas_scaler.referenceResolution = UIManagerComponent.Instance.Resolution;
                unity_canvas_scaler.matchWidthOrHeight = 1;
            }
        }

        //public override void Dispose()
        //{
        //    if (this.IsDisposed)
        //    {
        //        return;
        //    }

        //    base.Dispose();

        //    unity_canvas = null;
        //    unity_canvas_scaler = null;
        //    unity_graphic_raycaster = null;
        //}
    }


}
