using System;
using System.Collections.Generic;
namespace ET
{
    public class AssetsScene
    {
        public string Name;
        public List<AssetsObject> Objects;
        public Dictionary<long, List<int>> CellMapObjects;
    }
}