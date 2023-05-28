using Chocolate4.Editor.Graph.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace Chocolate4.Saving
{
    [System.Serializable]
    public class NodeSaveData
    {
        public BaseNode parentNode;
        public List<BaseNode> parentInputs;

        public string nodeID;
        public NodeTypes nodeType;
        public string text;
        public Vector3 position;
        public string groupID;

        public NodeSaveData(BaseNode parentNode, List<BaseNode> parentInputs)
        {
            this.parentNode = parentNode;
            this.parentInputs = parentInputs;
        }
    }
}
