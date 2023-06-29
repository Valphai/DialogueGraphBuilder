using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Edit.Graph.Utilities
{
    public abstract class ConstantPortType<T> : IConstantPortView
    {
        protected ConstantViewGeneric<T> constantViewGeneric;

        public Port ReferencePort { get; protected set; }
        public ConstantPortInput ConstantPortInput { get; protected set; }
        public T Value { get; protected set; }
        public abstract Type PortType { get; }

        protected abstract ConstantPortInput CreateConstantPortInput();

        public ConstantPortInput GetConstantPortInput()
        {
            return ConstantPortInput ?? CreateConstantPortInput();
        }

        public virtual string Save()
        {
            return Value.ToString();
        }

        public virtual void Load(string saveData)
        {
            Value = (T)Convert.ChangeType(saveData, typeof(T));
            UpdateControl(Value);
        }

        public void BindToPort(Port referencePort)
        {
            ReferencePort = referencePort;
        }
        
        public void SetDefaultValue()
        {
            Value = default;
            UpdateControl(Value);
        }

        public void UpdateControl(T value)
        {
            GetConstantPortInput();
            constantViewGeneric.UpdateControl(value);
        }
    }
}