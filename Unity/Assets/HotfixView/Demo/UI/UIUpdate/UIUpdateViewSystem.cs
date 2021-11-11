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
    public class UIUpdateViewOnCreateSystem : OnCreateSystem<UIUpdateView>
    {
        public override void OnCreate(UIUpdateView self)
        {
            self.m_slider = self.AddComponent<UISlider>("Loadingscreen/Slider");
            self.m_msgBoxView = self.transform.Find("msgbox_view").gameObject;
            self.m_msgBoxViewText = self.AddComponent<UIText>("msgbox_view/Text");
            self.m_msgBoxViewBtnCancel = self.AddComponent<UIButton>("msgbox_view/btn_cancel");
            self.m_msgBoxViewBtnCancelText = self.AddComponent<UIText>("msgbox_view/btn_cancel/Text");
            self.m_msgBoxViewBtnConfirm = self.AddComponent<UIButton>("msgbox_view/btn_confirm");
            self.m_msgBoxViewBtnConfirmText = self.AddComponent<UIText>("msgbox_view/btn_confirm/Text");
            self.m_msgBoxView.SetActive(false);
        }
    }

    public class UIUpdateViewOnEnableSystem : OnEnableSystem<UIUpdateView>
    {
        public override void OnEnable(UIUpdateView self)
        {
            self.m_slider.SetValue(0);
            self.StartCheckUpdate().Coroutine();
        }
    }
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

            //预加载Lua
            //AddressablesManager.Instance.ReleaseLuas();

            //重新加载配置
            AssetBundleConfig.Instance.SyncLoadGlobalAssetBundle();

            Game.EventSystem.Publish(new EventType.AppStart()).Coroutine();
        }


        public static void SetSlidValue(this UIUpdateView self,float pro)
        {
            self.m_slider.SetValue(pro);
        }

        static void HideMsgBoxView(this UIUpdateView self)
        {
            self.m_msgBoxView.SetActive(false);

        }

        async static ETTask<int> ShowMsgBoxView(this UIUpdateView self,string content, string confirmBtnText, string cancelBtnText)
        {
            var btnState = self.BTN_NONE;
            UnityAction confirmBtnFunc = () =>
             {
                 self.HideMsgBoxView();
                 btnState = self.BTN_CONFIRM;
             };

            UnityAction cancelBtnFunc = () =>
            {
                self.HideMsgBoxView();
                btnState = self.BTN_CANCEL;
            };
            self.m_msgBoxView.SetActive(true);
            self.m_msgBoxViewText.SetText(content);

            self.m_msgBoxViewBtnConfirm.SetOnClick(confirmBtnFunc);
            self.m_msgBoxViewBtnConfirmText.SetText(confirmBtnText);

            if (!string.IsNullOrEmpty(cancelBtnText))
            {
                self.m_msgBoxViewBtnCancel.gameObject.SetActive(true);
                self.m_msgBoxViewBtnCancel.SetOnClick(cancelBtnFunc);
                self.m_msgBoxViewBtnCancelText.SetText(cancelBtnText);
            }
            else
            {
                self.m_msgBoxViewBtnCancel.gameObject.SetActive(false);
            }
            while (btnState <= self.BTN_NONE)
                await TimerComponent.Instance.WaitAsync(1);
            return btnState;
        }
        public static async ETVoid StartCheckUpdate(this UIUpdateView self)
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
                Scene zoneScene = await SceneFactory.CreateZoneScene(1, "Game", Game.Scene);

                await Game.EventSystem.Publish(new EventType.AppStartInitFinish() { ZoneScene = zoneScene });
                self.CloseSelf();
            }
        }

        async static ETTask CheckIsInWhiteList(this UIUpdateView self)
        {
            var url = BootConfig.Instance.GetWhiteListCdnUrl();
            if (string.IsNullOrEmpty(url))
            {
                Log.Info(" no white list cdn url");
                return;
            }
            var info = await HttpManager.Instance.HttpGetResult<List<WhiteConfig>>(url);
            if (info != null)
            {
                BootConfig.Instance.SetWhiteList(info);
                if (BootConfig.Instance.IsInWhiteList())
                {
                    var btnState = await self.ShowMsgBoxView("是否进入白名单模式", "确认", "取消");
                    if (btnState == self.BTN_CONFIRM)
                    {
                        BootConfig.Instance.SetWhiteMode(true);
                    }
                    self.m_msgBoxViewBtnConfirm.RemoveOnClick();
                    self.m_msgBoxViewBtnCancel.RemoveOnClick();
                }
                return;
            }
        }

        async static ETTask CheckUpdateList(this UIUpdateView self)
        {
            var url = BootConfig.Instance.GetUpdateListCdnUrl();
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
            //Log.Info(JsonHelper.ToJson(aa));
            var info = await HttpManager.Instance.HttpGetResult<UpdateConfig>(url);
            if (info == null)
            {
                Log.Error("CheckUpdateList error");
                GameUtility.Quit();
                return;
            }
            BootConfig.Instance.SetUpdateList(info);

        }

        async static ETTask<bool> CheckAppUpdate(this UIUpdateView self)
        {
            var app_channel = PlatformUtil.GetAppChannel();
            var channel_app_update_list = BootConfig.Instance.GetAppUpdateListByChannel(app_channel);
            if (channel_app_update_list == null || channel_app_update_list.app_ver == null)
            {
                Log.Info("CheckAppUpdate channel_app_update_list or app_ver is nil, so return");
                return false;
            }
            var maxVer = BootConfig.Instance.FindMaxUpdateAppVer(app_channel);
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

            var force_update = true;//默认强更
            if (verInfo != null && verInfo.force_update == 0)
                force_update = false;

            var cancelBtnText = force_update ? "退出游戏" : "进入游戏";
            var content_updata = force_update ? "當前版本過低，請重新下載客戶端" : "當前版本較低，建議下載最新客戶端";
            var btnState = await self.ShowMsgBoxView(content_updata, "确认", cancelBtnText);
            self.m_msgBoxViewBtnConfirm.RemoveOnClick();
            self.m_msgBoxViewBtnCancel.RemoveOnClick();

            if (btnState == self.BTN_CONFIRM)
            {
                GameUtility.OpenURL(app_url);
                return await self.CheckAppUpdate();
            }
            else
            {
                if (force_update)
                {
                    Log.Info("CheckAppUpdate Need Force Update And User Choose Exit Game!");
                    GameUtility.Quit();
                    return true;
                }
            }
            return false;
        }

        //资源更新检查，并根据版本来修改资源cdn地址
        public static async ETTask<bool> CheckResUpdate(this UIUpdateView self)
        {
            var app_channel = PlatformUtil.GetAppChannel();
            var engine_ver = AssetBundleConfig.Instance.EngineVer;
            var maxVer = BootConfig.Instance.FindMaxUpdateResVer(engine_ver, app_channel);
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
            //if (PlatformUtil.IsEditor()) return false;

            //找到最新版本，则设置当前资源存放的cdn地址
            var url = BootConfig.Instance.GetUpdateCdnResUrlByVersion(maxVer);
            self.m_rescdn_url = url;
            Log.Info("CheckResUpdate res_cdn_url is " + url);
            AssetBundleMgr.GetInstance().SetAddressableRemoteResCdnUrl(self.m_rescdn_url);

            //检查更新版本
            Log.Info("begin  CheckCatalogUpdates");
            var handler = await self.CheckCatalogUpdates();
            if (handler == null) return false;

            var catalog = handler.catalog;
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
            handler = await self.GetDownloadSize();
            if (handler == null) return false;
            var size = handler.downloadSize;

            //提示给用户
            Log.Info("downloadSize " + size);
            double size_mb = size / (1024f * 1024f);
            Log.Info("CheckResUpdate res size_mb is " + size_mb);//不屏蔽
            if (size_mb > 0 && size_mb < 0.01) size_mb = 0.01;

            var ct = "檢測到資源更新\n更新包大小：<color=#DB744C>{0}</color> MB\n是否立即下載？".Fmt(size_mb.ToString("0.00"));
            var btnState = await self.ShowMsgBoxView(ct, "确认", "退出游戏");
            self.m_msgBoxViewBtnConfirm.RemoveOnClick();
            self.m_msgBoxViewBtnCancel.RemoveOnClick();
            if (btnState == self.BTN_CANCEL)
            {
                GameUtility.Quit();
                return false;
            }

            //开始进行更新
            self.m_slider.SetValue(0);
            //2、更新资源

            var merge_mode_union = 1;
            handler = AddressablesManager.Instance.CheckUpdateContent(new List<string>() { "default" }, merge_mode_union);
            while (!handler.isDone)
                await TimerComponent.Instance.WaitAsync(1);

            var needdownloadinfo = handler.GetNeedDownloadinfo();
            Log.Info("needdownloadinfo: ", needdownloadinfo);
            self.m_needdownloadinfo = self.SortDownloadInfo(JsonHelper.FromJson<Dictionary<string, string>>(needdownloadinfo));

            self.finish_count = 0;
            //记录下载好了的资源的index
            self.m_download_finish_index = new Dictionary<int, bool>();
            self.last_progress = 0;
            Log.Info("CheckResUpdate DownloadContent begin");
            bool result = await self.DownloadContent();
            if (!result) return false;
            Log.Info("CheckResUpdate DownloadContent Success");
            return true;
        }

        static List<DownLoadInfo> SortDownloadInfo(this UIUpdateView self,Dictionary<string, string> needdownloadinfo)
        {
            List<DownLoadInfo> temp = new List<DownLoadInfo>();
            DownLoadInfo global_ab = null;
            DownLoadInfo lua_ab = null;
            if (needdownloadinfo!=null)
            {
                foreach (var item in needdownloadinfo)
                {
                    string name = item.Key;
                    string hash = item.Value;
                    Log.Info("SortDownloadInfo check =" + name);
                    if (name == "global_assets_all.bundle")
                        global_ab = new DownLoadInfo { hash = hash, name = name };
                    else if (name == "luascript_bytes_content_assets_all.bundle")
                        lua_ab = new DownLoadInfo { hash = hash, name = name };
                    else
                        temp.Add(new DownLoadInfo { hash = hash, name = name });
                }
            }
            //版本资源最后
            if (lua_ab != null)
                temp.Add(lua_ab);
            if (global_ab != null)
                temp.Add(global_ab);
            return temp;
        }

        async static ETTask<AddressableUpdateAsyncOperation> GetDownloadSize(this UIUpdateView self)
        {
            var handler = AddressablesManager.Instance.GetDownloadSizeAsync(new string[] { "default" });
            while (!handler.isDone)
                await TimerComponent.Instance.WaitAsync(1);
            if (handler.isSuccess)
                return handler;
            else
            {
                Log.Info("CoGetDownloadSize Get Download Size Async Faild");
                var btnState = await self.ShowMsgBoxView("獲取更新失敗，請檢查網路", "重試", "退出遊戲");
                self.m_msgBoxViewBtnConfirm.RemoveOnClick();
                self.m_msgBoxViewBtnCancel.RemoveOnClick();
                if (btnState == self.BTN_CONFIRM)
                    return await self.GetDownloadSize();
                else
                {
                    GameUtility.Quit();
                    return null;
                }
            }
        }

        //检查更新Catalog
        async static ETTask<AddressableUpdateAsyncOperation> CheckCatalogUpdates(this UIUpdateView self)
        {
            var handler = AddressablesManager.Instance.CheckForCatalogUpdates();
            while (!handler.isDone)
                await TimerComponent.Instance.WaitAsync(1);
            if (handler.isSuccess) return handler;
            else
            {
                Log.Info("CheckCatalogUpdates Check CataLog Failed");
                var btnState = await self.ShowMsgBoxView("獲取更新失敗，請檢查網路", "重試", "退出遊戲");
                self.m_msgBoxViewBtnConfirm.RemoveOnClick();
                self.m_msgBoxViewBtnCancel.RemoveOnClick();
                if (btnState == self.BTN_CONFIRM)
                    return await self.CheckCatalogUpdates();
                else
                {
                    GameUtility.Quit();
                    return null;
                }
            }
        }

        //更新catalogs
        async static ETTask<bool> UpdateCatalogs(this UIUpdateView self,string catalog)
        {
            //这里可能连不上，导致认为UpdateCatalogs成功
            var handler = AddressablesManager.Instance.CheckForCatalogUpdates();
            while (!handler.isDone)
                await TimerComponent.Instance.WaitAsync(1);
            if (handler.isSuccess)
            {
                handler = AddressablesManager.Instance.UpdateCatalogs(catalog);
                while (!handler.isDone)
                    await TimerComponent.Instance.WaitAsync(1);
                if (handler.isSuccess) return true;
                else
                {
                    Log.Info("CoUpdateCatalogs Update Catalog handler retry");
                    return await self.UpdateCatalogs(catalog);
                }
            }
            else
            {
                Log.Info("CoUpdateCatalogs Update Catalog Failed");
                var btnState = await self.ShowMsgBoxView("獲取更新失敗，請檢查網路", "重試", "退出游戏");
                self.m_msgBoxViewBtnConfirm.RemoveOnClick();
                self.m_msgBoxViewBtnCancel.RemoveOnClick();
                if (btnState == self.BTN_CONFIRM)
                    return await self.UpdateCatalogs(catalog);
                else
                {
                    GameUtility.Quit();
                    return false;

                }
            }
        }

        async static ETTask<bool> DownloadContent(this UIUpdateView self)
        {
            var url = BootConfig.Instance.GetUpdateListCdnUrl();
            var info = await HttpManager.Instance.HttpGetResult(url);
            if (!string.IsNullOrEmpty(info))
            {
                await self.DownloadAllAssetBundle((progress) => { self.m_slider.SetValue(progress); });
                if(self.CheckNeedContinueDownload())
                {
                    Log.Info("DownloadContent DownloadDependenciesAsync retry");
                    return await self.DownloadContent();
                }
                else
                    return true;
            }
            else
            {
                Log.Info("DownloadContent Begin DownloadDependenciesAsync failed");
                var btnState = await self.ShowMsgBoxView("獲取更新失敗，請檢查網路", "重試", "退出遊戲");
                self.m_msgBoxViewBtnConfirm.RemoveOnClick();
                self.m_msgBoxViewBtnCancel.RemoveOnClick();
                if (btnState == self.BTN_CONFIRM)
                    return await self.DownloadContent();
                else
                {
                    GameUtility.Quit();
                    return false;
                }
            }
        }

        async static ETTask DownloadAllAssetBundle(this UIUpdateView self,Action<float> callback)
        {
            var total_count = self.m_needdownloadinfo.Count;
            Log.Info("DownloadAllAssetBundle count = " + total_count);
            if (total_count <= 0)
                return;
            var progress_slice = 1f / total_count;
            self.pool_conn = new Dictionary<int, PoolConn>();
            self.conn_num = total_count;
            var n_index = 1;
            var n_end = self.m_needdownloadinfo.Count - 1;
            n_index = self.AddDownloadConn(n_index, n_end);
            do
            {
                var total = 0f;
                var keys = self.pool_conn.Keys.ToList();
                foreach (var item in keys)
                {
                    var vo = self.pool_conn[item];
                    var k = item;
                    if (vo.dirty)
                    {
                        vo.asyncOp.Dispose();
                        self.pool_conn.Remove(k);
                    }
                    else
                    {
                        if (vo.asyncOp.isDone)
                        {
                            if (vo.asyncOp.isSuccess)
                            {
                                self.m_download_finish_index[vo.idx] = true;
                                self.finish_count++;
                            }
                            else
                                Log.Error("error == " + vo.asyncOp.errorMsg);
                            vo.dirty = true;//下一帧处理
                        }
                        else
                            total += vo.asyncOp.progress;
                    }
                }
                var progress = (self.finish_count + total) * progress_slice;
                if (progress > self.last_progress)
                {
                    callback(progress);
                    self.last_progress = progress;
                }
                n_index = self.AddDownloadConn(n_index, n_end);
                await TimerComponent.Instance.WaitAsync(1);
            } while (n_index <= n_end || self.GetDownConnCount() != 0);
            callback(self.finish_count * progress_slice);

            foreach (var item in self.pool_conn)
            {
                item.Value.asyncOp.Dispose();
            }
            self.pool_conn.Clear();

            //下载最后的版本资源
            var end_num = self.m_needdownloadinfo.Count;
            if (self.OtherDownloadFinish(end_num))
            {
                if (!self.m_download_finish_index.ContainsKey(end_num))
                {
                    var asyncOp = self.DownloadResinfo(end_num);
                    while (!asyncOp.isDone) 
                        await TimerComponent.Instance.WaitAsync(1);
                    if (asyncOp.isSuccess)
                    {
                        self.m_download_finish_index[end_num] = true;
                        self.finish_count++;
                        callback(self.finish_count * progress_slice);
                    }
                    else
                        Log.Info("error == " + asyncOp.errorMsg);
                    asyncOp.Dispose();
                }
                Log.Info("DownloadAllAssetBundle end");
            }
        }
        static bool OtherDownloadFinish(this UIUpdateView self,int res_idx)
        {
            for (int i = 1; i <= self.m_needdownloadinfo.Count; i++)
                if ((i != res_idx) && !self.m_download_finish_index.ContainsKey(i))
                    return false;
            return true;
        }
        static int AddDownloadConn(this UIUpdateView self,int order, int n_end)
        {
            while (order <= n_end)
            {
                var conn_idx = self.GetConnIdx();
                if (conn_idx != null)
                {
                    if (!self.m_download_finish_index.TryGetValue(order, out var res) || res == false)
                    {
                        self.pool_conn[(int)conn_idx] = new PoolConn { idx = order, asyncOp = self.DownloadResinfo(order) };

                    }
                    order = order + 1;
                }
                else break;
            }
            return order;
        }

        static int GetDownConnCount(this UIUpdateView self)
        {
            var num = 0;
            foreach (var item in self.pool_conn)
            {
                if (!item.Value.dirty)
                    num++;
            }
            return num;

        }

        static int? GetConnIdx(this UIUpdateView self)
        {
            for (int i = 0; i < self.conn_num; i++)
            {
                if (!self.pool_conn.ContainsKey(i))
                    return i;
            }
            return null;
        }

        static DownloadAssetBundleAsyncOperation DownloadResinfo(this UIUpdateView self,int order)
        {
            var downinfo = self.m_needdownloadinfo[order-1];
            var url = string.Format("{0}/{1}", self.m_rescdn_url, downinfo.name);
            Log.Info("download ab ============, " + order + " = " + url);
            var asyncOp = ABDownload.Instance.DownloadAssetBundle(url, downinfo.hash);
            return asyncOp;
        }

        static bool CheckNeedContinueDownload(this UIUpdateView self)
        {
            if (self.m_needdownloadinfo.Count > 0 && self.m_download_finish_index.Count == self.m_needdownloadinfo.Count) return false;
            for (int i = 1; i <= self.m_needdownloadinfo.Count; i++)
            {
                if (!self.m_download_finish_index.ContainsKey(i))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
