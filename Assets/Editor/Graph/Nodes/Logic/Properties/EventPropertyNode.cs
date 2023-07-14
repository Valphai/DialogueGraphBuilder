using Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Dialogue.Edit.Graph.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class EventPropertyNode : BlackboardPropertyNode, IDangerCauser
    {
        public override string Name { get; set; } = "Event Node";
        public override PropertyType PropertyType { get; protected set; } = PropertyType.Event;

        protected override void DrawTitle()
        {
            Label label = UpdateLabel();
            label.WithFontSize(UIStyles.FontSize)
                .WithMaxWidth(UIStyles.MaxWidth);

            label.style.unityTextAlign = TextAnchor.MiddleCenter;

            titleContainer.WithEventStyle();
        }
    }
}