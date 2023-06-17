using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class BoolConstantView : ConstantViewGeneric<bool>
    {
        public BoolConstantView(PropertyNode<bool> propertyNode) : base(propertyNode) { }

        public override VisualElement CreateControl()
        {
            Toggle toggleField = new Toggle();
            toggleField.RegisterValueChangedCallback(OnChangeToggle);
            Add(toggleField);

            return this;
        }

        void OnChangeToggle(ChangeEvent<bool> evt)
        {
            //m_Slot.owner.owner.owner.RegisterCompleteObjectUndo("Toggle Change");
            propertyNode.Value = evt.newValue;
            //m_Slot.owner.Dirty(ModificationScope.Node);
        }
    }
}