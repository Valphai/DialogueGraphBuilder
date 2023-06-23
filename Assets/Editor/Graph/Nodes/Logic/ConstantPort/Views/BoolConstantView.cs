using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class BoolConstantView : ConstantViewGeneric<bool>
    {
        [SerializeField]
        private Toggle toggleField;

        public BoolConstantView(PropertyNode<bool> propertyNode) : base(propertyNode) { }

        public override VisualElement CreateControl()
        {
            toggleField = new Toggle();
            toggleField.RegisterValueChangedCallback(OnChangeToggle);
            Add(toggleField);

            return this;
        }

        public override void UpdateControl(bool value)
        {
            toggleField.value = value;
        }

        void OnChangeToggle(ChangeEvent<bool> evt)
        {
            //m_Slot.owner.owner.owner.RegisterCompleteObjectUndo("Toggle Change");
            propertyNode.Value = evt.newValue;
            //m_Slot.owner.Dirty(ModificationScope.Node);
        }
    }
}