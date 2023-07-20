using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Edit.Entities.Utilities;
using Chocolate4.Dialogue.Edit.Graph.Utilities;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Entities.Sidebar
{
    public class ListViewItem : VisualElement
    {
        private Image iconElement; 
        private Label label;

        public void Initialize(DialogueEntity entity)
        {
            Clear();

            this
                .WithHeight(UIStyles.ListViewItemHeight)
                .WithHorizontalGrow()
                .WithMarginLeft(EntitiesConstants.MarginSmall);
            style.alignItems = Align.Center;

            iconElement = new Image();
            iconElement
                .WithWidth(EntitiesConstants.IconWidth)
                .WithHeight(EntitiesConstants.IconHeight);

            iconElement.image = EntitiesUtilities.GetEntityImage(entity);

            label = new Label() { text = entity.entityName };
            label.WithFlexBasis(1f);

            Add(iconElement);
            Add(label);
        }
    }
}