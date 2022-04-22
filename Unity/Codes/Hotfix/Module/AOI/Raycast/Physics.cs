using UnityEngine;
using System;
namespace ET
{
    [FriendClass(typeof(AOISceneComponent))]
    [FriendClass(typeof(AOIGrid))]
    [FriendClass(typeof(AOITriggerComponent))]
    [FriendClass(typeof(AOIUnitComponent))]
    public static class Physics
    {
        public static bool Raycast(AOISceneComponent scene,Ray ray,out RaycastHit hit,CampType[] type = null)
        {
            hit = default;
            if (type == null) return false;
            using (DictionaryComponent<CampType, bool> typeTemp = DictionaryComponent<CampType, bool>.Create())
            {
                using (HashSetComponent<AOITriggerComponent> temp = HashSetComponent<AOITriggerComponent>.Create())
                {
                    for (int i = 0; i < type.Length; i++)
                    {
                        var item = type[i];
                        typeTemp.Add(item, true);
                    }
                    int xIndex = (int) Math.Floor(ray.Start.x / scene.gridLen);
                    int yIndex = (int) Math.Floor(ray.Start.z / scene.gridLen);
                    //z = kx+b
                    float k = 0;
                    float k_1 = 0;
                    float b = 0;
                    if (ray.Dir.x != 0 && ray.Dir.z != 0)
                    {
                        k = ray.Dir.z / ray.Dir.x;
                        k_1 = ray.Dir.x / ray.Dir.z;
                        b = ray.Start.z - k * ray.Start.x;
                    }

                    Vector3 inPoint = ray.Start;
                    while (true)
                    {
                        long cellId = AOIHelper.CreateCellId(xIndex, yIndex);
                        AOIGrid grid = scene.GetChild<AOIGrid>(cellId);
                        var xMin = xIndex * scene.gridLen;
                        var xMax = xMin + scene.gridLen;
                        var yMin = yIndex * scene.gridLen;
                        var yMax = yMin + scene.gridLen;
                        //Log.Info("Raycast Check "+xIndex+" "+yIndex);
                        if (grid != null)
                        {
                            ListComponent<RaycastHit> hits = ListComponent<RaycastHit>.Create();
                            RaycastHits(ray, grid, inPoint, hits, temp, typeTemp);
                            if (hits.Count > 0)
                            {
                                hits.KSsort((i1,i2)=> i1.Distance >= i2.Distance?1:-1);//从小到大
                                hit = hits[0];
                                //Log.Info("hits.Count > 0"+hit.Trigger.Parent.Parent.Id);
                                hits.Dispose();
                                return true;
                            }
                        }
                        //一般情况
                        if (ray.Dir.x != 0&& ray.Dir.z != 0)
                        {
                            if (ray.Dir.x > 0 && ray.Dir.z > 0)
                            {
                                var z1 = xMax * k + b;
                                if (z1 > yMin && z1 < yMax)
                                {
                                    xIndex++;
                                    inPoint = new Vector3(xMax, inPoint.y+(xMax-inPoint.x)*ray.Dir.y/ray.Dir.x, z1);
                                }
                                else
                                {
                                    yIndex++;
                                    inPoint = new Vector3((yMax - b) * k_1, inPoint.y+(yMax-inPoint.z)*ray.Dir.y/ray.Dir.z, yMax);
                                }
                            }
                            else if (ray.Dir.x > 0 && ray.Dir.z < 0)
                            {
                                var z1 = xMax * k + b;
                                if (z1 > yMin && z1 < yMax)
                                {
                                    xIndex++;
                                    inPoint = new Vector3(xMax, inPoint.y+(xMax-inPoint.x)*ray.Dir.y/ray.Dir.x, z1);
                                }
                                else
                                {
                                    yIndex--;
                                    inPoint = new Vector3((yMin - b) * k_1, inPoint.y+(yMin-inPoint.z)*ray.Dir.y/ray.Dir.z, yMin);
                                }
                            }
                            else if (ray.Dir.x < 0 && ray.Dir.z < 0)
                            {
                                var z1 = xMin * k + b;
                                if (z1 > yMin && z1 < yMax)
                                {
                                    xIndex--;
                                    inPoint = new Vector3(xMin, inPoint.y+(xMin-inPoint.x)*ray.Dir.y/ray.Dir.x, z1);
                                }
                                else
                                {
                                    yIndex--;
                                    inPoint = new Vector3((yMin - b) * k_1, inPoint.y+(yMin-inPoint.z)*ray.Dir.y/ray.Dir.z, yMin);
                                }
                            }
                            else if (ray.Dir.x < 0 && ray.Dir.z > 0)
                            {
                                var z1 = xMin * k + b;
                                if (z1 > yMin && z1 < yMax)
                                {
                                    xIndex--;
                                    inPoint = new Vector3(xMin, inPoint.y+(xMin-inPoint.x)*ray.Dir.y/ray.Dir.x, z1);
                                }
                                else
                                {
                                    yIndex++;
                                    inPoint = new Vector3((yMax - b) * k_1, inPoint.y+(yMax-inPoint.z)*ray.Dir.y/ray.Dir.z, yMax);
                                }
                            }
                            else
                            {
                                Log.Error("What's fuck???");
                            }
                        }
                        //平行于轴了
                        else if (ray.Dir.x == 0&& ray.Dir.z != 0)
                        {
                            if (ray.Dir.z > 0)
                            {
                                yIndex++;
                                inPoint = new Vector3(inPoint.x, inPoint.y+(yMax-inPoint.z)*ray.Dir.y/ray.Dir.z, yMax);
                            }
                            else
                            {
                                yIndex--;
                                inPoint = new Vector3(inPoint.x, inPoint.y+(yMin-inPoint.z)*ray.Dir.y/ray.Dir.z, yMin);
                            }
                        }
                        else if (ray.Dir.z == 0&& ray.Dir.x != 0)
                        {
                            if (ray.Dir.x > 0)
                            {
                                xIndex++;
                                inPoint = new Vector3(xMax, inPoint.y+(xMax-inPoint.x)*ray.Dir.y/ray.Dir.x, inPoint.z);
                            }
                            else
                            {
                                xIndex--;
                                inPoint = new Vector3(xMin, inPoint.y+(xMin-inPoint.x)*ray.Dir.y/ray.Dir.x, inPoint.z);
                            }
                        }
                        //垂直于地图
                        else
                            break;
                        if(Vector3.Distance(inPoint,ray.Start)>ray.Distance)
                            break;
                    }
                }
            }
            return false;
        }

        private static void RaycastHits(Ray ray, AOIGrid grid,Vector3 inPoint,ListComponent<RaycastHit> hits,
            HashSetComponent<AOITriggerComponent> triggers, DictionaryComponent<CampType, bool> type)
        {
            for (int i = 0; i < grid.Triggers.Count; i++)
            {
                var item = grid.Triggers[i];
                if (item.IsCollider &&!triggers.Contains(item)&& type.ContainsKey(CampType.ALL) ||
                    type.ContainsKey(item.GetParent<AOIUnitComponent>().Type))
                {
                    if (item.IsPointInTrigger(inPoint, item.GetRealPos(), item.GetRealRot()))
                    {
                        triggers.Add(item);
                        hits.Add(new RaycastHit
                        {
                            Hit = inPoint,
                            Trigger = item,
                            Distance = Vector3.Distance(inPoint,ray.Start)
                        });
                    }
                    else if (item.IsRayInTrigger(ray,item.GetRealPos(),item.GetRealRot(),out var hit))
                    {
                        triggers.Add(item);
                        hits.Add(new RaycastHit
                        {
                            Hit = hit,
                            Trigger = item,
                            Distance = Vector3.Distance(hit,ray.Start)
                        });
                    }
                }
            }
        }
        
        //todo:
        private static RaycastHit[] RaycastAll(AOISceneComponent scene,Ray ray,CampType[] type = null)
        {
            return null;
        }
    }
}