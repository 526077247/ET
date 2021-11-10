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
    public class DownLoadInfo
    {
        public string name;
        public string hash;

    }

    public class PoolConn
    {
        public int idx;
        public DownloadAssetBundleAsyncOperation asyncOp;
        public bool dirty;

    }
    public class UIUpdateView : UIBaseView
    {

        const int BTN_NONE = 0;
        const int BTN_CANCEL = 1;
        const int BTN_CONFIRM = 2;

        UISlider m_slider;
        GameObject m_msgBoxView;
        UIText m_msgBoxViewText;
        UIButton m_msgBoxViewBtnCancel;
        UIText m_msgBoxViewBtnCancelText;
        UIButton m_msgBoxViewBtnConfirm;
        UIText m_msgBoxViewBtnConfirmText;

        List<DownLoadInfo> m_needdownloadinfo = null;
        string m_rescdn_url;
        Dictionary<int, bool> m_download_finish_index = null;
        int conn_num = 4;//默认连接数
        Dictionary<int, PoolConn> pool_conn = null;
        int finish_count = 0;
        float last_progress;

        public override string PrefabPath => "UI/UIUpdate/Prefabs/UIUpdateView.prefab";

        public override void OnCreate()
        {
            base.OnCreate();
            m_slider = AddComponent<UISlider>("Loadingscreen/Slider");
            m_msgBoxView = transform.Find("msgbox_view").gameObject;
            m_msgBoxViewText = AddComponent<UIText>("msgbox_view/Text");
            m_msgBoxViewBtnCancel = AddComponent<UIButton>("msgbox_view/btn_cancel");
            m_msgBoxViewBtnCancelText = AddComponent<UIText>("msgbox_view/btn_cancel/Text");
            m_msgBoxViewBtnConfirm = AddComponent<UIButton>("msgbox_view/btn_confirm");
            m_msgBoxViewBtnConfirmText = AddComponent<UIText>("msgbox_view/btn_confirm/Text");
            m_msgBoxView.SetActive(false);
        }

        private async ETTask UpdateFinishAndStartGame()
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

        public override void OnEnable()
        {
            base.OnEnable();
            m_slider.SetValue(0);
            StartCheckUpdate().Coroutine();
        }

        public void SetSlidValue(float pro)
        {
            m_slider.SetValue(pro);
        }

        void HideMsgBoxView()
        {
            m_msgBoxView.SetActive(false);

        }

        async ETTask<int> ShowMsgBoxView(string content, string confirmBtnText, string cancelBtnText)
        {
            var btnState = BTN_NONE;
            UnityAction confirmBtnFunc = () =>
             {
                 HideMsgBoxView();
                 btnState = BTN_CONFIRM;
             };

            UnityAction cancelBtnFunc = () =>
            {
                HideMsgBoxView();
                btnState = BTN_CANCEL;
            };
            m_msgBoxView.SetActive(true);
            m_msgBoxViewText.SetText(content);

            m_msgBoxViewBtnConfirm.SetOnClick(confirmBtnFunc);
            m_msgBoxViewBtnConfirmText.SetText(confirmBtnText);

            if (!string.IsNullOrEmpty(cancelBtnText))
            {
                m_msgBoxViewBtnCancel.gameObject.SetActive(true);
                m_msgBoxViewBtnCancel.SetOnClick(cancelBtnFunc);
                m_msgBoxViewBtnCancelText.SetText(cancelBtnText);
            }
            else
            {
                m_msgBoxViewBtnCancel.gameObject.SetActive(false);
            }
            while (btnState <= BTN_NONE)
                await TimerComponent.Instance.WaitAsync(1);
            return btnState;
        }
        async ETVoid StartCheckUpdate()
        {
            //TODO 网络检查 
            await CheckIsInWhiteList();

            await CheckUpdateList();

            var Over = await CheckAppUpdate();
            if (Over) return;

            var isUpdateDone = await CheckResUpdate();
            if (isUpdateDone)
            {
                Log.Info("更新完成，准备进入游戏");
                UpdateFinishAndStartGame().Coroutine();
            }
            else
            {
                Log.Info("不需要更新，直接进入游戏");
                Scene zoneScene = await SceneFactory.CreateZoneScene(1, "Game", Game.Scene);

                await Game.EventSystem.Publish(new EventType.AppStartInitFinish() { ZoneScene = zoneScene });
                this.CloseSelf();
            }
        }

        async ETTask CheckIsInWhiteList()
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
                    var btnState = await ShowMsgBoxView("是否进入白名单模式", "确认", "取消");
                    if (btnState == BTN_CONFIRM)
                    {
                        BootConfig.Instance.SetWhiteMode(true);
                    }
                    m_msgBoxViewBtnConfirm.RemoveOnClick();
                    m_msgBoxViewBtnCancel.RemoveOnClick();
                }
                return;
            }
        }

        async ETTask CheckUpdateList()
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

        async ETTask<bool> CheckAppUpdate()
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
            var btnState = await ShowMsgBoxView(content_updata, "确认", cancelBtnText);
            m_msgBoxViewBtnConfirm.RemoveOnClick();
            m_msgBoxViewBtnCancel.RemoveOnClick();

            if (btnState == BTN_CONFIRM)
            {
                GameUtility.OpenURL(app_url);
                return await CheckAppUpdate();
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
        public async ETTask<bool> CheckResUpdate()
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
            m_rescdn_url = url;
            Log.Info("CheckResUpdate res_cdn_url is " + url);
            AssetBundleMgr.GetInstance().SetAddressableRemoteResCdnUrl(m_rescdn_url);

            //检查更新版本
            Log.Info("begin  CheckCatalogUpdates");
            var handler = await CheckCatalogUpdates();
            if (handler == null) return false;

            var catalog = handler.catalog;
            Log.Info("CheckResUpdate CataLog = " + catalog);

            //1、先更新catalogs
            if (!string.IsNullOrEmpty(catalog))
            {
                Log.Info("begin  UpdateCatalogs");
                var res = await UpdateCatalogs(catalog);
                if (!res) return false;
                Log.Info("CoCheckResUpdate Update Catalog Success");
            }

            Log.Info("begin  GetDownloadSize");
            //获取需要更新的大小
            handler = await GetDownloadSize();
            if (handler == null) return false;
            var size = handler.downloadSize;

            //提示给用户
            Log.Info("downloadSize " + size);
            double size_mb = size / (1024f * 1024f);
            Log.Info("CheckResUpdate res size_mb is " + size_mb);//不屏蔽
            if (size_mb > 0 && size_mb < 0.01) size_mb = 0.01;

            var ct = "檢測到資源更新\n更新包大小：<color=#DB744C>{0}</color> MB\n是否立即下載？".Fmt(size_mb.ToString("0.00"));
            var btnState = await ShowMsgBoxView(ct, "确认", "退出游戏");
            m_msgBoxViewBtnConfirm.RemoveOnClick();
            m_msgBoxViewBtnCancel.RemoveOnClick();
            if (btnState == BTN_CANCEL)
            {
                GameUtility.Quit();
                return false;
            }

            //开始进行更新
            m_slider.SetValue(0);
            //2、更新资源

            var merge_mode_union = 1;
            handler = AddressablesManager.Instance.CheckUpdateContent(new List<string>() { "default" }, merge_mode_union);
            while (!handler.isDone)
                await TimerComponent.Instance.WaitAsync(1);

            var needdownloadinfo = handler.GetNeedDownloadinfo();
            Log.Info("needdownloadinfo: ", needdownloadinfo);
            m_needdownloadinfo = SortDownloadInfo(JsonHelper.FromJson<Dictionary<string, string>>(needdownloadinfo));

            finish_count = 0;
            //记录下载好了的资源的index
            m_download_finish_index = new Dictionary<int, bool>();
            last_progress = 0;
            Log.Info("CheckResUpdate DownloadContent begin");
            bool result = await DownloadContent();
            if (!result) return false;
            Log.Info("CheckResUpdate DownloadContent Success");
            return true;
        }

        List<DownLoadInfo> SortDownloadInfo(Dictionary<string, string> needdownloadinfo)
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

        async ETTask<AddressableUpdateAsyncOperation> GetDownloadSize()
        {
            var handler = AddressablesManager.Instance.GetDownloadSizeAsync(new string[] { "default" });
            while (!handler.isDone)
                await TimerComponent.Instance.WaitAsync(1);
            if (handler.isSuccess)
                return handler;
            else
            {
                Log.Info("CoGetDownloadSize Get Download Size Async Faild");
                var btnState = await ShowMsgBoxView("獲取更新失敗，請檢查網路", "重試", "退出遊戲");
                m_msgBoxViewBtnConfirm.RemoveOnClick();
                m_msgBoxViewBtnCancel.RemoveOnClick();
                if (btnState == BTN_CONFIRM)
                    return await GetDownloadSize();
                else
                {
                    GameUtility.Quit();
                    return null;
                }
            }
        }

        //检查更新Catalog
        async ETTask<AddressableUpdateAsyncOperation> CheckCatalogUpdates()
        {
            var handler = AddressablesManager.Instance.CheckForCatalogUpdates();
            while (!handler.isDone)
                await TimerComponent.Instance.WaitAsync(1);
            if (handler.isSuccess) return handler;
            else
            {
                Log.Info("CheckCatalogUpdates Check CataLog Failed");
                var btnState = await ShowMsgBoxView("獲取更新失敗，請檢查網路", "重試", "退出遊戲");
                m_msgBoxViewBtnConfirm.RemoveOnClick();
                m_msgBoxViewBtnCancel.RemoveOnClick();
                if (btnState == BTN_CONFIRM)
                    return await CheckCatalogUpdates();
                else
                {
                    GameUtility.Quit();
                    return null;
                }
            }
        }

        //更新catalogs
        async ETTask<bool> UpdateCatalogs(string catalog)
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
                    return await UpdateCatalogs(catalog);
                }
            }
            else
            {
                Log.Info("CoUpdateCatalogs Update Catalog Failed");
                var btnState = await ShowMsgBoxView("獲取更新失敗，請檢查網路", "重試", "退出游戏");
                m_msgBoxViewBtnConfirm.RemoveOnClick();
                m_msgBoxViewBtnCancel.RemoveOnClick();
                if (btnState == BTN_CONFIRM)
                    return await UpdateCatalogs(catalog);
                else
                {
                    GameUtility.Quit();
                    return false;

                }
            }
        }

        async ETTask<bool> DownloadContent()
        {
            var url = BootConfig.Instance.GetUpdateListCdnUrl();
            var info = await HttpManager.Instance.HttpGetResult(url);
            if (!string.IsNullOrEmpty(info))
            {
                await DownloadAllAssetBundle((progress) => { m_slider.SetValue(progress); });
                if(CheckNeedContinueDownload())
                {
                    Log.Info("DownloadContent DownloadDependenciesAsync retry");
                    return await DownloadContent();
                }
                else
                    return true;
            }
            else
            {
                Log.Info("DownloadContent Begin DownloadDependenciesAsync failed");
                var btnState = await ShowMsgBoxView("獲取更新失敗，請檢查網路", "重試", "退出遊戲");
                m_msgBoxViewBtnConfirm.RemoveOnClick();
                m_msgBoxViewBtnCancel.RemoveOnClick();
                if (btnState == BTN_CONFIRM)
                    return await DownloadContent();
                else
                {
                    GameUtility.Quit();
                    return false;
                }
            }
        }

        async ETTask DownloadAllAssetBundle(Action<float> callback)
        {
            var total_count = m_needdownloadinfo.Count;
            Log.Info("DownloadAllAssetBundle count = " + total_count);
            if (total_count <= 0)
                return;
            var progress_slice = 1f / total_count;
            pool_conn = new Dictionary<int, PoolConn>();
            conn_num = total_count;
            var n_index = 1;
            var n_end = m_needdownloadinfo.Count - 1;
            n_index = AddDownloadConn(n_index, n_end);
            do
            {
                var total = 0f;
                var keys = pool_conn.Keys.ToList();
                foreach (var item in keys)
                {
                    var vo = pool_conn[item];
                    var k = item;
                    if (vo.dirty)
                    {
                        vo.asyncOp.Dispose();
                        pool_conn.Remove(k);
                    }
                    else
                    {
                        if (vo.asyncOp.isDone)
                        {
                            if (vo.asyncOp.isSuccess)
                            {
                                m_download_finish_index[vo.idx] = true;
                                finish_count++;
                            }
                            else
                                Log.Error("error == " + vo.asyncOp.errorMsg);
                            vo.dirty = true;//下一帧处理
                        }
                        else
                            total += vo.asyncOp.progress;
                    }
                }
                var progress = (finish_count + total) * progress_slice;
                if (progress > last_progress)
                {
                    callback(progress);
                    last_progress = progress;
                }
                n_index = AddDownloadConn(n_index, n_end);
                await TimerComponent.Instance.WaitAsync(1);
            } while (n_index <= n_end || GetDownConnCount() != 0);
            callback(finish_count * progress_slice);

            foreach (var item in pool_conn)
            {
                item.Value.asyncOp.Dispose();
            }
            pool_conn.Clear();

            //下载最后的版本资源
            var end_num = m_needdownloadinfo.Count;
            if (OtherDownloadFinish(end_num))
            {
                if (!m_download_finish_index.ContainsKey(end_num))
                {
                    var asyncOp = DownloadResinfo(end_num);
                    while (!asyncOp.isDone) 
                        await TimerComponent.Instance.WaitAsync(1);
                    if (asyncOp.isSuccess)
                    {
                        m_download_finish_index[end_num] = true;
                        finish_count++;
                        callback(finish_count * progress_slice);
                    }
                    else
                        Log.Info("error == " + asyncOp.errorMsg);
                    asyncOp.Dispose();
                }
                Log.Info("DownloadAllAssetBundle end");
            }
        }
        bool OtherDownloadFinish(int res_idx)
        {
            for (int i = 1; i <= m_needdownloadinfo.Count; i++)
                if ((i != res_idx) && !m_download_finish_index.ContainsKey(i))
                    return false;
            return true;
        }
        int AddDownloadConn(int order, int n_end)
        {
            while (order <= n_end)
            {
                var conn_idx = GetConnIdx();
                if (conn_idx != null)
                {
                    if (!m_download_finish_index.TryGetValue(order, out var res) || res == false)
                    {
                        pool_conn[(int)conn_idx] = new PoolConn { idx = order, asyncOp = DownloadResinfo(order) };

                    }
                    order = order + 1;
                }
                else break;
            }
            return order;
        }

        int GetDownConnCount()
        {
            var num = 0;
            foreach (var item in pool_conn)
            {
                if (!item.Value.dirty)
                    num++;
            }
            return num;

        }

        int? GetConnIdx()
        {
            for (int i = 0; i < conn_num; i++)
            {
                if (!pool_conn.ContainsKey(i))
                    return i;
            }
            return null;
        }

        DownloadAssetBundleAsyncOperation DownloadResinfo(int order)
        {
            var downinfo = m_needdownloadinfo[order-1];
            var url = string.Format("{0}/{1}", m_rescdn_url, downinfo.name);
            Log.Info("download ab ============, " + order + " = " + url);
            var asyncOp = ABDownload.Instance.DownloadAssetBundle(url, downinfo.hash);
            return asyncOp;
        }

        bool CheckNeedContinueDownload()
        {
            if (m_needdownloadinfo.Count > 0 && m_download_finish_index.Count == m_needdownloadinfo.Count) return false;
            for (int i = 1; i <= m_needdownloadinfo.Count; i++)
            {
                if (!m_download_finish_index.ContainsKey(i))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
