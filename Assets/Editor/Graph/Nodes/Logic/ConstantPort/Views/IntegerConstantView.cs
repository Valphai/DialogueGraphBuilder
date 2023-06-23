using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class IntegerConstantView : ConstantViewGeneric<int>
    {
        [SerializeField]
        private TextField textField;

        public IntegerConstantView(Action<int> onValueChanged) : base(onValueChanged) { }

        public override VisualElement CreateControl()
        {
            textField = new TextField();
            textField.RegisterValueChangedCallback(OnChangeTextField);
            Add(textField);

            return this;
        }

        public override void UpdateControl(int value)
        {
            textField.value = value.ToString();
        }

        private void OnChangeTextField(ChangeEvent<string> evt)
        {
            if (!int.TryParse(evt.newValue, out int value))
            {
                textField.value = "0";
                onValueChanged?.Invoke(0);
                return;
            }

            onValueChanged?.Invoke(value);
        }
    }
}