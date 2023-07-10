using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Entities.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Collections.Generic;
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
        
        public static Foldout WithFoldout(this VisualElement element, string text)
        {
            Foldout foldout = new Foldout() { text = text };

            element.Add(foldout);
            return foldout;
        }

        public static VisualElement WithPropertyStyle(this VisualElement element)
        {
            element.WithBackgroundColor(UIStyles.PropertyColor);
            element.WithBaseNodeStyle();
            return element;
        }
        
        public static VisualElement WithEventStyle(this VisualElement element)
        {
            element.WithBackgroundColor(UIStyles.EventColor);
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

        public static VisualElement WithVerticalDividerStyle(this VisualElement element, Color backgroundColor)
        {
            element.WithWidth(.01f);
            element.style.borderRightWidth = 1f;
            element.WithBackgroundColor(backgroundColor);
            element.style.borderBottomColor = element.style.borderRightColor = 
                element.style.borderTopColor = element.style.borderLeftColor = backgroundColor;

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
        
        public static VisualElement WithMarginRight(this VisualElement element, float margin)
        {
            element.style.marginRight = margin;
            return element;
        }
        
        public static VisualElement WithMarginBot(this VisualElement element, float margin)
        {
            element.style.marginBottom = margin;
            return element;
        }
        
        public static VisualElement WithMargin(this VisualElement element, float margin)
        {
            element
                .WithMarginBot(margin)
                .WithMarginTop(margin)
                .WithMarginRight(margin)
                .WithMarginLeft(margin);
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
        
        public static VisualElement WithFlexShrink(this VisualElement element, float shrink)
        {
            element.style.flexShrink = shrink;
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

        public static void Rename(
            Label renamableLabel, string[] existingNames, Action<string> onFinishText=null
        )
        {
            renamableLabel.text = string.Empty;

            TextField textField = new TextField();
            renamableLabel.Add(textField);

            textField.Focus();

            textField.RegisterCallback<FocusOutEvent>(evt => {
                renamableLabel.text = ObjectNames.GetUniqueName(existingNames, textField.text);

                onFinishText?.Invoke(renamableLabel.text);
                renamableLabel.Remove(textField);
            });
        }

        public static TextField WithTextField(
            this VisualElement element, string startText,
            Action<ChangeEvent<string>> onInputChanged, bool multiline = true
        )
        {
            TextField textField = new TextField()
            {
                value = startText,
                multiline = multiline,

            };
            textField.RegisterValueChangedCallback(evt => onInputChanged?.Invoke(evt));

            element.Add(textField);
            return textField;
        }
        public static TextField WithNodeTextField(
            this VisualElement element, string startText, 
            Action<ChangeEvent<string>> onInputChanged, bool multiline = true
        )
        {
            TextField textField = element.WithTextField(startText, onInputChanged, multiline);

            textField.WithVerticalGrow()
                .WithFlexGrow();

            textField.Q<TextElement>()
                .WithMaxWidth(UIStyles.MaxWidth)
                .WithExpandableHeight();


            return textField;
        }

        public static (Label, Image) MakeHeaderWithEntity(
            VisualElement parentContainer, List<VisualElement> row1Children
        )
        {
            VisualElement titleRowsContainer = new VisualElement()
                            .WithVerticalGrow()
                            .WithFlexGrow();

            VisualElement row1 = new VisualElement()
                .WithHorizontalGrow()
                .WithFlexGrow();
            VisualElement row2 = new VisualElement()
                .WithHorizontalGrow()
                .WithBackgroundColor(UIStyles.DefaultDarker0Color)
                .WithFlexGrow();
            row2.style.justifyContent = Justify.Center;
            row2.style.borderBottomRightRadius = 3f;
            row2.style.borderTopRightRadius = 3f;
            row2.style.borderRightColor = UIStyles.DefaultDarker2Color;
            row2.style.borderTopColor = UIStyles.DefaultDarker2Color;
            row2.style.borderRightWidth = .03f;
            row2.style.borderTopWidth = .01f;


            parentContainer.Add(titleRowsContainer);
            titleRowsContainer.Add(row1);
            titleRowsContainer.Add(row2);

            row1Children.ForEach(child => row1.Add(child));
            row1.Q<Label>().WithFontSize(20f);

            Label entityLabel = new Label();
            entityLabel.style.alignSelf = Align.Center;
            entityLabel.WithFontSize(20f);
            row2.Add(entityLabel);

            VisualElement divider = new VisualElement()
                .WithVerticalDividerStyle(UIStyles.StoryDarkerColor);

            parentContainer.Add(divider);

            VisualElement imageBackground = new VisualElement()
                .WithBackgroundColor(UIStyles.StoryLighterColor)
                .WithHorizontalGrow()
                .WithFlexShrink(0f);
            imageBackground.style.alignItems = Align.Center;

            Image entityPortrait = new Image();
            entityPortrait
                .WithWidth(GraphConstants.NodePortraitWidth - UIStyles.PaddingSmall)
                .WithHeight(GraphConstants.NodePortraitHeight - UIStyles.PaddingSmall)
                .WithFlexShrink(0f);

            imageBackground.Add(entityPortrait);

            parentContainer
                .WithHeight(GraphConstants.NodePortraitHeight + UIStyles.PaddingSmall);
            parentContainer.Add(imageBackground);

            return (entityLabel, entityPortrait);
        }
    }
}
