using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using AssetBundles;
using System.Linq;

namespace ET
{
    [UISystem]
    [FriendClass(typeof(UIUpdateView))]
    public class UIUpdateViewOnCreateSystem : OnCreateSystem<UIUpdateView>
    {
        public override void OnCreate(UIUpdateView self)
        {
            self.m_slider = self.AddUIComponent<UISlider>("Loadingscreen/Slider");
        }
    }
    [UISystem]
    [FriendClass(typeof(UIUpdateView))]
    public class UIUpdateViewOnEnableSystem : OnEnableSystem<UIUpdateView,Action>
    {
        public override void OnEnable(UIUpdateView self,Action func)
        {
            self.force_update = Define.ForceUpdate;
            self.OnOver = func;
            self.last_progress = 0;
            self.m_slider.SetValue(0);
            self.StartCheckUpdate().Coroutine();
        }
    }
    [FriendClass(typeof(UIUpdateView))]
    public static class UIUpdateViewSystem
    {

        private static async ETTask UpdateFinishAndStartGame(this UIUpdateView self)
        {
            Game.Scene.RemoveAllComponent();
            // 重启资源管理器
            while (AddressablesManager.Instance.IsProsessRunning)
            {
                await TimerComponent.Instance.WaitAsync(1);
            }
            AddressablesManager.Instance.ClearAssetsCache();
            AddressablesManager.Instance.ClearConfigCache();
            //重新加载配置
            AssetBundleConfig.Instance.SyncLoadGlobalAssetBundle();

            //热修复
            AddressablesManager.Instance.StartInjectFix();
            CodeLoader.Instance.ReStart();
        }

        async static ETTask<int> ShowMsgBoxView(this UIUpdateView self,string content, string confirmBtnText, string cancelBtnText)
        {
            ETTask<int> tcs = ETTask<int>.Create();
            Action confirmBtnFunc = () =>
             {
                 tcs.SetResult(self.BTN_CONFIRM);
             };

            Action cancelBtnFunc = () =>
            {
                tcs.SetResult(self.BTN_CANCEL);
            };
            I18NComponent.Instance.I18NTryGetText(content, out self.para.Content);
            I18NComponent.Instance.I18NTryGetText(confirmBtnText, out self.para.ConfirmText);
            I18NComponent.Instance.I18NTryGetText(cancelBtnText, out self.para.CancelText);
            self.para.ConfirmCallback = confirmBtnFunc;
            self.para.CancelCallback = cancelBtnFunc;
            await UIManagerComponent.Instance.OpenWindow<UIMsgBoxWin, UIMsgBoxWin.MsgBoxPara>(UIMsgBoxWin.PrefabPath,
                self.para,UILayerNames.TipLayer);
            var result = await tcs;
            await UIManagerComponent.Instance.CloseWindow<UIMsgBoxWin>();
            return result;
        }
        public static async ETTask StartCheckUpdate(this UIUpdateView self)
        {
            //TODO 网络检查 
            await self.CheckIsInWhiteList();

            await self.CheckUpdateList();

            var Over = await self.CheckAppUpdate();
            if (Over) return;

            var isUpdateDone = await self.CheckResUpdate();
            if (isUpdateDone)
            {
                Log.Info("更新完成，准备进入游戏");
                self.UpdateFinishAndStartGame().Coroutine();
            }
            else
            {
                Log.Info("不需要更新，直接进入游戏");
                self.OnOver?.Invoke();
                await self.CloseSelf();
            }
        }

        async static ETTask CheckIsInWhiteList(this UIUpdateView self)
        {
            var url = ServerConfigComponent.Instance.GetWhiteListCdnUrl();
            if (string.IsNullOrEmpty(url))
            {
                Log.Info(" no white list cdn url");
                return;
            }
            var info = await HttpManager.Instance.HttpGetResult<List<WhiteConfig>>(url);
            if (info != null)
            {
                ServerConfigComponent.Instance.SetWhiteList(info);
                if (ServerConfigComponent.Instance.IsInWhiteList())
                {
                    var btnState = await self.ShowMsgBoxView("Update_White", "Global_Btn_Confirm", "Global_Btn_Cancel");
                    if (btnState == self.BTN_CONFIRM)
                    {
                        ServerConfigComponent.Instance.SetWhiteMode(true);
                    }
                }
                return;
            }
        }

        async static ETTask CheckUpdateList(this UIUpdateView self)
        {
            var url = ServerConfigComponent.Instance.GetUpdateListCdnUrl();
            //UpdateConfig aa = new UpdateConfig
            //{
            //    app_list = new Dictionary<string, AppConfig>
            //    {
                    
            //    },
            //    res_list = new Dictionary<string, Dictionary<string, Resver>>
            //    {
            //        {"100",new Dictionary<string, Resver>{
            //            { "1.0.0",new Resver{
            //                channel = new List<string>(){"all"},
            //                update_tailnumber = new List<string>(){"all"},
            //            } }
            //        }}
            //    }
            //};
            var info = await HttpManager.Instance.HttpGetResult<UpdateConfig>(url);
            if (info == null)
            {
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", self.force_update?"Btn_Exit":"Update_Skip");
                if (btnState == self.BTN_CONFIRM)
                {
                    await self.CheckUpdateList();
                }
                else if(self.force_update)
                {
                    GameUtility.Quit();
                    return;
                }
            }
            else
            {
                ServerConfigComponent.Instance.SetUpdateList(info);
            }
        }

        async static ETTask<bool> CheckAppUpdate(this UIUpdateView self)
        {
            var app_channel = PlatformUtil.GetAppChannel();
            var channel_app_update_list = ServerConfigComponent.Instance.GetAppUpdateListByChannel(app_channel);
            if (channel_app_update_list == null || channel_app_update_list.app_ver == null)
            {
                Log.Info("CheckAppUpdate channel_app_update_list or app_ver is nil, so return");
                return false;
            }
            var maxVer = ServerConfigComponent.Instance.FindMaxUpdateAppVer(app_channel);
            if (string.IsNullOrEmpty(maxVer))
            {
                Log.Info("CheckAppUpdate maxVer is nil");
                return false;
            }
            var app_ver = Application.version;
            var flag = VersionCompare.Compare(app_ver, maxVer);
            Log.Info(string.Format("CoCheckAppUpdate AppVer:{0} maxVer:{1}", app_ver, maxVer));
            if (flag >= 0)
            {
                Log.Info("CheckAppUpdate AppVer is Most Max Version, so return; flag = " + flag);
                return false;
            }

            var app_url = channel_app_update_list.app_url;
            var verInfo = channel_app_update_list.app_ver[app_ver];
            Log.Info("CheckAppUpdate app_url = " + app_url);

            self.force_update = Define.ForceUpdate; 
            if (Define.ForceUpdate)//默认强更
            {
                if (verInfo != null && verInfo.force_update == 0)
                    self.force_update = false;
            }
            else
            {
                if (verInfo != null && verInfo.force_update != 0)
                    self.force_update = true;
            }


            var cancelBtnText = self.force_update ? "Btn_Exit" : "Btn_Enter_Game";
            var content_updata = self.force_update ? "Update_ReDownload" : "Update_SuDownload";
            var btnState = await self.ShowMsgBoxView(content_updata, "Global_Btn_Confirm", cancelBtnText);

            if (btnState == self.BTN_CONFIRM)
            {
                GameUtility.OpenURL(app_url);
                //为了防止切换到网页后回来进入了游戏，所以这里需要继续进入该流程
                return await self.CheckAppUpdate();
            }
            else if(self.force_update)
            {
                Log.Info("CheckAppUpdate Need Force Update And User Choose Exit Game!");
                GameUtility.Quit();
                return true;
            }
            return false;
        }

        //资源更新检查，并根据版本来修改资源cdn地址
        public static async ETTask<bool> CheckResUpdate(this UIUpdateView self)
        {
            var app_channel = PlatformUtil.GetAppChannel();
            var engine_ver = AssetBundleConfig.Instance.EngineVer;
            var maxVer = ServerConfigComponent.Instance.FindMaxUpdateResVer(engine_ver, app_channel);
            if (string.IsNullOrEmpty(maxVer))
            {
                Log.Info("CheckResUpdate No Max Ver EngineVer = " + engine_ver + " app_channel " + app_channel);
                return false;
            }

            var res_ver = AssetBundleConfig.Instance.ResVer;
            var flag = VersionCompare.Compare(res_ver, maxVer);
            Log.Info("CheckResUpdate ResVer:{0} maxVer:{1}", res_ver, maxVer);
            if (flag >= 0)
            {
                Log.Info("CheckResUpdate ResVer is Most Max Version, so return; flag = " + flag);
                return false;
            }

            // 编辑器下不能测试热更，但可以测试下载。
            if (Define.IsEditor) return false;

            //找到最新版本，则设置当前资源存放的cdn地址
            var url = ServerConfigComponent.Instance.GetUpdateCdnResUrlByVersion(maxVer);
            self.m_rescdn_url = url;
            Log.Info("CheckResUpdate res_cdn_url is " + url);
            AssetBundleMgr.GetInstance().SetAddressableRemoteResCdnUrl(self.m_rescdn_url);

            //等一会等addressables的Update回调执行完
            await TimerComponent.Instance.WaitAsync(1);

            //检查更新版本
            Log.Info("begin  CheckCatalogUpdates");
            var catalog = await self.CheckCatalogUpdates();
            Log.Info("CheckResUpdate CataLog = " + catalog);

            //1、先更新catalogs
            if (!string.IsNullOrEmpty(catalog))
            {
                Log.Info("begin  UpdateCatalogs");
                var res = await self.UpdateCatalogs(catalog);
                if (!res) return false;
                Log.Info("CoCheckResUpdate Update Catalog Success");
            }

            Log.Info("begin  GetDownloadSize");
            //获取需要更新的大小
            var size = await self.GetDownloadSize();

            //提示给用户
            Log.Info("downloadSize " + size);
            double size_mb = size / (1024f * 1024f);
            Log.Info("CheckResUpdate res size_mb is " + size_mb);//不屏蔽
            if (size_mb > 0 && size_mb < 0.01) size_mb = 0.01;

            var ct = I18NComponent.Instance.I18NGetParamText("Update_Info",size_mb.ToString("0.00"));
            var btnState = await self.ShowMsgBoxView(ct, "Global_Btn_Confirm", "Btn_Exit");
            if (btnState == self.BTN_CANCEL)
            {
                if (self.force_update)
                {
                    GameUtility.Quit();
                    return false;
                }
                return true;
            }

            //开始进行更新

            self.last_progress = 0;
            self.SetProgress(0);
            //2、更新资源

            var merge_mode_union = 1;
            var needdownloadinfo = await self.CheckUpdateContent(merge_mode_union);
            Log.Info("needdownloadinfo count: "+ needdownloadinfo.Count);
            self.m_needdownloadinfo = SortDownloadInfo(needdownloadinfo);

            Log.Info("CheckResUpdate DownloadContent begin");
            bool result = await self.DownloadContent();
            if (!result) return false;
            Log.Info("CheckResUpdate DownloadContent Success");
            return true;
        }

        static List<DownLoadInfo> SortDownloadInfo(Dictionary<string, string> needdownloadinfo)
        {
            List<DownLoadInfo> temp = new List<DownLoadInfo>();
            DownLoadInfo global_ab = null;
            if (needdownloadinfo!=null)
            {
                foreach (var item in needdownloadinfo)
                {
                    string name = item.Key;
                    string hash = item.Value;
                    Log.Info("SortDownloadInfo check =" + name);
                    if (name == "global_assets_all.bundle")
                        global_ab = new DownLoadInfo { hash = hash, name = name };
                    else
                        temp.Add(new DownLoadInfo { hash = hash, name = name });
                }
            }
            //版本资源最后
            if (global_ab != null)
                temp.Add(global_ab);
            return temp;
        }

        async static ETTask<long> GetDownloadSize(this UIUpdateView self)
        {
            var size = await AddressablesManager.Instance.GetDownloadSizeAsync("default");
            if (size<0)
            {
                Log.Info("CoGetDownloadSize Get Download Size Async Faild");
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", self.force_update?"Btn_Exit":"Update_Skip");
                if (btnState == self.BTN_CONFIRM)
                    return await self.GetDownloadSize();
                else
                {
                    if (self.force_update)
                        GameUtility.Quit();
                    return 0;
                }
            }
            return size;
        }

        //检查更新Catalog
        async static ETTask<string> CheckCatalogUpdates(this UIUpdateView self)
        {
            var catlog = await AddressablesManager.Instance.CheckForCatalogUpdates();
            if (!string.IsNullOrEmpty(catlog)) return catlog;
            else
            {
                Log.Info("CheckCatalogUpdates Check CataLog Failed");
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", "Btn_Exit");
                if (btnState == self.BTN_CONFIRM)
                    return await self.CheckCatalogUpdates();
                else
                {
                    if(self.force_update)
                        GameUtility.Quit();
                    return null;
                }
            }
        }

        //更新catalogs
        async static ETTask<bool> UpdateCatalogs(this UIUpdateView self,string catalog)
        {
            //这里可能连不上，导致认为UpdateCatalogs成功
            var res = await AddressablesManager.Instance.CheckForCatalogUpdates();
            if (!string.IsNullOrEmpty(res))
            {
                var updateRes = await AddressablesManager.Instance.UpdateCatalogs(catalog);
                if (updateRes) return true;
                else
                {
                    Log.Info("CoUpdateCatalogs Update Catalog handler retry");
                    return await self.UpdateCatalogs(catalog);
                }
            }
            else
            {
                Log.Info("CoUpdateCatalogs Update Catalog Failed");
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", self.force_update?"Btn_Exit":"Update_Skip");
                if (btnState == self.BTN_CONFIRM)
                    return await self.UpdateCatalogs(catalog);
                else
                {
                    if(self.force_update)
                        GameUtility.Quit();
                    return false;

                }
            }
        }

        async static ETTask<Dictionary<string,string>> CheckUpdateContent(this UIUpdateView self,int merge_mode_union)
        {
            var res = await AddressablesManager.Instance.CheckUpdateContent(new List<string>() { "default" }, merge_mode_union);
            if (res!=null) return res;
            else
            {
                Log.Info("CheckUpdateContent Failed");
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", self.force_update?"Btn_Exit":"Update_Skip");
                if (btnState == self.BTN_CONFIRM)
                    return await self.CheckUpdateContent(merge_mode_union);
                else
                {
                    if(self.force_update)
                        GameUtility.Quit();
                    return null;
                }
            }
        }
        static void SetProgress(this UIUpdateView self, float value)
        {
            if(value> self.last_progress)
                self.last_progress = value;
            self.m_slider.SetNormalizedValue(self.last_progress);
        }
        async static ETTask<bool> DownloadContent(this UIUpdateView self)
        {
            var url = ServerConfigComponent.Instance.GetUpdateListCdnUrl();
            var info = await HttpManager.Instance.HttpGetResult(url);
            if (!string.IsNullOrEmpty(info))
            {
                await self.DownloadAllAssetBundle();
                return true;
            }
            else
            {
                Log.Info("DownloadContent Begin DownloadDependenciesAsync failed");
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", self.force_update?"Btn_Exit":"Update_Skip");
                if (btnState == self.BTN_CONFIRM)
                    return await self.DownloadContent();
                else
                {
                    if(self.force_update)
                        GameUtility.Quit();
                    return false;
                }
            }
        }

        async static ETTask DownloadAllAssetBundle(this UIUpdateView self)
        {
            var downloadTool = self.AddComponent<DownloadComponent>();
            for (int i = 0; i < self.m_needdownloadinfo.Count; i++)
            {
                var url = string.Format("{0}/{1}", self.m_rescdn_url, self.m_needdownloadinfo[i].name);
                var savePath = AssetBundleMgr.GetInstance().getCachedAssetBundlePath(self.m_needdownloadinfo[i].name) + ".temp";
                downloadTool.AddDownloadUrl(url,savePath);
            }
            self.RefreshProgress(downloadTool).Coroutine();
            var res = await downloadTool.DownloadAll();
            if (!res)
            {
                var btnState = await self.ShowMsgBoxView("Update_Download_Fail", "Update_ReTry",
                    self.force_update ? "Btn_Exit" : "Btn_Cancel");
                if (btnState == self.BTN_CONFIRM)
                {
                    await self.DownloadAllAssetBundle();
                }
                else if (self.force_update)
                {
                    GameUtility.Quit();
                }
            }
            else
            {
                for (int i = 0; i < self.m_needdownloadinfo.Count; i++)
                {
                    var downinfo = self.m_needdownloadinfo[i];
                    AssetBundleMgr.GetInstance().CacheAssetBundle(downinfo.name, downinfo.hash);
                }
            }

        }
        static async ETTask RefreshProgress(this UIUpdateView self, DownloadComponent downloadTool)
        {
            while (self.m_needdownloadinfo.Count!= self.overCount)
            {
                self.SetProgress(downloadTool.GetProress());
                await TimerComponent.Instance.WaitAsync(10);
            }
        }
    }
}
