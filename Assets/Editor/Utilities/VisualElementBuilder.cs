using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Utilities
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

        public static IManipulator AddContextualMenu(this VisualElement element, string actionEntryTitle, Action<DropdownMenuAction> action)
        {
            IManipulator manipulator = CreateContextualMenuManipulator(actionEntryTitle, action);
            element.AddManipulator(manipulator);

            return manipulator;
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
            element.WithBackgroundColor(UIStyles.PropertyColor);
            element.WithBaseNodeStyle();
            return element;
        }

        public static VisualElement WithLogicStyle(this VisualElement element)
        {
            element.WithBackgroundColor(UIStyles.LogicColor);
            element.style.paddingLeft = UIStyles.PaddingMedium;
            element.WithBaseNodeStyle();
            return element;
        }
        
        public static VisualElement WithStoryStyle(this VisualElement element)
        {
            element.WithBackgroundColor(UIStyles.StoryColor);
            element.WithBaseNodeStyle();
            return element;
        }
        
        public static VisualElement WithTransferStyle(this VisualElement element)
        {
            element.WithBackgroundColor(UIStyles.TransferColor);
            element.WithBaseNodeStyle();
            return element;
        }
        
        public static VisualElement WithBaseNodeStyle(this VisualElement element)
        {
            element.WithMaxWidth(UIStyles.MaxWidth);
            return element;
        }

        public static VisualElement WithPortStyle(this VisualElement element, Color backgroundColor, Color borderColor)
        {
            element.style.borderRightColor = element.style.borderLeftColor = element.style.borderBottomColor = element.style.borderTopColor = borderColor;

            const float radius = UIStyles.ConstantPortWidth;

            element.WithBackgroundColor(backgroundColor)
                .WithWidth(radius)
                .WithHeight(radius)
                .WithBorderRadius(radius)
                .WithBorderWidth(1f);

            const float innerRadius = UIStyles.ConstantPortOffset;

            element.style.alignItems = Align.Center;
            element.style.alignSelf = Align.Center;
            element.style.justifyContent = Justify.Center;
            element.style.marginLeft = innerRadius;
            element.style.marginRight = innerRadius;

            VisualElement cap = new VisualElement()
                .WithBackgroundColor(borderColor)
                .WithWidth(innerRadius)
                .WithHeight(innerRadius)
                .WithBorderRadius(innerRadius);

            element.Add(cap);

            return element;
        }
        
        public static VisualElement WithBorderRadius(this VisualElement element, float radius)
        {
            element.style.borderBottomLeftRadius = element.style.borderBottomRightRadius =
                element.style.borderTopLeftRadius =
                element.style.borderTopRightRadius = radius;

            return element;
        }
        
        public static VisualElement WithBorderWidth(this VisualElement element, float width)
        {
            element.style.borderLeftWidth = element.style.borderRightWidth =
                element.style.borderTopWidth =
                element.style.borderBottomWidth = width;

            return element;
        }

        public static VisualElement WithBackgroundColor(this VisualElement element, Color color)
        {
            element.style.backgroundColor = color;
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

        public static VisualElement WithMarginLeft(this VisualElement element, float margin)
        {
            element.style.marginLeft = margin;
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
        
        public static VisualElement WithFlexBasis(this VisualElement element, float value)
        {
            element.style.flexBasis = value;
            return element;
        }

        public static VisualElement WithFlexGrow(this VisualElement element)
        {
            element.style.flexGrow = 1f;
            return element;
        }
        
        public static VisualElement WithFlexShrink(this VisualElement element)
        {
            element.style.flexShrink = 1f;
            return element;
        }
        
        public static VisualElement WithOverflow(this VisualElement element)
        {
            element.style.overflow = Overflow.Visible; 
            return element;
        }

        public static VisualElement WithMinHeight(this VisualElement element, float minHeight)
        {
            element.style.minHeight = minHeight;
            return element;
        }
        
        public static VisualElement WithMinWidth(this VisualElement element, float minWidth)
        {
            element.style.minWidth = minWidth;
            return element;
        }
        
        public static VisualElement WithWidth(this VisualElement element, float width)
        {
            element.style.width = width;
            return element;
        }
        
        public static VisualElement WithHeight(this VisualElement element, float height)
        {
            element.style.height = height;
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
