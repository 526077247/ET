using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ET
{
    public class UIButton : UIBaseComponent
    {
        UnityAction __onclick;
        Button __unity_uibutton;
        bool gray_state;
        Image __unity_uiimage;
        string sprite_path;

        Button unity_uibutton
        {
            get
            {
                if (__unity_uibutton == null)
                {
                    __unity_uibutton = this.gameObject.GetComponent<Button>();
                }
                return __unity_uibutton;
            }
        }
        Image unity_uiimage
        {
            get
            {
                if (__unity_uiimage == null)
                {
                    __unity_uiimage = this.gameObject.GetComponent<Image>();
                }
                return __unity_uiimage;
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
            gray_state = false;
        }

        //虚拟点击
        public void Click()
        {
            __onclick?.Invoke();
        }

        public void SetOnClick(UnityAction callback)
        {
            RemoveOnClick();
            __onclick = () =>
            {
                //AkSoundEngine.PostEvent("ConFirmation", Camera.main.gameObject);
                callback();
            };
            unity_uibutton.onClick.AddListener(__onclick);
        }

        public void RemoveOnClick()
        {
            if (__onclick != null)
                unity_uibutton.onClick.RemoveListener(__onclick);
            __onclick = null;
        }

        public void SetEnabled(bool flag)
        {
            unity_uibutton.enabled = flag;
        }

        public void SetInteractable(bool flag)
        {
            unity_uibutton.interactable = flag;
        }
        /// <summary>
        /// 设置按钮变灰
        /// </summary>
        /// <param name="isGray">是否变灰</param>
        /// <param name="includeText">是否包含字体, 不填的话默认为true</param>
        /// <param name="affectInteractable">是否影响交互, 不填的话默认为true</param>
        public async void SetBtnGray(bool isGray, bool includeText = true, bool affectInteractable = true)
        {
            if (gray_state == isGray) return;
            gray_state = isGray;
            var mat = await MaterialComponent.Instance.LoadMaterialAsync("UI/UICommon/Materials/uigray.mat");
            if (affectInteractable)
            {
                unity_uiimage.raycastTarget = !isGray;
            }
            SetBtnGray(mat, isGray, includeText);
        }

        public void SetBtnGray(Material grayMaterial, bool isGray, bool includeText)
        {
            GameObject go = gameObject;
            if (go == null)
            {
                return;
            }
            Material mt = null;
            if (isGray)
            {
                mt = grayMaterial;
            }
            var coms = go.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < coms.Length; i++)
            {
                coms[i].material = mt;
            }

            if (includeText)
            {
                var textComs = go.GetComponentsInChildren<Text>();
                for (int i = 0; i < textComs.Length; i++)
                {
                    var uITextColorCtrl = UITextColorCtrl.Get(textComs[i].gameObject);
                    if (isGray)
                    {
                        uITextColorCtrl.SetTextColor(new Color(89 / 255f, 93 / 255f, 93 / 255f));
                    }
                    else
                    {
                        uITextColorCtrl.ClearTextColor();
                    }
                }
            }
        }
        public async void SetSpritePath(string sprite_path)
        {
            if (string.IsNullOrEmpty(sprite_path)) return;
            if (sprite_path == this.sprite_path) return;

            var base_sprite_path = this.sprite_path;
            this.sprite_path = sprite_path;
            var sprite =await ImageLoaderComponent.Instance.LoadImageAsync(sprite_path);
            if (sprite == null)
            {
                ImageLoaderComponent.Instance.ReleaseImage(sprite_path);
                return;
            }

            if (!string.IsNullOrEmpty(base_sprite_path))
                ImageLoaderComponent.Instance.ReleaseImage(base_sprite_path);

            unity_uiimage.sprite = sprite;

        }

        public string GetSpritePath()
        {
            return sprite_path;
        }

        public void SetImageColor(Color color)
        {
            unity_uiimage.color = color;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (__onclick != null)
                unity_uibutton.onClick.RemoveListener(__onclick);
            if (!string.IsNullOrEmpty(sprite_path))
                ImageLoaderComponent.Instance?.ReleaseImage(sprite_path);
            __unity_uiimage = null;
            __unity_uibutton = null;
            __onclick = null;
        }
    }
}