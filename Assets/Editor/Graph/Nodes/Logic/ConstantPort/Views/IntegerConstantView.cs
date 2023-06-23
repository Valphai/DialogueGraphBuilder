using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class IntegerConstantView : ConstantViewGeneric<int>
    {
        [SerializeField]
        private TextField textField;

        public IntegerConstantView(PropertyNode<int> propertyNode) : base(propertyNode) { }

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
                textField.value = string.Empty;
                return;
            }

            propertyNode.Value = value;
        }
    }
}