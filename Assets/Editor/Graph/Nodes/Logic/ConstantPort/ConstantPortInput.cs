using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Utilities;
using System;
using System.Net;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class ConstantPortInput : GraphElement
    {
        public ConstantPortInput(IConstantViewControlCreator constantView)
        {
            pickingMode = PickingMode.Ignore;

            EdgeControl connectingEdge = new EdgeControl
            {
                @from = new Vector2(95f, 11.5f),
                to = new Vector2(95f + 24f, 11.5f),
                edgeWidth = 2,
                inputColor = UIStyles.defaultLightColor,
                outputColor = UIStyles.defaultLightColor,
                pickingMode = PickingMode.Ignore
            };

            Add(connectingEdge);

            VisualElement container = new VisualElement { name = "container" }
                .WithHorizontalGrow()
                .WithBorderRadius(2f)
                .WithMarginTop(-1.5f);

            Label label = new Label() { text = "Value" };
            label.style.unityTextAlign = TextAnchor.MiddleCenter;

            container.Add(label);

            VisualElement containerControl = constantView.CreateControl()
                .WithFlexBasis(50f);

            if (containerControl != null)
            {
                container.Add(containerControl);
            }

            VisualElement connector = new VisualElement().WithPortStyle(UIStyles.defaultDarkerColor, UIStyles.defaultLightColor);

            container.Add(connector);
            Add(container);
        }
    }
}