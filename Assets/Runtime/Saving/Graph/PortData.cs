using System;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class PortData
    {
        public string otherNodeID;
        public string otherPortName;
        public string thisPortName;
        public string thisPortType;

        public PortData()
        {
            
        }

        public PortData(PortData portData)
        {
            otherNodeID = portData.otherNodeID;
            otherPortName = portData.otherPortName;
            thisPortName = portData.thisPortName;
            thisPortType = portData.thisPortType;
        }
    }
}
