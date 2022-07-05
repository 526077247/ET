using System;
using System.Collections.Generic;
namespace ET
{
    internal class AssetsScene
    {
        public string Name;
        public List<AssetsObject> Objects;
        public Dictionary<long, List<int>> CellMapObjects;
    }
}