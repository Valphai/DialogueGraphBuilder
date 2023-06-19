using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class NodeSaveData : IDataHolder
    {
        public string nodeID;
        public string nodeType;
        public Vector2 position;
        public string groupID;
        public List<PortData> outputPortDatas;

        public NodeSaveData NodeData => this;
    }
}
