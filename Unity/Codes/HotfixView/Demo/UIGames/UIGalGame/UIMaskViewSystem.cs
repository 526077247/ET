﻿using UnityEngine;
namespace ET
{
    [UISystem]
    public class UIMaskViewOnCreateSystem:OnCreateSystem<UIMaskView>
    {
        public override void OnCreate(UIMaskView self)
        {
            self.bg = self.AddUIComponent<UIImage>("Image");
            self.bg2 = self.AddUIComponent<UIImage>("Image1");
        }
    }
    
    [UISystem]
    public class UIMaskViewOnEnableSystem:OnEnableSystem<UIMaskView,string,float,bool>
    {
        public override void OnEnable(UIMaskView self, string imagePath, float time, bool isStart)
        {
            Vector2 size = new Vector2(Define.DesignScreen_Width, Define.DesignScreen_Height);
            var trans = self.bg2.GetTransform() as RectTransform;
            trans.sizeDelta = size;
            for (int i = 0; i < trans.childCount; i++)
            {
                var temp = trans.GetChild(i) as RectTransform;
                temp.sizeDelta = size;
            }
            self.StartChange(time,isStart,imagePath).Coroutine();
        }
    }
    
    public static class UIMaskViewSystem
    {
        public static async ETTask StartChange(this UIMaskView self,float interval,bool isStart,string imagePath = null)
        {
            if (GalGameEngineComponent.Instance.State == GalGameEngineComponent.GalGameEngineState.FastForward)
            {
                self.CloseSelf().Coroutine();
                return;
            }
            var hasSprite = !string.IsNullOrEmpty(imagePath);
            if (!hasSprite)
            {
                self.bg.SetEnabled(true);
                self.bg2.SetEnabled(false);
                self.bg.SetImageColor(Color.black);
                long tillTime = TimeHelper.ClientNow() + (int) (interval * 1000);
                while (TimeHelper.ClientNow() < tillTime)
                {
                    float flag = (tillTime - TimeHelper.ClientNow()) / 1000f;
                    if (isStart)
                    {
                        self.bg.SetImageAlpha(flag);
                    }
                    else
                    {
                        self.bg.SetImageAlpha(1 - flag);
                    }

                    await TimerComponent.Instance.WaitAsync(1);
                }
            }
            else
            {
                self.bg.SetEnabled(true);
                await self.bg2.SetSpritePath(imagePath);
                self.bg.SetEnabled(false);
                long tillTime = TimeHelper.ClientNow() + (int) (interval * 1000);
                var rect = self.bg2.GetTransform() as RectTransform;
                while (TimeHelper.ClientNow() < tillTime)
                {
                    float flag = (tillTime - TimeHelper.ClientNow()) / 1000f;
                    if (isStart)
                    {
                        Log.Info(flag);
                        rect.sizeDelta = new Vector2(Define.DesignScreen_Width, Define.DesignScreen_Height) * 5 * Mathf.Pow((1-flag),2);
                    }
                    else
                    {
                        Log.Info(flag);
                        rect.sizeDelta = new Vector2(Define.DesignScreen_Width, Define.DesignScreen_Height) * 5 * Mathf.Pow(flag,2);
                    }

                    await TimerComponent.Instance.WaitAsync(1);
                }
                
            }

            if(isStart) self.CloseSelf().Coroutine();
        }
    }
}