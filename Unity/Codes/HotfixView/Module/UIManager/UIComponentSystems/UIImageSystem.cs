using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [UISystem]
    public class UIImageOnCreateSystem : OnCreateSystem<UIImage,string>
    {
        public override void OnCreate(UIImage self, string path)
        {
            self.SetSpritePath(path).Coroutine();
        }
    }
    [UISystem]
    public class UIImageOnDestroySystem : OnDestroySystem<UIImage>
    {
        public override void OnDestroy(UIImage self)
        {
            if (!string.IsNullOrEmpty(self.sprite_path))
                ImageLoaderComponent.Instance?.ReleaseImage(self.sprite_path);
        }
    }
    public static class UIImageSystem
    {
        static void ActivatingComponent(this UIImage self)
        {
            if (self.unity_uiimage == null)
            {
                self.unity_uiimage = self.GetGameObject().GetComponent<Image>();
                if (self.unity_uiimage == null)
                {
                    Log.Error($"添加UI侧组件UIImage时，物体{self.GetGameObject().name}上没有找到Image组件");
                }
            }
        }
        public static async ETTask SetSpritePath(this UIImage self,string sprite_path)
        {
            if (string.IsNullOrEmpty(sprite_path)) return;
            if (sprite_path == self.sprite_path) return;
            self.ActivatingComponent();
            var base_sprite_path = self.sprite_path;
            self.sprite_path = sprite_path;
	        var sprite = await ImageLoaderComponent.Instance.LoadImageAsync(sprite_path);
            if (sprite == null)
            {
                ImageLoaderComponent.Instance.ReleaseImage(sprite_path);
                return;
            }
            
            if(!string.IsNullOrEmpty(base_sprite_path))
                ImageLoaderComponent.Instance.ReleaseImage(base_sprite_path);
            self.unity_uiimage.sprite = sprite;

        }

        public static string GetSpritePath(this UIImage self)
        {
            return self.sprite_path;
        }

        public static void SetImageColor(this UIImage self,Color color)
        {
            self.ActivatingComponent();
            self.unity_uiimage.color = color;
        }

        public static void SetEnabled(this UIImage self,bool flag)
        {
            self.ActivatingComponent();
            self.unity_uiimage.enabled = flag;
        }
        public static async void SetImageGray(this UIImage self,bool isGray)
        {
            self.ActivatingComponent();
            Material mt = null;
            if (isGray)
            {
                mt = await MaterialComponent.Instance.LoadMaterialAsync("UI/UICommon/Materials/uigray.mat");
            }
            self.unity_uiimage.material = mt;
        }
        public static void SetFillAmount(this UIImage self, float value)
        {
            self.ActivatingComponent();
            self.unity_uiimage.fillAmount = value;
        }
    }
}
