using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class IntegerSlotControlView : ConstantViewGeneric<int>
    {
        [SerializeField]
        private TextField textField;

        public IntegerSlotControlView(PropertyNode<int> propertyNode) : base(propertyNode) { }

        public override VisualElement CreateControl()
        {
            textField = new TextField();
            textField.RegisterValueChangedCallback(OnChangeTextField);
            Add(textField);

            return this;
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