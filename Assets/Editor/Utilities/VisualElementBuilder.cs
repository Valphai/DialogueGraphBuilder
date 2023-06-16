using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Utilities
{
    public static class VisualElementBuilder
    {
        public static IManipulator CreateContextualMenuManipulator(string actionEntryTitle, Action<DropdownMenuAction> actionEvent)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuBuilder => menuBuilder.menu.AppendAction(actionEntryTitle, actionEvent)
            );

            return contextualMenuManipulator;
        }

        public static VisualElement AddContextualMenu(this VisualElement element, string actionEntryTitle, Action<DropdownMenuAction> action)
        {
            VisualElementExtensions.AddManipulator(element,
                CreateContextualMenuManipulator(actionEntryTitle, action)
            );

            return element;
        }

        public static Button WithOnClick(this Button button, Action clicked)
        {
            button.clicked += clicked;
            return button;
        }

        public static Button WithButton(this VisualElement element, string text)
        {
            Button button = new Button() { text = text };

            element.Add(button);
            return button;
        }

        public static VisualElement WithPropertyStyle(this VisualElement element)
        {
            element.style.backgroundColor = UIStyles.propertyColor;
            element.WithBaseNodeStyle();
            return element;
        }

        public static VisualElement WithLogicStyle(this VisualElement element)
        {
            element.style.backgroundColor = UIStyles.logicColor;
            element.style.paddingLeft = UIStyles.PaddingMedium;
            element.WithBaseNodeStyle();
            return element;
        }
        
        public static VisualElement WithStoryStyle(this VisualElement element)
        {
            element.style.backgroundColor = UIStyles.storyColor;
            element.WithBaseNodeStyle();
            return element;
        }
        
        public static VisualElement WithBaseNodeStyle(this VisualElement element)
        {
            element.WithMaxWidth(UIStyles.MaxWidth);
            return element;
        }
        
        public static VisualElement WithFontSize(this VisualElement element, float size)
        {
            element.style.fontSize = size;
            return element;
        }
        
        public static VisualElement WithMarginTop(this VisualElement element, float margin)
        {
            element.style.marginTop = margin;
            return element;
        }

        public static VisualElement WithHorizontalGrow(this VisualElement element)
        {
            element.style.flexDirection = FlexDirection.Row;
            return element;
        }
        
        public static VisualElement WithVerticalGrow(this VisualElement element)
        {
            element.style.flexDirection = FlexDirection.Column;
            return element;
        }
        
        public static VisualElement WithStretchToParentHeight(this VisualElement element)
        {
            element.style.flexGrow = 1;
            return element;
        }
        
        public static VisualElement WithMinHeight(this VisualElement element, float minHeight)
        {
            element.style.minHeight = minHeight;
            return element;
        }

        public static VisualElement WithMaxWidth(this VisualElement element, float maxWidth)
        {
            element.style.maxWidth = maxWidth;
            return element;
        }

        public static VisualElement WithExpandableHeight(this VisualElement element)
        {
            element.style.whiteSpace = WhiteSpace.Normal;
            return element;
        }

        public static void Rename(Label renamableLabel, DialogueTreeView treeView, Action<string> onFinishText=null)
        {
            renamableLabel.text = string.Empty;

            TextField textField = new TextField();
            renamableLabel.Add(textField);

            textField.Focus();

            textField.RegisterCallback<FocusOutEvent>(evt => {

                string[] existingNames = treeView.DialogueTreeItems.Select(item => item.displayName).ToArray();
                renamableLabel.text = ObjectNames.GetUniqueName(existingNames, textField.text);

                onFinishText?.Invoke(renamableLabel.text);
                renamableLabel.Remove(textField);
            });
        }
    }
}
