using Chocolate4.Dialogue.Runtime.Saving;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chocolate4.Runtime.Utilities
{
    public static class NodeUtilities
    {
        public static string GetNodeType(this IDataHolder dataHolder)
        {
            return dataHolder.NodeData.nodeType.Split('.').Last();
        }
        
        public static bool IsNodeOfType(string nodeType, string type)
        {
            return nodeType.Equals(type);
        }

        public static bool IsNodeOfType(this IDataHolder dataHolder, string type)
        {
            string nodeType = dataHolder.GetNodeType();
            return IsNodeOfType(nodeType, type);
        }
        
        public static bool IsNodeOfType(string nodeType, params string[] types)
        {
            return types.Any(type => IsNodeOfType(nodeType, type));
        }

        public static bool IsNodeOfType(this IDataHolder dataHolder, params string[] types)
        {
            string nodeType = dataHolder.GetNodeType();
            return IsNodeOfType(nodeType, types);
        }

        public static List<IDataHolder> GetParents(this IDataHolder dataHolder, Func<string, IDataHolder> findNode)
        {
            List<IDataHolder> parents = new List<IDataHolder>();

            foreach (PortData portData in dataHolder.NodeData.inputPortDataCollection)
            {
                string otherNodeID = portData.otherNodeID;
                if (string.IsNullOrEmpty(otherNodeID))
                {
                    parents.Add(null);
                }

                parents.Add(findNode(otherNodeID));
            }

            return parents;
        }
    }
}