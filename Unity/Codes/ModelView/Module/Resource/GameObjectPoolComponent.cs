using AssetBundles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ET
{
	[ObjectSystem]
	public class GameObjectPoolComponentAwakeSystem : AwakeSystem<GameObjectPoolComponent>
	{
		public override void Awake(GameObjectPoolComponent self)
		{
			self.Awake();
		}
	}
	//--[[
	//-- GameObject缓存池
	//-- 注意：
	//-- 1、所有需要预设都从这里加载，不要直接到ResourcesManager去加载，由这里统一做缓存管理
	//-- 2、缓存分为两部分：从资源层加载的原始GameObject(Asset)，从GameObject实例化出来的多个Inst

	//	原则: 有借有还，再借不难，借出去的东西回收时，请不要污染(pc上会进行检测，发现则报错)
	//	何为污染？
	//	1、不要往里面添加什么节点，借出来的是啥样子，返回来的就是啥样子


	//	GameObject内存管理，采用lru cache来管理prefab
	//	为了对prefab和其产生的go的内存进行管理，所以严格管理go生命周期 
	//	1、创建 -> GetGameObjectAsync
	//	2、回收 -> 绝大部分的时候应该采用回收(回收go不能被污染)，对象的销毁对象池会自动管理 RecycleGameObject
	//	3、销毁 -> 如果的确需要销毁，或一些用不到的数据想要销毁，也必须从这GameObjectPool中进行销毁，
	//			  严禁直接调用GameObject.Destroy方法来进行销毁，而应该采用GameObjectPool.DestroyGameObject方法

	//	不管是销毁还是回收，都不要污染go，保证干净
	//--]]
	[ChildType]
	public class GameObjectPoolComponent : Entity,IAwake
	{
		AddressablesManager AddressablesManager;
		Transform __cacheTransRoot;
		public static GameObjectPoolComponent Instance { get; set; }
		LruCache<string, GameObject> __goPool;
		Dictionary<string, int> __goInstCountCache;//go: inst_count 用于记录go产生了多少个实例

		Dictionary<string, int> __goChildsCountPool;//path: child_count 用于在editor模式下检测回收的go是否被污染 path:num

		Dictionary<string, List<GameObject>> __instCache; //path: inst_array
		Dictionary<GameObject, string> __instPathCache;// inst : prefab_path 用于销毁和回收时反向找到inst对应的prefab TODO:这里有优化空间path太占内存
		Dictionary<string, bool> __persistentPathCache;//需要持久化的资源
		Dictionary<string, Dictionary<string, int>> __detailGoChildsCount;//记录go子控件具体数量信息
		public void Awake()
		{
			Instance = this;
			AddressablesManager = AddressablesManager.Instance;
			__goPool = new LruCache<string, GameObject>();
			__goInstCountCache = new Dictionary<string, int>();
			__goChildsCountPool = new Dictionary<string, int>();
			__instCache = new Dictionary<string, List<GameObject>>();
			__instPathCache = new Dictionary<GameObject, string>();
			__persistentPathCache = new Dictionary<string, bool>();
			__detailGoChildsCount = new Dictionary<string, Dictionary<string, int>>();

			var go = GameObject.Find("GameObjectCacheRoot");
			if (go == null)
			{
				go = new GameObject("GameObjectCacheRoot");
			}
			GameObject.DontDestroyOnLoad(go);
			__cacheTransRoot = go.transform;

			__goPool.SetPopCallback((path, pooledGo) =>
			{
				__ReleaseAsset(path);
			});
			__goPool.SetCheckCanPopCallback((path, pooledGo) =>
			{
				var cnt = __goInstCountCache[path] - (__instCache.ContainsKey(path) ? __instCache[path].Count : 0);
				if (cnt > 0)
					Log.Info(string.Format("path={0} __goInstCountCache={1} __instCache={2}", path, __goInstCountCache[path], (__instCache[path] != null ? __instCache[path].Count : 0)));
				return cnt == 0 && !__persistentPathCache.ContainsKey(path);
			});

		}

		// 初始化inst
		void InitInst(GameObject inst)
		{
			if (inst != null)
			{
				inst.SetActive(true);
			}
		}

		// 检测是否已经被缓存
		public bool CheckHasCached(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				Log.Error("path err :\"" + path + "\"");
				return false;
			}
			if (!path.EndsWith(".prefab"))
			{
				Log.Error("GameObject must be prefab : \"" + path + "\"");
				return false;
			}

			if (__instCache.ContainsKey(path) && __instCache[path].Count > 0)
			{
				return true;
			}
			return __goPool.ContainsKey(path);
		}

		//缓存并实例化GameObject
		void CacheAndInstGameObject(string path, GameObject go, int inst_count)
		{
			__goPool.Set(path, go);
			__InitGoChildCount(path, go);
			if (inst_count > 0)
			{
				List<GameObject> cachedInst;
				if (!__instCache.TryGetValue(path, out cachedInst))
					cachedInst = new List<GameObject>();
				for (int i = 0; i < inst_count; i++)
				{
					var inst = GameObject.Instantiate(go);
					inst.transform.SetParent(__cacheTransRoot);
					inst.SetActive(false);
					cachedInst.Add(inst);
					__instPathCache[inst] = path;
				}
				__instCache[path] = cachedInst;
				if (!__goInstCountCache.ContainsKey(path)) __goInstCountCache[path] = 0;
				__goInstCountCache[path] = __goInstCountCache[path] + inst_count;
			}
		}
		//预加载一系列资源
		public async ETTask LoadDependency(List<string> res)
		{
			if (res.Count <= 0) return;
			using (ListComponent<ETTask> TaskScheduler = ListComponent<ETTask>.Create())
			{
				for (int i = 0; i < res.Count; i++)
				{
					TaskScheduler.Add(PreLoadGameObjectAsync(res[i], 1));
				}
				await ETTaskHelper.WaitAll(TaskScheduler);
			}
		}
		//尝试从缓存中获取
		public bool TryGetFromCache(string path, out GameObject go)
		{
			go = null;
			if (!CheckHasCached(path)) return false;
			if (__instCache.TryGetValue(path, out var cachedInst))
			{
				if (cachedInst.Count > 0)
				{
					var inst = cachedInst[cachedInst.Count - 1];
					cachedInst.RemoveAt(cachedInst.Count - 1);
					go = inst;
					if (inst == null)
					{
						Log.Error("Something wrong, there gameObject instance in cache is null!");
						return false;
					}
					return true;
				}
			}
			if (__goPool.TryGet(path, out var pooledGo))
			{
				if (pooledGo != null)
				{
					var inst = GameObject.Instantiate(pooledGo);
					__goInstCountCache[path] = __goInstCountCache[path] + 1;
					__instPathCache[inst] = path;
					go = inst;
					return true;
				}
			}
			return false;
		}

		//预加载：可提供初始实例化个数
		public async ETTask PreLoadGameObjectAsync(string path, int inst_count,Action callback = null)
		{
			CoroutineLock coroutineLock = null;
			try
			{
				coroutineLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Resources, path.GetHashCode());
				if (CheckHasCached(path))
				{
					callback?.Invoke();
				}
				else
				{
					var go = await ResourcesComponent.Instance.LoadAsync<GameObject>(path);
					if (go != null)
					{
						CacheAndInstGameObject(path, go as GameObject, inst_count);
					}
					callback?.Invoke();
				}
			}
			finally
			{
				coroutineLock?.Dispose();
			}
		}


		//异步获取：必要时加载
		public async ETTask<GameObject> GetGameObjectAsync(string path,Action<GameObject> callback = null)
		{
			if (TryGetFromCache(path, out var inst))
			{
				InitInst(inst);
				callback?.Invoke(inst);
				return inst;
			}
			await PreLoadGameObjectAsync(path, 1);
			if (TryGetFromCache(path, out inst))
			{
				InitInst(inst);
				callback?.Invoke(inst);
				return inst;
			}
			callback?.Invoke(null);
			return null;
		}

		public GameObject GetGameObject(string path)
		{
			if (TryGetFromCache(path, out var inst))
			{
				InitInst(inst);
				return inst;
			}
			return null;
		}

		//回收
		public void RecycleGameObject(GameObject inst, bool isclear = false)
		{
			if (!__instPathCache.ContainsKey(inst))
			{
				Log.Error("RecycleGameObject inst not found from __instPathCache");
				return;
			}
			var path = __instPathCache[inst];
			if (!isclear)
			{
				__CheckRecycleInstIsDirty(path, inst, null);
				inst.transform.SetParent(__cacheTransRoot, false);
				inst.SetActive(false);
				if (!__instCache.ContainsKey(path))
				{
					__instCache[path] = new List<GameObject>();
				}
				__instCache[path].Add(inst);
			}
			else
			{
				DestroyGameObject(inst);
			}


		}
		//检测回收的时候是否需要清理资源(这里是检测是否清空 inst和缓存的go)
		//这里可以考虑加一个配置表来处理优先级问题，一些优先级较高的保留
		public void CheckCleanRes(string path)
		{
			var cnt = __goInstCountCache[path] - (__instCache.ContainsKey(path) ? __instCache[path].Count : 0);
			if (cnt == 0 && !__persistentPathCache.ContainsKey(path))
				__ReleaseAsset(path);
		}


		//删除gameobject 所有从GameObjectPool中
		void DestroyGameObject(GameObject inst)
		{
			if (__instPathCache.TryGetValue(inst, out string path))
			{
				if (__goInstCountCache.TryGetValue(path, out int count))
				{
					if (count <= 0)
					{
						Log.Error("__goInstCountCache[path] must > 0");
					}
					else
					{
						__CheckRecycleInstIsDirty(path, inst, () =>
						{
							GameObject.Destroy(inst);
							__goInstCountCache[path] = __goInstCountCache[path] - 1;
						});
					}
				}
			}
			else
			{
				Log.Error("DestroyGameObject inst not found from __instPathCache");
			}
		}
		//添加需要持久化的资源
		public void AddPersistentPrefabPath(string path)
		{
			__persistentPathCache[path] = true;

		}
		void __CheckRecycleInstIsDirty(string path, GameObject inst, Action callback)
		{
			if (!__IsOpenCheck())
			{
				callback?.Invoke();
				return;
			}
			inst.SetActive(false);
			__CheckAfter(path, inst).Coroutine();
			callback?.Invoke();
		}

		async ETTask __CheckAfter(string path, GameObject inst)
		{
			await TimerComponent.Instance.WaitAsync(2000);
			if (inst != null && inst.transform != null && __CheckInstIsInPool(path, inst))
			{
				var go_child_count = __goChildsCountPool[path];
				Dictionary<string, int> childsCountMap = new Dictionary<string, int>();
				int inst_child_count = RecursiveGetChildCount(inst.transform, "", ref childsCountMap);
				if (go_child_count != inst_child_count)
				{
					Log.Error($"go_child_count({ go_child_count }) must equip inst_child_count({inst_child_count}) path = {path} ");
					foreach (var item in childsCountMap)
					{
						var k = item.Key;
						var v = item.Value;
						var unfair = false;
						if (!__detailGoChildsCount[path].ContainsKey(k))
							unfair = true;
						else if (__detailGoChildsCount[path][k] != v)
							unfair = true;
						if (unfair)
							Log.Error($"not match path on checkrecycle = { k}, count = {v}");
					}
				}
			}
		}

		bool __CheckInstIsInPool(string path, GameObject inst)
		{
			if (__instCache.TryGetValue(path, out var inst_array))
			{
				for (int i = 0; i < inst_array.Count; i++)
				{
					if (inst_array[i] == inst) return true;
				}
			}
			return false;
		}
		void __InitGoChildCount(string path, GameObject go)
		{
			if (!__IsOpenCheck()) return;
			if (!__goChildsCountPool.ContainsKey(path))
			{
				Dictionary<string, int> childsCountMap = new Dictionary<string, int>();
				int total_child_count = RecursiveGetChildCount(go.transform, "", ref childsCountMap);
				__goChildsCountPool[path] = total_child_count;
				__detailGoChildsCount[path] = childsCountMap;
			}
		}

		// 释放资源
		void __ReleaseAsset(string path)
		{
			if (__instCache.ContainsKey(path))
			{
				for (int i = __instCache[path].Count - 1; i >= 0; i--)
				{
					__instPathCache.Remove(__instCache[path][i]);
					GameObject.Destroy(__instCache[path][i]);
					__instCache[path].RemoveAt(i);
				}
				__instCache.Remove(path);
				__goInstCountCache.Remove(path);
			}
			if (__goPool.TryOnlyGet(path, out var pooledGo) && __CheckNeedUnload(path))
			{
				AddressablesManager.ReleaseAsset(pooledGo);
				__goPool.Remove(path);
			}
		}
		bool __IsOpenCheck()
		{
			return Define.Debug;
		}

		public int RecursiveGetChildCount(Transform trans, string path, ref Dictionary<string, int> record)
		{
			int total_child_count = trans.childCount;
			for (int i = 0; i < trans.childCount; i++)
			{
				var child = trans.GetChild(i);
				if (child.name.Contains("Input Caret") || child.name.Contains("TMP SubMeshUI") || child.name.Contains("TMP UI SubObject") || /*child.GetComponent<LoopListViewItem2>()!=null
					 || child.GetComponent<LoopGridViewItem>() != null ||*/ (child.name.Contains("Caret") && child.parent.name.Contains("Text Area")))
				{
					//Input控件在运行时会自动生成个光标子控件，而prefab中是没有的，所以得过滤掉
					//TextMesh会生成相应字体子控件
					//TextMeshInput控件在运行时会自动生成个光标子控件，而prefab中是没有的，所以得过滤掉
					total_child_count = total_child_count - 1;
				}
				else
				{
					string cpath = path + "/" + child.name;
					if (record.ContainsKey(cpath))
					{
						record[cpath] += 1;
					}
					else
					{
						record[cpath] = 1;
					}
					total_child_count += RecursiveGetChildCount(child, cpath, ref record);
				}
			}
			return total_child_count;
		}
		//清理缓存
		public void Cleanup(bool includePooledGo = true, List<string> excludePathArray = null)
		{
			Log.Info("GameObjectPool Cleanup ");
			foreach (var item in __instCache)
			{
				for (int i = 0; i < item.Value.Count; i++)
				{
					var inst = item.Value[i];
					if (inst != null)
					{
						GameObject.Destroy(inst);
						__goInstCountCache[item.Key]--;
					}
					__instPathCache.Remove(inst);
				}
			}
			__instCache = new Dictionary<string, List<GameObject>>();

			if (includePooledGo)
			{
				Dictionary<string, bool> dict_excludepath = null;
				if (excludePathArray != null)
				{
					dict_excludepath = new Dictionary<string, bool>();
					for (int i = 0; i < excludePathArray.Count; i++)
					{
						dict_excludepath[excludePathArray[i]] = true;
					}
				}

				List<string> keys = __goPool.Keys.ToList();
				for (int i = keys.Count - 1; i >= 0; i--)
				{
					var path = keys[i];
					if (dict_excludepath != null && !dict_excludepath.ContainsKey(path) && __goPool.TryOnlyGet(path, out var pooledGo))
					{
						if (pooledGo != null && __CheckNeedUnload(path))
						{
							AddressablesManager.ReleaseAsset(pooledGo);
							__goPool.Remove(path);
						}
					}
				}
			}
			Log.Info("GameObjectPool Cleanup Over");
		}
		//--释放asset
		//--注意这里需要保证外面没有引用这些path的inst了，不然会出现材质丢失的问题
		//--不要轻易调用，除非你对内部的资源的生命周期有了清晰的了解
		//--@param includePooledGo: 是否需要将预设也释放
		//--@param patharray： 需要释放的资源路径数组
		public void CleanupWithPathArray(bool includePooledGo = true, List<string> patharray = null)
		{
			Debug.Log("GameObjectPool Cleanup ");
			Dictionary<string, bool> dict_path = null;
			if (patharray != null)
			{
				dict_path = new Dictionary<string, bool>();
				for (int i = 0; i < patharray.Count; i++)
				{
					dict_path[patharray[i]] = true;
				}
			}
			foreach (var item in __instCache)
			{
				if (dict_path.ContainsKey(item.Key))
				{
					for (int i = 0; i < item.Value.Count; i++)
					{
						var inst = item.Value[i];
						if (inst != null)
						{
							GameObject.Destroy(inst);
							__goInstCountCache[item.Key]-- ;
						}
						__instPathCache.Remove(inst);
					}
				}
			}
			for (int i = 0; i < patharray.Count; i++)
			{
				__instCache.Remove(patharray[i]);
			}

			if (includePooledGo)
			{
				List<string> keys = __goPool.Keys.ToList();
				for (int i = keys.Count - 1; i >= 0; i--)
				{
					var path = keys[i];
					if (patharray != null && dict_path.ContainsKey(path) && __goPool.TryOnlyGet(path, out var pooledGo))
					{
						if (pooledGo != null && __CheckNeedUnload(path))
						{
							AddressablesManager.ReleaseAsset(pooledGo);
							__goPool.Remove(path);
						}
					}
				}
			}
		}
		/// <summary>
		/// 检查指定路径是否有未回收的预制体
		/// </summary>
		/// <param name="path"></param>
		private bool __CheckNeedUnload(string path)
		{
			return !__instPathCache.ContainsValue(path);
		}
		public GameObject GetCachedGoWithPath(string path)
		{
			if (__goPool.TryOnlyGet(path, out var res))
			{
				return res;
			}
			return null;
		}
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			Cleanup();
			base.Dispose();

			Instance = null;
		}
	}
}
