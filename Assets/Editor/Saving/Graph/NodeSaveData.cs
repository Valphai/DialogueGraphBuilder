using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chocolate4.Saving
{
    [Serializable]
    public class NodeSaveData
    {
        public List<string> inputIDs;
        public string nodeID;
        public string nodeType;
        public string text;
        public Vector2 position;
        public string groupID;

        public NodeSaveData(BaseNode node)
        {
            inputIDs = node.InputIDs;
            nodeID = node.ID;
            nodeType = node.NodeType.ToString();
            text = node.Text;
            position = node.GetPosition().position;
            groupID = node.GroupID;
        }
    }
}
