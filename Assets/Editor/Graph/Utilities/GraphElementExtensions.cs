using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities
{
    public static class GraphElementExtensions
    {
        public static Vector2 GetPositionRaw(this GraphElement graphElement)
        {
            return new Vector2(
                graphElement.style.left.value.value, 
                graphElement.style.top.value.value
            );
        }
    }
}