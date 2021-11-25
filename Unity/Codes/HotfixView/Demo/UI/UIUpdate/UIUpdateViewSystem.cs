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
    public class UIUpdateViewOnCreateSystem : OnCreateSystem<UIUpdateView>
    {
        public override void OnCreate(UIUpdateView self)
        {
            self.m_slider = self.AddComponent<UISlider>("Loadingscreen/Slider");
        }
    }
    [UISystem]
    public class UIUpdateViewOnEnableSystem : OnEnableSystem<UIUpdateView>
    {
        public override void OnEnable(UIUpdateView self)
        {
            self.last_progress = 0;
            self.m_slider.SetValue(0);
            self.StartCheckUpdate().Coroutine();
        }
    }
    public static class UIUpdateViewSystem
    {

        private static async ETTask UpdateFinishAndStartGame(this UIUpdateView self)
        {
            Game.Scene.RemoveAllComponent();
            // ������Դ������
            while (AddressablesManager.Instance.IsProsessRunning)
            {
                await TimerComponent.Instance.WaitAsync(1);
            }
            AddressablesManager.Instance.ClearAssetsCache();

            //���¼�������
            AssetBundleConfig.Instance.SyncLoadGlobalAssetBundle();

            //���޸�
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
            I18NComponent.Instance.I18NTryGetText(content, out content);
            I18NComponent.Instance.I18NTryGetText(confirmBtnText, out confirmBtnText);
            I18NComponent.Instance.I18NTryGetText(cancelBtnText, out cancelBtnText);
            self.para.Content = content;
            self.para.ConfirmCallback = confirmBtnFunc;
            self.para.ConfirmText = confirmBtnText;
            self.para.CancelText = cancelBtnText;
            self.para.CancelCallback = cancelBtnFunc;
            await UIManagerComponent.Instance.OpenWindow<UIMsgBoxWin, UIMsgBoxWin.MsgBoxPara>(self.para);
            var result = await tcs;
            await UIManagerComponent.Instance.CloseWindow<UIMsgBoxWin>();
            return result;
        }
        public static async ETVoid StartCheckUpdate(this UIUpdateView self)
        {
            //TODO ������ 
            await self.CheckIsInWhiteList();

            await self.CheckUpdateList();

            var Over = await self.CheckAppUpdate();
            if (Over) return;

            var isUpdateDone = await self.CheckResUpdate();
            if (isUpdateDone)
            {
                Log.Info("������ɣ�׼��������Ϸ");
                self.UpdateFinishAndStartGame().Coroutine();
            }
            else
            {
                Log.Info("����Ҫ���£�ֱ�ӽ�����Ϸ");
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
                    var btnState = await self.ShowMsgBoxView("Update_White", "Global_Btn_Confirm", "Global_Btn_Cancel");
                    if (btnState == self.BTN_CONFIRM)
                    {
                        BootConfig.Instance.SetWhiteMode(true);
                    }
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
            var info = await HttpManager.Instance.HttpGetResult<UpdateConfig>(url);
            if (info == null)
            {
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", "Btn_Exit");
                if (btnState == self.BTN_CONFIRM)
                {
                    await self.CheckUpdateList();
                }
                else
                {
                    GameUtility.Quit();
                    return;
                }
            }
            else
            {
                BootConfig.Instance.SetUpdateList(info);
            }
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

            var force_update = true;//Ĭ��ǿ��
            if (verInfo != null && verInfo.force_update == 0)
                force_update = false;

            var cancelBtnText = force_update ? "Btn_Exit" : "Btn_Enter_Game";
            var content_updata = force_update ? "Update_ReDownload" : "Update_SuDownload";
            var btnState = await self.ShowMsgBoxView(content_updata, "Global_Btn_Confirm", cancelBtnText);

            if (btnState == self.BTN_CONFIRM)
            {
                GameUtility.OpenURL(app_url);
                //Ϊ�˷�ֹ�л�����ҳ�������������Ϸ������������Ҫ�������������
                return await self.CheckAppUpdate();
            }
            else if(force_update)
            {
                Log.Info("CheckAppUpdate Need Force Update And User Choose Exit Game!");
                GameUtility.Quit();
                return true;
            }
            return false;
        }

        //��Դ���¼�飬�����ݰ汾���޸���Դcdn��ַ
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

            // �༭���²��ܲ����ȸ��������Բ������ء�
            if (Define.IsEditor) return false;

            //�ҵ����°汾�������õ�ǰ��Դ��ŵ�cdn��ַ
            var url = BootConfig.Instance.GetUpdateCdnResUrlByVersion(maxVer);
            self.m_rescdn_url = url;
            Log.Info("CheckResUpdate res_cdn_url is " + url);
            AssetBundleMgr.GetInstance().SetAddressableRemoteResCdnUrl(self.m_rescdn_url);
            //��һ֡��addressables��Update�ص�ִ����
            await TimerComponent.Instance.WaitAsync(1);
            //�����°汾
            Log.Info("begin  CheckCatalogUpdates");
            var handler = await self.CheckCatalogUpdates();
            if (handler == null) return false;
            //��һ֡
            await TimerComponent.Instance.WaitAsync(1);
            var catalog = handler.catalog;
            Log.Info("CheckResUpdate CataLog = " + catalog);

            //1���ȸ���catalogs
            if (!string.IsNullOrEmpty(catalog))
            {
                Log.Info("begin  UpdateCatalogs");
                var res = await self.UpdateCatalogs(catalog);
                if (!res) return false;
                Log.Info("CoCheckResUpdate Update Catalog Success");
            }

            Log.Info("begin  GetDownloadSize");
            //��ȡ��Ҫ���µĴ�С
            handler = await self.GetDownloadSize();
            if (handler == null) return false;
            var size = handler.downloadSize;

            //��ʾ���û�
            Log.Info("downloadSize " + size);
            double size_mb = size / (1024f * 1024f);
            Log.Info("CheckResUpdate res size_mb is " + size_mb);//������
            if (size_mb > 0 && size_mb < 0.01) size_mb = 0.01;

            var ct = I18NComponent.Instance.I18NGetParamText("Update_Info",size_mb.ToString("0.00"));
            var btnState = await self.ShowMsgBoxView(ct, "Global_Btn_Confirm", "Btn_Exit");
            if (btnState == self.BTN_CANCEL)
            {
                GameUtility.Quit();
                return false;
            }

            //��ʼ���и���

            self.last_progress = 0;
            self.SetProgress(0);
            //2��������Դ

            var merge_mode_union = 1;
            handler =await AddressablesManager.Instance.CheckUpdateContent(new List<string>() { "default" }, merge_mode_union);

            var needdownloadinfo = handler.GetNeedDownloadinfo();
            Log.Info("needdownloadinfo: ", needdownloadinfo);
            self.m_needdownloadinfo = SortDownloadInfo(JsonHelper.FromJson<Dictionary<string, string>>(needdownloadinfo));

            Log.Info("CheckResUpdate DownloadContent begin");
            bool result = await self.DownloadContent(size);
            if (!result) return false;
            Log.Info("CheckResUpdate DownloadContent Success");
            return true;
        }

        static List<DownLoadInfo> SortDownloadInfo(Dictionary<string, string> needdownloadinfo)
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
            //�汾��Դ���
            if (lua_ab != null)
                temp.Add(lua_ab);
            if (global_ab != null)
                temp.Add(global_ab);
            return temp;
        }

        async static ETTask<AddressableUpdateAsyncOperation> GetDownloadSize(this UIUpdateView self)
        {
            var handler = await AddressablesManager.Instance.GetDownloadSizeAsync("default");
            if (handler.isSuccess)
                return handler;
            else
            {
                Log.Info("CoGetDownloadSize Get Download Size Async Faild");
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", "Btn_Exit");
                if (btnState == self.BTN_CONFIRM)
                    return await self.GetDownloadSize();
                else
                {
                    GameUtility.Quit();
                    return null;
                }
            }
        }

        //������Catalog
        async static ETTask<AddressableUpdateAsyncOperation> CheckCatalogUpdates(this UIUpdateView self)
        {
            var handler = await AddressablesManager.Instance.CheckForCatalogUpdates();
            if (handler.isSuccess) return handler;
            else
            {
                Log.Info("CheckCatalogUpdates Check CataLog Failed");
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", "Btn_Exit");
                if (btnState == self.BTN_CONFIRM)
                    return await self.CheckCatalogUpdates();
                else
                {
                    GameUtility.Quit();
                    return null;
                }
            }
        }

        //����catalogs
        async static ETTask<bool> UpdateCatalogs(this UIUpdateView self,string catalog)
        {
            //������������ϣ�������ΪUpdateCatalogs�ɹ�
            var handler = await AddressablesManager.Instance.CheckForCatalogUpdates();
            if (handler.isSuccess)
            {
                handler = await AddressablesManager.Instance.UpdateCatalogs(catalog);
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
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", "Btn_Exit");
                if (btnState == self.BTN_CONFIRM)
                    return await self.UpdateCatalogs(catalog);
                else
                {
                    GameUtility.Quit();
                    return false;

                }
            }
        }
        static void SetProgress(this UIUpdateView self, float value)
        {
            if(value> self.last_progress)
                self.last_progress = value;
            self.m_slider.SetValue(self.last_progress);
        }
        async static ETTask<bool> DownloadContent(this UIUpdateView self,long size)
        {
            var url = BootConfig.Instance.GetUpdateListCdnUrl();
            var info = await HttpManager.Instance.HttpGetResult(url);
            if (!string.IsNullOrEmpty(info))
            {
                await self.DownloadAllAssetBundle(size);
                return true;
            }
            else
            {
                Log.Info("DownloadContent Begin DownloadDependenciesAsync failed");
                var btnState = await self.ShowMsgBoxView("Update_Get_Fail", "Update_ReTry", "Btn_Exit");
                if (btnState == self.BTN_CONFIRM)
                    return await self.DownloadContent(size);
                else
                {
                    GameUtility.Quit();
                    return false;
                }
            }
        }

        async static ETTask DownloadAllAssetBundle(this UIUpdateView self, long size)
        {
            var downloadTool = (self as Entity).AddComponent<UnityWebRequestRenewalAsync>();
            var total_count = self.m_needdownloadinfo.Count;
            Log.Info("DownloadAllAssetBundle count = " + total_count);
            if (total_count <= 0)
                return;
            self.download_size = 0;
            self.overCount = 0;
            self.RefreshProgress(downloadTool, size).Coroutine();
            for (int i = 0; i < self.m_needdownloadinfo.Count; i++)
            {
                await self.DownloadResinfoAsync(i);
                self.download_size += downloadTool.ByteDownloaded;
            }
            Log.Info("DownloadAllAssetBundle end");

        }
        static async ETTask RefreshProgress(this UIUpdateView self, UnityWebRequestRenewalAsync downloadTool, long size)
        {
            while (self.m_needdownloadinfo.Count!= self.overCount)
            {
                self.SetProgress((float)((double)(downloadTool.ByteDownloaded + self.download_size) / size));
                await TimerComponent.Instance.WaitAsync(10);
            }
        }
        static async ETTask DownloadResinfoAsync(this UIUpdateView self, int order)
        {
            var downloadTool = (self as Entity).GetComponent<UnityWebRequestRenewalAsync>();
            var downinfo = self.m_needdownloadinfo[order];
            var url = string.Format("{0}/{1}", self.m_rescdn_url, downinfo.name);
            Log.Info("download ab ============, " + order + " = " + url);
            var savePath = AssetBundleMgr.GetInstance().getCachedAssetBundlePath(downinfo.name) + ".temp";
            for (int i = 0; i < 3; i++)//����3��
            {
                try
                {
                    await downloadTool.DownloadAsync(url, savePath);
                    downloadTool.Clear();
                    AssetBundleMgr.GetInstance().CacheAssetBundle(downinfo.name, downinfo.hash);
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            downloadTool.DeleteTempFile(savePath);
            var btnState = await self.ShowMsgBoxView("Update_Download_Fail", "Update_ReTry", "Btn_Exit");
            if (btnState == self.BTN_CONFIRM)
            {
                await self.DownloadResinfoAsync(order);
            }
            else
            {
                GameUtility.Quit();
            }
        }
    }
}
