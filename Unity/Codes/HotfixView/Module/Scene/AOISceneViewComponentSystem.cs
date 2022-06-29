using System.Collections.Generic;
using UnityEngine;
using System;

namespace ET
{
    [ObjectSystem]
    public class AOISceneViewComponentAwakeSystem: AwakeSystem<AOISceneViewComponent,int>
    {
        public override void Awake(AOISceneViewComponent self, int len)
        {
            AOISceneViewComponent.Instance = self;
            self.GridLen = len;
            self.DynamicSceneObjectMapCount = new Dictionary<AssetsObject, int>();
            self.DynamicSceneObjectMapObj = new Dictionary<AssetsObject, AOISceneViewComponent.DynamicSceneViewObj>();
            self.Init().Coroutine();
        }
        
        
    }
    [FriendClass(typeof(AOISceneViewComponent))]
    [FriendClass(typeof(SceneManagerComponent))]
    public static class AOISceneViewComponentSystem
    {
        public static async ETTask Init(this AOISceneViewComponent self)
        {
            #region 从Json初始化场景物体信息
            self.DynamicSceneMap = new Dictionary<string, AOISceneViewComponent.DynamicScene>();
            string xmlPath = "GameAssets/Config/Map.json";
            var content = await ResourcesComponent.Instance.LoadAsync<TextAsset>(xmlPath);
            AssetsRoot root = null;
            try
            {
                root= JsonHelper.FromJson<AssetsRoot>(content.text);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            
            if (root == null) return;
            foreach(var scene in root.Scenes)
            {
                AOISceneViewComponent.DynamicScene item = new AOISceneViewComponent.DynamicScene();
                item.sceneName = scene.Name;
                item.Objects = scene.Objects;
                self.DynamicSceneMap.Add(item.sceneName,item);
                item.GridMapObjects = new Dictionary<long, List<AssetsObject>>();
                for (int index = 0; index < item.Objects.Count; index++)
                {
                    var sceneObject = item.Objects[index];
                    int x = (int)Math.Floor( sceneObject.Transform.Position.x / self.GridLen);
                    int y = (int)Math.Floor( sceneObject.Transform.Position.z / self.GridLen);
                    float radius = Mathf.Sqrt(sceneObject.Size.x*sceneObject.Size.x+sceneObject.Size.y*
                        sceneObject.Size.y+sceneObject.Size.z*sceneObject.Size.z)/2;
                    int count = (int)Math.Ceiling(radius / self.GridLen);//环境多加一格
                    float cellSqrRadius = Mathf.Pow(self.GridLen, 2) * 2;
                    float cellRadius = Mathf.Sqrt(cellSqrRadius);
                    for (int i = x-count; i <= x+count; i++)
                    {
                        var xMin = i * self.GridLen;
                        for (int j = y-count; j <=y+count; j++)
                        {
                            var yMin = j* self.GridLen;
                            var res = AOIHelper.GetGridRelationshipWithOBB(sceneObject.Transform.Position, sceneObject.Transform.Rotation,
                                sceneObject.Size, self.GridLen, xMin, yMin,radius,radius*radius);
                            if (res >= 0)
                            {
                                var id = AOIHelper.CreateCellId(i, j);
                                if (!item.GridMapObjects.ContainsKey(id))
                                {
                                    item.GridMapObjects.Add(id,new List<AssetsObject>());
                                }
                                item.GridMapObjects[id].Add(sceneObject);
                            }
                        }
                    }
                }
            }
            #endregion
            
        }
        /// <summary>
        /// 切换到某个场景
        /// </summary>
        /// <param name="name"></param>
        public static async ETTask ChangeToScene(this AOISceneViewComponent self,string name=null,SceneLoadComponent slc = null)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(self.CurMap)) return;
            if (SceneManagerComponent.Instance.Busing) return;
            SceneManagerComponent.Instance.Busing = true;
            self.LastGridX = null;
            self.LastGridY = null;
            Log.Info("InnerSwitchScene start open uiloading");
            //打开loading界面
            await Game.EventSystem.PublishAsync(new UIEventType.LoadingBegin());
            float slid_value = 0;
            if (name != self.CurMap)//需要重新加载场景物体
            {
                Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });
                if (SceneManagerComponent.Instance.CurrentScene != SceneNames.Map)
                {
                    CameraManagerComponent.Instance.SetCameraStackAtLoadingStart();
                }
                slid_value = 0.1f;
                Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });
                //等待资源管理器加载任务结束，否则很多Unity版本在切场景时会有异常，甚至在真机上crash
                Log.Info("InnerSwitchScene ProsessRunning Done ");
                while (ResourcesComponent.Instance.IsProsessRunning())
                {
                    await TimerComponent.Instance.WaitAsync(1);
                }
                
                //清理旧场景
                foreach (var item in self.DynamicSceneObjectMapObj)
                {
                    if (item.Value.IsLoading)
                    {
                        item.Value.IsLoading = false;
                    }
                    else
                    {
                        GameObjectPoolComponent.Instance.RecycleGameObject(item.Value.Obj);
                    }
                }
                self.DynamicSceneObjectMapObj.Clear();
                self.DynamicSceneObjectMapCount.Clear();
                slid_value = 0.2f;
                Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });

                if (SceneManagerComponent.Instance.CurrentScene != SceneNames.Map)
                {
                    //清理UI
                    Log.Info("InnerSwitchScene Clean UI");
                    await UIManagerComponent.Instance.DestroyWindowExceptNames(SceneManagerComponent.Instance.DestroyWindowExceptNames.ToArray());
                    
                    slid_value = 0.25f;
                    Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });
                    //清除ImageLoaderManager里的资源缓存 这里考虑到我们是单场景
                    Log.Info("InnerSwitchScene ImageLoaderManager Cleanup");
                    ImageLoaderComponent.Instance.Clear();
                    //清除预设以及其创建出来的gameobject, 这里不能清除loading的资源
                    Log.Info("InnerSwitchScene GameObjectPool Cleanup");
                    
                    GameObjectPoolComponent.Instance.Cleanup(true, SceneManagerComponent.Instance.ScenesChangeIgnoreClean);
                    slid_value = 0.3f;
                    Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });
                    //清除除loading外的资源缓存 
                    List<UnityEngine.Object> gos = new List<UnityEngine.Object>();
                    for (int i = 0; i < SceneManagerComponent.Instance.ScenesChangeIgnoreClean.Count; i++)
                    {
                        var path = SceneManagerComponent.Instance.ScenesChangeIgnoreClean[i];
                        var go = GameObjectPoolComponent.Instance.GetCachedGoWithPath(path);
                        if (go != null)
                        {
                            gos.Add(go);
                        }
                    }
                    Log.Info("InnerSwitchScene ResourcesManager ClearAssetsCache excludeAssetLen = " + gos.Count);
                    ResourcesComponent.Instance.ClearAssetsCache(gos.ToArray());
                    slid_value = 0.45f;
                    Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });
                    
                    await ResourcesComponent.Instance.LoadSceneAsync(SceneManagerComponent.Instance.GetSceneConfigByName(SceneNames.Loading).SceneAddress, false);
                    Log.Info("LoadSceneAsync Over");
                    slid_value = 0.5f;
                    Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });
                    //GC：交替重复2次，清干净一点
                    GC.Collect();
                    GC.Collect();

                    var res = Resources.UnloadUnusedAssets();
                    while (!res.isDone)
                    {
                        await TimerComponent.Instance.WaitAsync(1);
                    }
                    slid_value = 0.6f;
                    Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });

                    Log.Info("异步加载目标场景 Start");
                    var scene_config = SceneManagerComponent.Instance.GetSceneConfigByName(SceneNames.Map); //固定Map空场景
                    //异步加载目标场景
                    await ResourcesComponent.Instance.LoadSceneAsync(scene_config.SceneAddress, false);
                    SceneManagerComponent.Instance.CurrentScene = scene_config.Name;
                }
                slid_value = 0.7f;
                Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });
                //准备工作：预加载资源等
                if (slc != null)
                {
                    await slc.OnPrepare((progress) =>
                    {
                        Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value + 0.3f * progress });
                        if (progress > 1) Log.Error("scene load waht's the fuck!");
                    });
                }
                slid_value = 1f;
                Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = slid_value });
                CameraManagerComponent.Instance.SetCameraStackAtLoadingDone();
            }

            Game.EventSystem.Publish(new UIEventType.LoadingProgress { Progress = 1 });
            //等久点，跳的太快
            await TimerComponent.Instance.WaitAsync(100);
            //加载完成，关闭loading界面
            await Game.EventSystem.PublishAsync(new UIEventType.LoadingFinish());
            SceneManagerComponent.Instance.Busing = false;
            self.CurMap = name;
        }

        /// <summary>
        /// 改变格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="viewLen"></param>
        public static async ETTask ChangeGrid(this AOISceneViewComponent self,int x,int y,int viewLen)
        {
            CoroutineLock coroutineLock = null;
            try
            {
                coroutineLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.AOIView, self.GetHashCode());
                while (SceneManagerComponent.Instance.Busing)
                {
                    await TimerComponent.Instance.WaitAsync(1);
                }
                if (self.LastGridX != null)
                {
                    int count = 0;
                    count += Math.Abs(x - (int) self.LastGridX);
                    count += Math.Abs(y - (int) self.LastGridY);
                    if (count > 4) //太远了走loading
                    {
                        self.ChangeToScene().Coroutine();
                    }
                }

                DictionaryComponent<long, int> temp = DictionaryComponent<long, int>.Create();
                for (int i = -viewLen; i <= viewLen; i++)
                {
                    for (int j = -viewLen; j <= viewLen; j++)
                    {
                        if (self.LastGridY != null)
                        {
                            var oldid = AOIHelper.CreateCellId((int) self.LastGridX + i, (int) self.LastGridY + j);
                            if (!temp.ContainsKey(oldid))
                            {
                                temp[oldid] = 0;
                            }

                            temp[oldid]--;
                        }

                        var newid = AOIHelper.CreateCellId(x + i, y + j);
                        if (!temp.ContainsKey(newid))
                        {
                            temp[newid] = 0;
                        }

                        temp[newid]++;


                    }
                }

                foreach (var item in temp)
                {
                    if (item.Value == 0) continue;
                    var objs = self.DynamicSceneMap[self.CurMap].GridMapObjects;
                    if (!objs.ContainsKey(item.Key)) continue;
                    for (int i = 0; i < objs[item.Key].Count; i++)
                    {
                        var obj = objs[item.Key][i];
                        if (item.Value > 0) //新增
                        {
                            if (self.DynamicSceneObjectMapCount.ContainsKey(obj))
                            {
                                self.DynamicSceneObjectMapCount[obj]++;
                            }
                            else
                            {
                                self.DynamicSceneObjectMapCount[obj] = 1;
                            }

                            //需要显示
                            if (self.DynamicSceneObjectMapCount[obj] > 0)
                            {
                                AOISceneViewComponent.DynamicSceneViewObj viewObj;
                                //已经有
                                if (self.DynamicSceneObjectMapObj.ContainsKey(obj))
                                {
                                    viewObj = self.DynamicSceneObjectMapObj[obj];
                                    if (viewObj.Obj == null) //之前有单没加载出来，IsLoading改为true，防止之前已经被改成false了
                                    {
                                        viewObj.IsLoading = true;
                                    }

                                    continue;
                                }

                                Log.Info("AOISceneView Load " + obj.PrefabPath);
                                //没有
                                self.DynamicSceneObjectMapObj[obj] = new AOISceneViewComponent.DynamicSceneViewObj();
                                viewObj = self.DynamicSceneObjectMapObj[obj];
                                viewObj.IsLoading = true;
                                GameObjectPoolComponent.Instance.GetGameObjectAsync(obj.PrefabPath, (view) =>
                                {
                                    if (!viewObj.IsLoading) //加载出来后已经不需要的
                                    {
                                        GameObjectPoolComponent.Instance.RecycleGameObject(view);
                                        self.DynamicSceneObjectMapObj.Remove(obj);
                                    }
                                    viewObj.Obj = view;
                                    viewObj.IsLoading = false;
                                    view.transform.position = obj.Transform.Position;
                                    view.transform.rotation = obj.Transform.Rotation;
                                    view.transform.localScale = obj.Transform.Scale;
                                    view.transform.parent = GlobalComponent.Instance.Scene;
                                }).Coroutine();
                            }
                        }
                        else //移除
                        {
                            if (self.DynamicSceneObjectMapCount.ContainsKey(obj))
                            {
                                self.DynamicSceneObjectMapCount[obj]--;
                            }
                            else
                            {
                                self.DynamicSceneObjectMapCount[obj] = -1;
                            }

                            //不需要显示但有
                            if (self.DynamicSceneObjectMapCount[obj] <= 0 && self.DynamicSceneObjectMapObj.ContainsKey(obj))
                            {
                                Log.Info("AOISceneView Remove " + obj.PrefabPath);
                                var viewObj = self.DynamicSceneObjectMapObj[obj];
                                if (viewObj.Obj == null) //还在加载
                                {
                                    viewObj.IsLoading = false;
                                }
                                else
                                {
                                    viewObj.Obj.SetActive(false);
                                    GameObjectPoolComponent.Instance.RecycleGameObject(viewObj.Obj);
                                    self.DynamicSceneObjectMapObj.Remove(obj);
                                }

                            }
                        }
                    }
                }

                temp.Dispose();
                self.LastGridX = x;
                self.LastGridY = y;
            }
            finally
            {
                coroutineLock?.Dispose();
            }
        }
    }
}