using System;
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

        public static VisualElement WithStyle(this VisualElement element, string style)
        {
            element.AddToClassList(style);
            return element;
        }

        public static void Rename(Label renamableLabel, Action<string> onFinishText=null)
        {
            renamableLabel.text = string.Empty;

            TextField textField = new TextField();
            renamableLabel.Add(textField);

            textField.Focus();

            textField.RegisterCallback<FocusOutEvent>(evt => {
                renamableLabel.text = textField.text;
                onFinishText?.Invoke(textField.text);
                renamableLabel.Remove(textField);
            });
        }
    }
}
