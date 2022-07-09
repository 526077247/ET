using System;
using System.Collections.Generic;
using ProtoBuf;
namespace ET
{
    [ProtoContract]
    public class AssetsScene
    {
        [ProtoContract]
        public class IntList
        {
            [ProtoMember(1)]
            public List<int> Value;
        }
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public int CellLen;
        [ProtoMember(3)]
        public List<AssetsObject> Objects;

        [ProtoMember(4)]
        public List<long> CellIds;
        [ProtoMember(5)]
        public List<IntList> MapObjects;

        [ProtoIgnore]
        public Dictionary<long, List<int>> cellMapObjects;
        [ProtoIgnore]
        public Dictionary<long, List<int>> CellMapObjects
        {
            get
            {
                if (cellMapObjects == null)
                {
                    cellMapObjects = new Dictionary<long, List<int>>();
                    for (int i = 0; i < CellIds.Count; i++)
                    {
                        cellMapObjects.Add(CellIds[i],MapObjects[i].Value);
                    }
                }
                return cellMapObjects;
            }
        }

    }
}