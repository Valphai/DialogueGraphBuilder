using Chocolate4.Dialogue.Runtime.Saving;
using System.Linq;

namespace Chocolate4.Runtime.Utilities
{
    public static class NodeUtilities
    {
        public static string GetNodeType(this NodeSaveData nodeSaveData)
        {
            return nodeSaveData.nodeType.Split('.').Last();
        }
        
        public static bool IsNodeOfType(this NodeSaveData nodeSaveData, string type)
        {
            return nodeSaveData.GetNodeType().Equals(type);
        }
        
        public static bool IsNodeOfType(this NodeSaveData nodeSaveData, params string[] types)
        {
            return types.Any(type => nodeSaveData.IsNodeOfType(type));
        }
    }
}