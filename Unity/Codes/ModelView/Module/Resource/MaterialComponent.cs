using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class MaterialComponentAwakeSystem : AwakeSystem<MaterialComponent>
    {
        public override void Awake(MaterialComponent self)
        {
            self.Awake();
        }
    }
    public class MaterialComponent:Entity
    {
        public static MaterialComponent Instance { get; set; }
        Dictionary<string, Material> m_cacheMaterial;
        public void Awake()
        {
            Instance = this;
            m_cacheMaterial = new Dictionary<string, Material>();
        }

        public async ETTask<Material> LoadMaterialAsync(string address,Action<Material> callback = null)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Resources, address.GetHashCode()))//协程锁防重入
            {
                if (m_cacheMaterial.TryGetValue(address, out var res))
                {
                    res = await ResourcesComponent.Instance.LoadAsync<Material>(address);
                    if (res != null)
                        m_cacheMaterial[address] = res;
                }
                callback?.Invoke(res);
                return res;
            }
        }
    }
}
