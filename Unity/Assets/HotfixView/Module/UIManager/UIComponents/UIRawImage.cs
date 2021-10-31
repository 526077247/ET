using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class UIRawImage:UIBaseContainer
    {
        string sprite_path;
        RawImage __unity_uiimage;
        RawImage unity_uiimage
        {
            get
            {
                if (__unity_uiimage == null)
                {
                    __unity_uiimage = transform.GetComponent<RawImage>();
                    if (__unity_uiimage == null)
                        Log.Error($"添加UI侧组件UIRawImage时，物体{gameObject.name}上没有找到RawImage组件");
                }
                return __unity_uiimage;
            }
        }
        public override void OnCreate()
        {
            base.OnCreate();

        }
        public override void OnCreate<T>(T t)
        {
            base.OnCreate();
            string path = t.ToString();
            SetSpritePath(path);
        }


        public async void SetSpritePath(string sprite_path)
        {
            if (string.IsNullOrEmpty(sprite_path)) return;
            if (sprite_path == this.sprite_path) return;

            var base_sprite_path = this.sprite_path;
            this.sprite_path = sprite_path;
            var sprite = await ImageLoaderComponent.Instance.LoadImageAsync(sprite_path);
            if (sprite == null)
            {
                ImageLoaderComponent.Instance.ReleaseImage(sprite_path);
                return;
            }
            
            if(!string.IsNullOrEmpty(base_sprite_path))
                ImageLoaderComponent.Instance.ReleaseImage(base_sprite_path);
            unity_uiimage.texture = sprite.texture;

        }

        public string GetSpritePath()
        {
            return sprite_path;
        }

        public void SetImageColor(Color color)
        {
            unity_uiimage.color = color;
        }

        public void SetEnabled(bool flag)
        {
            unity_uiimage.enabled = flag;
        }

        public async void SetImageGray(bool isGray)
        {
            Material mt = null;
            if (isGray)
            {
                mt = await MaterialComponent.Instance.LoadMaterialAsync("UI/UICommon/Materials/uigray.mat");
            }
            unity_uiimage.material = mt;
        }

        public override void Dispose()
        {
            base.Dispose();
            if(!string.IsNullOrEmpty(sprite_path))
                ImageLoaderComponent.Instance?.ReleaseImage(sprite_path);
            __unity_uiimage = null;
        }
    }
}
