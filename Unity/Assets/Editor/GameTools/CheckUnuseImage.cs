using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics;

public class CheckUnuseImage : EditorWindow
{

    static List<string> Results = new List<string>();
    public delegate List<string> ThreadRun(ThreadPars par);
    private static EditorApplication.CallbackFunction _updateDelegate;
    private Vector2 scrollPosition = Vector2.zero;
    private const int ThreadCount = 4;
    private string curSelectTextKey = "";

    private Stopwatch watch = new Stopwatch();

    private bool isDone = false;
    public class ThreadPars
    {
        public List<string> assetContents = new List<string>();
        public List<string> luaContents = new List<string>();
        public List<string> imagePaths = new List<string>();
        public List<string> imageGUIDPaths = new List<string>();
    }

    private static List<string> ThreadFind(ThreadPars par)
    {
        if (par == null) return null;
        List<string> ret = new List<string>();
        Dictionary<string,string> assetContents = new Dictionary<string,string>();
        Dictionary<string, string> luaContents = new Dictionary<string, string>();

        for (int i = 0; i < par.imagePaths.Count; i++)
        {
            bool isHas = false;
            //查找资源是否有引用
            string guid = par.imageGUIDPaths[i];

            foreach (var fileContent in par.assetContents)
            {
                if (Regex.IsMatch(fileContent, guid))
                {
                    isHas = true;
                    break;
                }
            }

            if (isHas) continue;

            //查找lua
            string matchStr = GetMatchImagePath(par.imagePaths[i]);
            //UnityEngine.Debug.Log(matchStr);
            foreach (var fileContent in par.luaContents)
            {
                if (Regex.IsMatch(fileContent, matchStr))
                {
                    isHas = true;
                    break;
                }
            }

            if (!isHas)
                ret.Add(par.imagePaths[i]);
        }

        return ret;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("开始"))
        {
            isDone = false;
            Results.Clear();
            watch.Reset();
            watch.Start();
            this.StartCheck();
        }

        if (isDone)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < Results.Count; i++)
            {
                DrawImageItem(Results[i]);
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawImageItem(string title)
    {

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Image:", GUILayout.Width(60));
        Texture image = AssetDatabase.LoadAssetAtPath<Texture>(title);
        EditorGUILayout.ObjectField((UnityEngine.Object)image, typeof(Texture), false);
        GUILayout.TextField(title, GUILayout.Width(600));
        EditorGUILayout.EndHorizontal();
    }

    private void StartCheck()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        AssetDatabase.Refresh();

        List<string> imagePaths = UIAssetUtils.GetAllImages(false);
        imagePaths = imagePaths.Where(s => !Regex.IsMatch(s, "ColorPokerCard")).Where(s => !Regex.IsMatch(s, "PokerCard")).ToList();

        //for (int i = 0; i < imagePaths.Count; i++)
        //{
        //    Debug.LogError(GetMatchImagePath(imagePaths[i]));
        //}
        //return;

        List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat" };
        string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "AssetsPackage"), "*.*", SearchOption.AllDirectories)
            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();

        //预先都内容
        List<string> assetContents = new List<string>();
        List<string> luaContents = new List<string>();
        foreach (var file in files)
        {
            assetContents.Add(File.ReadAllText(file));
        }


        ThreadPars[] threadParses = new ThreadPars[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)//添加查找的范围内容
        {
            threadParses[i] = new ThreadPars();
            threadParses[i].assetContents = assetContents;
            threadParses[i].luaContents = luaContents;
        }

        for (int i = 0; i < imagePaths.Count; i++)
        {
            int index = i % ThreadCount;
            threadParses[index].imagePaths.Add(imagePaths[i]);
            threadParses[index].imageGUIDPaths.Add(AssetDatabase.AssetPathToGUID(imagePaths[i]));
        }

        ThreadRun[] tRun = new ThreadRun[ThreadCount];
        int finishedState = ThreadCount;

        IAsyncResult[] results = new IAsyncResult[ThreadCount];

        _updateDelegate = delegate
        {
            var finishedCount = 0;
            for (int i = 0; i < ThreadCount; i++)
            {
                if (results[i].IsCompleted) ++finishedCount;
            }

            EditorUtility.DisplayProgressBar("匹配资源中", string.Format("进度：{0}", finishedCount), (float)finishedCount/ ThreadCount);

            if (finishedCount >= finishedState)
            {

                for (int i = 0; i < ThreadCount; i++)
                {
                    List<string> temRunThreadData = tRun[i].EndInvoke(results[i]);
                    foreach (var path in temRunThreadData)
                    {
                        Results.Add(path);
                    }
                }

                EditorUtility.ClearProgressBar();
                EditorApplication.update -= _updateDelegate;
                this.ShowNotification(new GUIContent("查找完成！"));
                isDone = true;
                watch.Stop();
                UnityEngine.Debug.Log("累计用时：" + watch.ElapsedMilliseconds);
            }
        };

        for (int i = 0; i < ThreadCount; i++)
        {
            tRun[i] = ThreadFind;
            results[i] = tRun[i].BeginInvoke(threadParses[i], null, null);
        }

        EditorApplication.update += _updateDelegate;
    }

    private static string GetMatchImagePath(string path)
    {
        string str1 = path.Substring(0, path.Length - 4).Replace("Assets/AssetsPackage/", "");
        int index = str1.LastIndexOf('/');
        string filename = str1.Substring(index + 1, str1.Length - index - 1);
        int num;
        if (int.TryParse(filename, out num))
        {
            return str1.Replace("/" + filename, "");
        }

        int _index = str1.LastIndexOf('_');
        if (_index == -1)
        {
            return str1;
        }
        else
        {
            string _filename = str1.Substring(_index + 1, str1.Length - _index - 1);
            if (int.TryParse(_filename, out num))
            {
                return str1.Substring(0, str1.LastIndexOf('_') + 1);
            }
            else
            {
                return str1;
            }
        }
    }
}
