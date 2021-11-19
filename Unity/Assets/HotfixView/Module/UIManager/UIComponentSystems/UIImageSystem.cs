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

        public static async ETTask SetSpritePath(this UIImage self,string sprite_path)
        {
            if (string.IsNullOrEmpty(sprite_path)) return;
            if (sprite_path == self.sprite_path) return;

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
            self.unity_uiimage.color = color;
        }

        public static void SetEnabled(this UIImage self,bool flag)
        {
            self.unity_uiimage.enabled = flag;
        }
        public static async void SetImageGray(this UIImage self,bool isGray)
        {
            Material mt = null;
            if (isGray)
            {
                mt = await MaterialComponent.Instance.LoadMaterialAsync("UI/UICommon/Materials/uigray.mat");
            }
            self.unity_uiimage.material = mt;
        }

    }
}
