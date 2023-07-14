using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities
{
    public static class GraphUtilities
    {
        public static void PerformOnGraphElementsOfType<T>(
            IEnumerable<GraphElement> graphElements, Action<T> onElementFound
        ) where T : GraphElement
        {
            List<GraphElement> elements = graphElements.ToList();
            elements.ForEach(graphElement => {
                if (graphElement is not T element)
                {
                    return;
                }

                onElementFound?.Invoke(element);
            });
        }

        public static void GeneratePastedIds(List<GroupSaveData> groupSaveData, List<IDataHolder> cache)
        {
            foreach (IDataHolder dataHolder in cache)
            {
                string newId = Guid.NewGuid().ToString();
                string oldId = dataHolder.NodeData.nodeId;

                dataHolder.NodeData.nodeId = newId;

                List<IDataHolder> otherDataHolders = cache.Where(data => data != dataHolder).ToList();
                foreach (IDataHolder otherHolder in otherDataHolders)
                {
                    ReplaceOldIds(newId, oldId, otherHolder.NodeData.inputPortDataCollection);
                    ReplaceOldIds(newId, oldId, otherHolder.NodeData.outputPortDataCollection);
                }

                foreach (GroupSaveData groupData in groupSaveData)
                {
                    if (dataHolder.NodeData.groupId.Equals(groupData.id))
                    {
                        string newGroupId = Guid.NewGuid().ToString();
                        dataHolder.NodeData.groupId = groupData.id = newGroupId;
                    }
                }
            }

            void ReplaceOldIds(string newId, string oldId, List<PortData> otherCollection)
            {
                foreach (PortData portData in otherCollection)
                {
                    if (portData.otherNodeID.Equals(oldId))
                    {
                        portData.otherNodeID = newId;
                    }
                }
            }
        }
    }
}