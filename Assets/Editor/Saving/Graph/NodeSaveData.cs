using System.Collections.Generic;

namespace Chocolate4.Saving
{
    [System.Serializable]
    public class NodeSaveData
    {
        public BaseNode parentNode;
        public List<BaseNode> parentInputs;

        public NodeSaveData(BaseNode parentNode, List<BaseNode> parentInputs)
        {
            this.parentNode = parentNode;
            this.parentInputs = parentInputs;
        }
    }
}
