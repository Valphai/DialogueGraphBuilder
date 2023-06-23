using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class NodeSaveData : IDataHolder
    {
        public string nodeId;
        public string nodeType;
        public Vector2 position;
        public string groupId;
        public List<PortData> inputPortDataCollection;
        public List<PortData> outputPortDataCollection;

        public NodeSaveData NodeData => this;
    }
}
