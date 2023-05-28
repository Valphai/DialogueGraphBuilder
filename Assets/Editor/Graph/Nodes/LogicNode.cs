using Chocolate4.Editor.Graph.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Chocolate4
{
    public class LogicNode : BaseNode
    {
        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);

            NodeType = NodeTypes.LogicNode;

            Choices.Add("True");
            Choices.Add("False");
        }

        public override void DrawPorts()
        {
            DrawInputPort();

            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                choicePort.portName = choice;
                outputContainer.Add(choicePort);
            }
        }
    }
}
