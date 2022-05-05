using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BestHTTP;
using UnityEngine;
using System.Threading;
namespace ET
{
    [FriendClass(typeof(DownloadComponent))]
    public static class DownloadComponentSystem
    {
        [ObjectSystem]
        public class DownloadComponentAwakeSystem : AwakeSystem<DownloadComponent>
        {
            public override void Awake(DownloadComponent self)
            {
                self.Awake();
            }
        }
        [ObjectSystem]
        public class DownloadComponentUpdateSystem : UpdateSystem<DownloadComponent>
        {
            public override void Update(DownloadComponent self)
            {
                self.Update();
            }
        }
        [ObjectSystem]
        public class DownloadComponentDestroySystem : DestroySystem<DownloadComponent>
        {
            public override void Destroy(DownloadComponent self)
            {
                self.Destroy();
            }
        }
        
        public static void Awake(this DownloadComponent self,int packageLength = 1000000,int maxCount = 5,int timeOut = 20, int maxTryCount = 3)
        {
            if (!DownloadComponent.isInit)
            {
                HTTPUpdateDelegator.IsThreaded = true;
                HTTPManager.Setup();
                DownloadComponent.isInit = true;
            }
            self.packageLength = packageLength;
            self.maxCount = maxCount;
            self.timeOut = timeOut;
            self.maxTryCount = maxTryCount;
            self.StandByTasks = new LinkedList<DownloadComponent.DownloadTask>();
            self.WriteFileTasks = new Dictionary<string, List<DownloadComponent.DownloadingTask>>();
            self.SaveFileMap = new Dictionary<string, string>();
            self.DownloadingTaskList = new List<DownloadComponent.DownloadingTask>();
            self.FileStreamMap = new Dictionary<string, FileStream>();
            self.CacheInfo = new Dictionary<string, string>();
            self.SuccessCount = 0;
            self.FailureCount = 0;
            self.ContinuousError = 0;
            self.TotalBytes = new Dictionary<string, long>();
            self.DownLoadBytes = new Dictionary<string, long>();
        }
        
        public static void AddDownloadUrl(this DownloadComponent self,string url,string savePath,bool clear = false)
        {
            if (self.SaveFileMap.ContainsKey(url))
            {
                Log.Info("url 重复添加"+url);
                return;
            }
            if (clear && File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            url = url.Replace(" ", "%20");
            self.StandByTasks.AddFirst(new DownloadComponent.DownloadTask
            {
                RequestType = DownloadComponent.RequestType.Head,
                Url = url,
            });
            self.SaveFileMap[url] = savePath;
            self.CacheInfo[url] = PlayerPrefs.GetString(url);
        }
        public static void Update(this DownloadComponent self)
        {
            if (self.ex != null)//下载出错
            {
                self.IsDownloading = false;
                self.tcs?.SetResult(false);
                self.tcs = null;
                return;
            }
            if (self.IsDownloading) return;//等待子线程完成
            
            if (self.SuccessCount + self.FailureCount >= self.SaveFileMap.Count)
            {
                self.IsDownloading = false;
                self.tcs?.SetResult(true);
                self.tcs = null;
                return;
            }
        }
        public static void UpdateThread(this DownloadComponent self)
        {
            try
            {
                if (!self.IsDownloading) return;
                if (self.SuccessCount + self.FailureCount >= self.SaveFileMap.Count)//下载完成
                {
                    self. IsDownloading = false;
                    return;
                }
                //开始
                while (self.StandByTasks.Count > 0 && self.DownloadingTaskList.Count < self.maxCount)
                {
                    var task = self.StandByTasks.First.Value;
                    self.StandByTasks.RemoveFirst();
                    var res = self.StartDownload(task);
                    self.DownloadingTaskList.Add(res);
                }

                for (int i = self.DownloadingTaskList.Count - 1; i >= 0; i--)
                {
                    if (self.DownloadingTaskList[i].State == DownloadComponent.DownloadingTaskState.DownloadOver)
                    {
                        self.DownloadOver(self.DownloadingTaskList[i]);
                        self.DownloadingTaskList.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                self.ex = ex;
                Log.Error(ex);
            }
        }

        public static ETTask<bool> DownloadAll(this DownloadComponent self)
        {
            if (self.tcs == null)
            {
                self.tcs = ETTask<bool>.Create(true);
                self.IsDownloading = true;
                ThreadPool.QueueUserWorkItem((_) =>
                {
                    while (true)
                    {
                        if (!self.IsDownloading) return;
                        HTTPManager.OnUpdate();
                        self.UpdateThread();
                        Thread.Sleep(1);
                    }
                });
            }
            return self.tcs;
        }
        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static DownloadComponent.DownloadingTask StartDownload(this DownloadComponent self,DownloadComponent.DownloadTask task)
        {
            DownloadComponent.DownloadingTask res = new DownloadComponent.DownloadingTask();
            res.Task = task;
            res.State = DownloadComponent.DownloadingTaskState.Downloading;
            if (task.RequestType == DownloadComponent.RequestType.Head || task.RequestType == DownloadComponent.RequestType.CheckHead)
            {
                res.HttpRequest = new HTTPRequest(new Uri(task.Url), HTTPMethods.Head, (a, b) =>
                {
                    res.State = DownloadComponent.DownloadingTaskState.DownloadOver;
                });
                res.HttpRequest.Timeout = TimeSpan.FromSeconds(self.timeOut - 1);
            }
            else
            {
                res.HttpRequest = new HTTPRequest(new Uri(task.Url), (a, b) =>
                {
                    res.State = DownloadComponent.DownloadingTaskState.DownloadOver;
                });
                res.HttpRequest.SetHeader("Range", $"bytes={task.Start}-{task.End}");
                res.HttpRequest.Timeout = TimeSpan.FromSeconds(self.timeOut - 1);
            }
            res.HttpRequest.Send();
            return res;
        }
        /// <summary>
        /// 下载完成
        /// </summary>
        /// <param name="task"></param>
        public static void DownloadOver(this DownloadComponent self,DownloadComponent.DownloadingTask task)
        {
            if(task.Task.RequestType == DownloadComponent.RequestType.Head|| task.Task.RequestType == DownloadComponent.RequestType.CheckHead)
            {
                if(task.HttpRequest.State!= HTTPRequestStates.Finished) // 下载失败
                {
                    task.HttpRequest.Dispose();
                    self.ContinuousError++;
                    if (self.ContinuousError >= self.maxTryCount) throw new Exception("下载连续失败多次");
                    var standBy = task.Task;
                    standBy.TryCount++;
                    if (standBy.TryCount >= self.maxTryCount) throw new Exception("同一个文件下载连续失败多次");
                    self.StandByTasks.AddFirst(standBy);
                    return;
                }
                self.ContinuousError = 0;
                long totalBytes = long.Parse(task.HttpRequest.Response.GetFirstHeaderValue("Content-Length"));

                string modifiedTime = task.HttpRequest.Response.GetFirstHeaderValue("Last-Modified");

                #region Check Local File

                //打开或创建
                var fileStream = new FileStream(self.SaveFileMap[task.Task.Url], FileMode.OpenOrCreate, FileAccess.Write);
                //获取已下载长度
                long byteWrites = fileStream.Length;
                self.DownLoadBytes[task.Task.Url] = byteWrites;
                self.TotalBytes[task.Task.Url] = totalBytes;
                //通过本地缓存的服务器文件修改时间和文件总长度检测服务器是否是同一个文件 不是同一个从头开始写入
                if (!self.CheckSameFile(task.Task.Url, modifiedTime, totalBytes))
                {
                    byteWrites = 0;
                }

                Log.Info($"byteWrites: {byteWrites}");
                if (byteWrites == totalBytes)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                    self.SuccessCount++;
                    Log.Info("已经下载完成");
                    return;
                }
                else if(task.Task.RequestType == DownloadComponent.RequestType.CheckHead)//下完再检测已经变化了
                {
                    fileStream.Close();
                    fileStream.Dispose();
                    self.FailureCount++;
                    File.Delete(self.SaveFileMap[task.Task.Url]);
                    Log.Error("下载后检测源文件已变动");
                    return;
                }

                //设置开始写入位置
                fileStream.Seek(byteWrites, SeekOrigin.Begin);

                self.FileStreamMap.Add(task.Task.Url, fileStream);
                #endregion

                #region Download File Data

                int i = 0;
                while (true)
                {
                    long start = byteWrites + i * self.packageLength;
                    long end = byteWrites + (i + 1) * self.packageLength - 1;
                    if (end > totalBytes)
                    {
                        end = totalBytes - 1;
                    }

                    DownloadComponent.DownloadTask downloadTask = new DownloadComponent.DownloadTask();
                    downloadTask.Url = task.Task.Url;
                    downloadTask.Start = start;
                    downloadTask.End = end;
                    downloadTask.RequestType = DownloadComponent.RequestType.Data;
                    downloadTask.Index = i;
                    downloadTask.IsLast = false;
                    self.StandByTasks.AddLast(downloadTask);
                    if (end == totalBytes - 1)
                    {
                        downloadTask.IsLast = true;
                        break;
                    }
                    i++;
                }

                #endregion
            }
            else
            {
                if (task.HttpRequest.State != HTTPRequestStates.Finished) // 下载失败
                {
                    task.HttpRequest.Dispose();
                    self.ContinuousError++;
                    if (self.ContinuousError >= self.maxTryCount) throw new Exception("下载连续失败多次");
                    var standBy = task.Task;
                    standBy.TryCount++;
                    if (standBy.TryCount >= self.maxTryCount) throw new Exception("同一个文件下载连续失败多次");
                    self.StandByTasks.AddFirst(standBy);
                    return;
                }
                if (!self.WriteFileTasks.ContainsKey(task.Task.Url))
                {
                    self.WriteFileTasks.Add(task.Task.Url, new List<DownloadComponent.DownloadingTask>());
                }
                self.DownLoadBytes[task.Task.Url] += task.Task.End-task.Task.Start;
                task.State = DownloadComponent.DownloadingTaskState.WaitWrite;
                self.WriteFileTasks[task.Task.Url].Add(task);
                self.CheckWriteFile(task.Task.Url);
            }
        }
        //检测是不是同一个文件
        public static bool CheckSameFile(this DownloadComponent self,string url, string modifiedTime,long totalBytes)
        {
            string cacheValue = self.CacheInfo[url];
            string currentValue = totalBytes + "|" + modifiedTime;
            if (cacheValue == currentValue)
            {
                return true;
            }
            self.CacheInfo[url] = currentValue;
            Log.Info($"断点续传下载一个新的文件:{url} cacheValue:{cacheValue} currentValue:{currentValue}");
            return false;
        }

        public static void CheckWriteFile(this DownloadComponent self,string url)
        {
            var taskList = self.WriteFileTasks[url];
            taskList.Sort((a, b) =>
            {
                return a.Task.Index - b.Task.Index;
            });
            int last = -1;
            for (int i = 0; i < taskList.Count; i++)
            {
                var task = taskList[i];
                last++;
                
                if(task.Task.Index!= last)
                {
                    return;
                }
                //从第一个开始顺序写入
                if(task.State== DownloadComponent.DownloadingTaskState.WaitWrite)
                {
                    self.WritePackage(task);
                }
                
            }
        }
        static void WritePackage(this DownloadComponent self,DownloadComponent.DownloadingTask task)
        {
            task.State = DownloadComponent.DownloadingTaskState.Over;
            byte[] buff = task.HttpRequest.Response.Data;
            if (buff != null && buff.Length > 0)
            {
                self.FileStreamMap[task.Task.Url].Write(buff, 0, buff.Length);
            }
            task.HttpRequest.Dispose();
            task.HttpRequest = null;
            Log.Info($"write file Length:{buff.Length}");

            if (task.Task.IsLast)
            {
                self.FileStreamMap[task.Task.Url].Close();
                self.FileStreamMap[task.Task.Url].Dispose();
                self.FileStreamMap[task.Task.Url] = null;
                Log.Info($"Download Over:{task.Task.Url}");
                DownloadComponent.DownloadTask checktask = new DownloadComponent.DownloadTask
                {
                    Url = task.Task.Url,
                    RequestType = DownloadComponent.RequestType.CheckHead,
                };
                self.StandByTasks.AddLast(checktask);
            }
        }

        public static long GetTotalBytes(this DownloadComponent self)
        {
            long res = 0;
            foreach (var item in self.TotalBytes)
            {
                res += item.Value;
            }

            return res;
        }
        public static long GetDownLoadBytes(this DownloadComponent self)
        {
            long res = 0;
            foreach (var item in self.DownLoadBytes)
            {
                res += item.Value;
            }

            return res;
        }
        public static float GetProress(this DownloadComponent self)
        {
            if (self.TotalBytes.Count != self.SaveFileMap.Count) return 0;
            long temp = 0;
            for (int i = 0; i < self.DownloadingTaskList.Count; i++)
            {
                temp += self.DownloadingTaskList[i].HttpRequest.Downloaded;
            }
            return (float)(temp+self.GetDownLoadBytes())/self.GetTotalBytes();
        }
        public static void Destroy(this DownloadComponent self)
        {
            foreach (var item in self.CacheInfo)
            {
                PlayerPrefs.SetString(item.Key, item.Value);
            }
            PlayerPrefs.Save();
            self.CacheInfo.Clear();
            self.CacheInfo = null;
            Log.Info("Dispose");
            self.IsDownloading = false;
            foreach (var item in self.FileStreamMap)
            {
                if (item.Value != null)
                {
                    item.Value.Close();
                    item.Value.Dispose();
                }
            }
            self.FileStreamMap.Clear();
            self.FileStreamMap = null;
            self.StandByTasks .Clear();
            self.WriteFileTasks.Clear();
            self.SaveFileMap.Clear();
            self.DownloadingTaskList.Clear();
            self.SuccessCount = 0;
            self.FailureCount = 0;
            self.ContinuousError = 0;
            self.TotalBytes.Clear();
            self.DownLoadBytes.Clear();
        }
    }
}