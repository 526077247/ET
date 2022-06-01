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
    [ComponentOf]
    public class DownloadComponent:Entity,IAwake,IDestroy,IUpdate
    {
        public static bool isInit = false;
        //每个下载包长度
        public int packageLength = 1000000; //1M

        //同时开启任务最大个数
        public int maxCount = 5;

        //超时时间
        public int timeOut = 20;

        //最多连续错误次数和单个文件错误次数
        public int maxTryCount = 3;

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
        
    }
}
