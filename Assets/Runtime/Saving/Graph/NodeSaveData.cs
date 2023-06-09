using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Saving
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
    }
}
