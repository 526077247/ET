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
    public class DownloadComponent:Entity,IAwake,IDestroy,IUpdate
    {
        static bool isInit = false;
        //每个下载包长度
        private int packageLength = 1000000; //1M

        //同时开启任务最大个数
        private int maxCount = 5;

        //超时时间
        private int timeOut = 20;

        //最多连续错误次数和单个文件错误次数
        private int maxTryCount = 3;

        //请求资源类型
        public class RequestType
        {
            public const int None = 0; //未开始
            public const int Head = 1; //文件头
            public const int Data = 2; //文件数据
            public const int CheckHead = 3; //检查文件头
        }
        public class DownloadingTaskState
        {
            public const int StandBy = 0; //未开始
            public const int Downloading = 1; //下载中
            public const int DownloadOver = 2; //下载完毕
            public const int WaitWrite = 3; //等待写入
            public const int Over = 4; //完成
        }
        public void Awake(int packageLength = 1000000,int maxCount = 5,int timeOut = 20, int maxTryCount = 3)
        {
            if (!isInit)
            {
                HTTPUpdateDelegator.IsThreaded = true;
                HTTPManager.Setup();
                isInit = true;
            }
            this.packageLength = packageLength;
            this.maxCount = maxCount;
            this.timeOut = timeOut;
            this.maxTryCount = maxTryCount;
            StandByTasks = new LinkedList<DownloadTask>();
            WriteFileTasks = new Dictionary<string, List<DownloadingTask>>();
            SaveFileMap = new Dictionary<string, string>();
            DownloadingTaskList = new List<DownloadingTask>();
            FileStreamMap = new Dictionary<string, FileStream>();
            CacheInfo = new Dictionary<string, string>();
            SuccessCount = 0;
            FailureCount = 0;
            ContinuousError = 0;
            TotalBytes = new Dictionary<string, long>();
            DownLoadBytes = new Dictionary<string, long>();
        }
        
        public class DownloadTask
        {
            /// <summary>
            /// 请求类型
            /// </summary>
            public int RequestType;
            /// <summary>
            /// url
            /// </summary>
            public string Url;
            /// <summary>
            /// 开始位置
            /// </summary>
            public long Start;
            /// <summary>
            /// 结束位置
            /// </summary>
            public long End;
            /// <summary>
            /// 分包序号
            /// </summary>
            public int Index;
            /// <summary>
            /// 是否最后一个
            /// </summary>
            public bool IsLast;
            /// <summary>
            /// 重试次数
            /// </summary>
            public int TryCount;
        }
        public class DownloadingTask
        {
            /// <summary>
            /// 任务信息
            /// </summary>
            public DownloadTask Task;
            /// <summary>
            /// 下载请求
            /// </summary>
            public HTTPRequest HttpRequest;
            /// <summary>
            /// 是否完成
            /// </summary>
            public int State;
            
        }

        /// <summary>
        /// 待下载任务
        /// </summary>
        public LinkedList<DownloadTask> StandByTasks;
        /// <summary>
        /// 待写入任务
        /// </summary>
        public Dictionary<string, List<DownloadingTask>> WriteFileTasks;
        /// <summary>
        /// 保存url对应路径
        /// </summary>
        public Dictionary<string, string> SaveFileMap;
        /// <summary>
        /// 正在下载队列
        /// </summary>
        public List<DownloadingTask> DownloadingTaskList;
        /// <summary>
        /// 是否在下载中
        /// </summary>
        public bool IsDownloading;
        /// <summary>
        /// 正在下载队列
        /// </summary>
        public Dictionary<string,FileStream> FileStreamMap;
        /// <summary>
        /// 缓存信息
        /// </summary>
        public Dictionary<string, string> CacheInfo;
        /// <summary>
        /// 总字节数
        /// </summary>
        public Dictionary<string, long> TotalBytes;
        /// <summary>
        /// 总字节数
        /// </summary>
        public Dictionary<string, long> DownLoadBytes;
        public Exception ex;

        public ETTask<bool> tcs;

        public int SuccessCount = 0;
        public int FailureCount = 0;

        public int ContinuousError = 0;
        public void AddDownloadUrl(string url,string savePath,bool clear = false)
        {
            if (SaveFileMap.ContainsKey(url))
            {
                Log.Info("url 重复添加"+url);
                return;
            }
            if (clear && File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            url = url.Replace(" ", "%20");
            StandByTasks.AddFirst(new DownloadTask
            {
                RequestType = RequestType.Head,
                Url = url,
            });
            SaveFileMap[url] = savePath;
            CacheInfo[url] = PlayerPrefs.GetString(url);
        }
        public void Update()
        {
            if (ex != null)//下载出错
            {
                IsDownloading = false;
                tcs?.SetResult(false);
                tcs = null;
                return;
            }
            if (IsDownloading) return;//等待子线程完成
            
            if (SuccessCount + FailureCount >= SaveFileMap.Count)
            {
                IsDownloading = false;
                tcs?.SetResult(true);
                tcs = null;
                return;
            }
        }
        public void UpdateThread()
        {
            try
            {
                if (!IsDownloading) return;
                if (SuccessCount + FailureCount >= SaveFileMap.Count)//下载完成
                {
                    IsDownloading = false;
                    return;
                }
                //开始
                while (StandByTasks.Count > 0 && DownloadingTaskList.Count < maxCount)
                {
                    var task = StandByTasks.First.Value;
                    StandByTasks.RemoveFirst();
                    var res = StartDownload(task);
                    DownloadingTaskList.Add(res);
                }

                for (int i = DownloadingTaskList.Count - 1; i >= 0; i--)
                {
                    if (DownloadingTaskList[i].State == DownloadingTaskState.DownloadOver)
                    {
                        DownloadOver(DownloadingTaskList[i]);
                        DownloadingTaskList.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ex = ex;
                Log.Error(ex);
            }
        }

        public ETTask<bool> DownloadAll()
        {
            if (tcs == null)
            {
                tcs = ETTask<bool>.Create(true);
                IsDownloading = true;
                ThreadPool.QueueUserWorkItem((_) =>
                {
                    while (true)
                    {
                        if (!IsDownloading) return;
                        HTTPManager.OnUpdate();
                        UpdateThread();
                        Thread.Sleep(1);
                    }
                });
            }
            return tcs;
        }
        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public DownloadingTask StartDownload(DownloadTask task)
        {
            DownloadingTask res = new DownloadingTask();
            res.Task = task;
            res.State = DownloadingTaskState.Downloading;
            if (task.RequestType == RequestType.Head || task.RequestType == RequestType.CheckHead)
            {
                res.HttpRequest = new HTTPRequest(new Uri(task.Url), HTTPMethods.Head, (a, b) =>
                {
                    res.State = DownloadingTaskState.DownloadOver;
                });
                res.HttpRequest.Timeout = TimeSpan.FromSeconds(timeOut - 1);
            }
            else
            {
                res.HttpRequest = new HTTPRequest(new Uri(task.Url), (a, b) =>
                {
                    res.State = DownloadingTaskState.DownloadOver;
                });
                res.HttpRequest.SetHeader("Range", $"bytes={task.Start}-{task.End}");
                res.HttpRequest.Timeout = TimeSpan.FromSeconds(timeOut - 1);
            }
            res.HttpRequest.Send();
            return res;
        }
        /// <summary>
        /// 下载完成
        /// </summary>
        /// <param name="task"></param>
        public void DownloadOver(DownloadingTask task)
        {
            if(task.Task.RequestType == RequestType.Head|| task.Task.RequestType == RequestType.CheckHead)
            {
                if(task.HttpRequest.State!= HTTPRequestStates.Finished) // 下载失败
                {
                    task.HttpRequest.Dispose();
                    ContinuousError++;
                    if (ContinuousError >= this.maxTryCount) throw new Exception("下载连续失败多次");
                    var standBy = task.Task;
                    standBy.TryCount++;
                    if (standBy.TryCount >= this.maxTryCount) throw new Exception("同一个文件下载连续失败多次");
                    StandByTasks.AddFirst(standBy);
                    return;
                }
                ContinuousError = 0;
                long totalBytes = long.Parse(task.HttpRequest.Response.GetFirstHeaderValue("Content-Length"));

                string modifiedTime = task.HttpRequest.Response.GetFirstHeaderValue("Last-Modified");

                #region Check Local File

                //打开或创建
                var fileStream = new FileStream(SaveFileMap[task.Task.Url], FileMode.OpenOrCreate, FileAccess.Write);
                //获取已下载长度
                long byteWrites = fileStream.Length;
                DownLoadBytes[task.Task.Url] = byteWrites;
                TotalBytes[task.Task.Url] = totalBytes;
                //通过本地缓存的服务器文件修改时间和文件总长度检测服务器是否是同一个文件 不是同一个从头开始写入
                if (!CheckSameFile(task.Task.Url, modifiedTime, totalBytes))
                {
                    byteWrites = 0;
                }

                Log.Info($"byteWrites: {byteWrites}");
                if (byteWrites == totalBytes)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                    SuccessCount++;
                    Log.Info("已经下载完成");
                    return;
                }
                else if(task.Task.RequestType == RequestType.CheckHead)//下完再检测已经变化了
                {
                    fileStream.Close();
                    fileStream.Dispose();
                    FailureCount++;
                    File.Delete(SaveFileMap[task.Task.Url]);
                    Log.Error("下载后检测源文件已变动");
                    return;
                }

                //设置开始写入位置
                fileStream.Seek(byteWrites, SeekOrigin.Begin);

                FileStreamMap.Add(task.Task.Url, fileStream);
                #endregion

                #region Download File Data

                int i = 0;
                while (true)
                {
                    long start = byteWrites + i * packageLength;
                    long end = byteWrites + (i + 1) * packageLength - 1;
                    if (end > totalBytes)
                    {
                        end = totalBytes - 1;
                    }

                    DownloadTask downloadTask = new DownloadTask();
                    downloadTask.Url = task.Task.Url;
                    downloadTask.Start = start;
                    downloadTask.End = end;
                    downloadTask.RequestType = RequestType.Data;
                    downloadTask.Index = i;
                    downloadTask.IsLast = false;
                    StandByTasks.AddLast(downloadTask);
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
                    ContinuousError++;
                    if (ContinuousError >= this.maxTryCount) throw new Exception("下载连续失败多次");
                    var standBy = task.Task;
                    standBy.TryCount++;
                    if (standBy.TryCount >= this.maxTryCount) throw new Exception("同一个文件下载连续失败多次");
                    StandByTasks.AddFirst(standBy);
                    return;
                }
                if (!WriteFileTasks.ContainsKey(task.Task.Url))
                {
                    WriteFileTasks.Add(task.Task.Url, new List<DownloadingTask>());
                }
                DownLoadBytes[task.Task.Url] += task.Task.End-task.Task.Start;
                task.State = DownloadingTaskState.WaitWrite;
                WriteFileTasks[task.Task.Url].Add(task);
                CheckWriteFile(task.Task.Url);
            }
        }
        //检测是不是同一个文件
        public bool CheckSameFile(string url, string modifiedTime,long totalBytes)
        {
            string cacheValue = CacheInfo[url];
            string currentValue = totalBytes + "|" + modifiedTime;
            if (cacheValue == currentValue)
            {
                return true;
            }
            CacheInfo[url] = currentValue;
            Log.Info($"断点续传下载一个新的文件:{url} cacheValue:{cacheValue} currentValue:{currentValue}");
            return false;
        }

        public void CheckWriteFile(string url)
        {
            var taskList = WriteFileTasks[url];
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
                if(task.State== DownloadingTaskState.WaitWrite)
                {
                    WritePackage(task);
                }
                
            }
        }
        void WritePackage(DownloadingTask task)
        {
            task.State = DownloadingTaskState.Over;
            byte[] buff = task.HttpRequest.Response.Data;
            if (buff != null && buff.Length > 0)
            {
                FileStreamMap[task.Task.Url].Write(buff, 0, buff.Length);
            }
            task.HttpRequest.Dispose();
            task.HttpRequest = null;
            Log.Info($"write file Length:{buff.Length}");

            if (task.Task.IsLast)
            {
                FileStreamMap[task.Task.Url].Close();
                FileStreamMap[task.Task.Url].Dispose();
                FileStreamMap[task.Task.Url] = null;
                Log.Info($"Download Over:{task.Task.Url}");
                DownloadTask checktask = new DownloadTask
                {
                    Url = task.Task.Url,
                    RequestType = RequestType.CheckHead,
                };
                StandByTasks.AddLast(checktask);
            }
        }

        public long GetTotalBytes()
        {
            long res = 0;
            foreach (var item in TotalBytes)
            {
                res += item.Value;
            }

            return res;
        }
        public long GetDownLoadBytes()
        {
            long res = 0;
            foreach (var item in DownLoadBytes)
            {
                res += item.Value;
            }

            return res;
        }
        public float GetProress()
        {
            if (TotalBytes.Count != SaveFileMap.Count) return 0;
            long temp = 0;
            for (int i = 0; i < DownloadingTaskList.Count; i++)
            {
                temp += DownloadingTaskList[i].HttpRequest.Downloaded;
            }
            return (float)(temp+GetDownLoadBytes())/GetTotalBytes();
        }
        public void Destroy()
        {
            foreach (var item in CacheInfo)
            {
                PlayerPrefs.SetString(item.Key, item.Value);
            }
            PlayerPrefs.Save();
            CacheInfo.Clear();
            CacheInfo = null;
            Log.Info("Dispose");
            IsDownloading = false;
            foreach (var item in FileStreamMap)
            {
                if (item.Value != null)
                {
                    item.Value.Close();
                    item.Value.Dispose();
                }
            }
            FileStreamMap.Clear();
            FileStreamMap = null;
            StandByTasks .Clear();
            WriteFileTasks.Clear();
            SaveFileMap.Clear();
            DownloadingTaskList.Clear();
            SuccessCount = 0;
            FailureCount = 0;
            ContinuousError = 0;
            TotalBytes.Clear();
            DownLoadBytes.Clear();
        }
    }
}
