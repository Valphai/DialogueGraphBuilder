using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Entities.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Edit.Entities
{
    public class EntityView : VisualElement
    {
        private SerializedObject currentEntity;
        private string lastPropPath;

        public EntityView()
        {
            this
                .WithVerticalGrow()
                .WithMarginLeft(EntitiesConstants.MarginBig)
                .WithMarginRight(EntitiesConstants.MarginSmall)
                .WithMarginTop(EntitiesConstants.MarginSmall)
                .WithMarginBot(EntitiesConstants.MarginSmall);
        }

        internal void Display(DialogueEntity entity)
        {
            Clear();

            lastPropPath = string.Empty;
            currentEntity = new SerializedObject(entity);

            SerializedProperty property = currentEntity.GetIterator();

            while (property.Next(enterChildren: true))
            {
                bool alreadyDrawn = property.propertyPath.Contains(lastPropPath);
                if (!string.IsNullOrEmpty(lastPropPath) && alreadyDrawn)
                {
                    continue;
                }

                lastPropPath = property.propertyPath;

                if (lastPropPath.Contains("m_"))
                {
                    continue;
                }

                PropertyField propertyField = new PropertyField(property);
                propertyField.BindProperty(property);

                if (lastPropPath.Contains(nameof(DialogueEntity.entityImage)))
                {
                    VisualElement previewContainer = new VisualElement()
                        .WithHorizontalGrow();

                    propertyField.WithFlexGrow();
                    propertyField.style.alignSelf = Align.FlexEnd;

                    VisualElement previewImage = new VisualElement()
                        .WithWidth(EntitiesConstants.ImageWidth)
                        .WithHeight(EntitiesConstants.ImageHeight)
                        .WithBackgroundColor(UIStyles.DefaultDarkerColor);
                    previewImage.style.backgroundImage = EntitiesUtilities.GetEntityImage(entity);

                    previewContainer.Add(propertyField);
                    previewContainer.Add(previewImage);
                    Add(previewContainer);
                    continue;
                }

                Add(propertyField);
            }
        }
    }
}