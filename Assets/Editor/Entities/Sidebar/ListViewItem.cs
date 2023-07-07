using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Edit.Entities.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Edit.Entities.Sidebar
{
    public class ListViewItem : VisualElement
    {
        private VisualElement iconElement; 
        private Label label;

        public void Initialize(string name, Texture2D icon)
        {
            Clear();

            this
                .WithHeight(EntitiesConstants.ListViewItemHeight)
                .WithHorizontalGrow()
                .WithMarginLeft(EntitiesConstants.MarginSmall);
            style.alignItems = Align.Center;

            iconElement = new VisualElement()
                .WithWidth(EntitiesConstants.IconWidth)
                .WithHeight(EntitiesConstants.IconHeight);
            
            iconElement.style.backgroundImage = icon;

            label = new Label() { text = name };
            label.WithFlexBasis(1f);

            Add(iconElement);
            Add(label);
        }
    }
}