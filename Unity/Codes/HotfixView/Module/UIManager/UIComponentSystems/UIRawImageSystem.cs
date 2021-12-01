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
    public class UIRawImageOnCreateSystem : OnCreateSystem<UIRawImage, string>
    {
        public override void OnCreate(UIRawImage self, string path)
        {
            self.SetSpritePath(path);
        }
    }
    [UISystem]
    public class UIRawImageOnDestroySystem : OnDestroySystem<UIRawImage>
    {
        public override void OnDestroy(UIRawImage self)
        {
            if (!string.IsNullOrEmpty(self.sprite_path))
                ImageLoaderComponent.Instance?.ReleaseImage(self.sprite_path);
        }
    }
    public static class UIRawImageSystem
    {
        static void ActivatingComponent(this UIRawImage self)
        {
            if (self.unity_uiimage == null)
            {
                self.unity_uiimage = self.GetGameObject().GetComponent<RawImage>();
                if (self.unity_uiimage == null)
                {
                    Log.Error($"添加UI侧组件UIRawImage时，物体{self.GetGameObject().name}上没有找到RawImage组件");
                }
            }
        }
        public static async void SetSpritePath(this UIRawImage self, string sprite_path)
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
            self.unity_uiimage.texture = sprite.texture;

        }

        public static string GetSpritePath(this UIRawImage self)
        {
            return self.sprite_path;
        }

        public static void SetImageColor(this UIRawImage self, Color color)
        {
            self.ActivatingComponent();
            self.unity_uiimage.color = color;
        }

        public static void SetEnabled(this UIRawImage self, bool flag)
        {
            self.ActivatingComponent();
            self.unity_uiimage.enabled = flag;
        }

        public static async void SetImageGray(this UIRawImage self, bool isGray)
        {
            self.ActivatingComponent();
            Material mt = null;
            if (isGray)
            {
                mt = await MaterialComponent.Instance.LoadMaterialAsync("UI/UICommon/Materials/uigray.mat");
            }
            self.unity_uiimage.material = mt;
        }

    }
}
