using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class ConstantViewGeneric<T> : VisualElement, IConstantViewControlCreator
    {
        [SerializeField]
        protected PropertyNode<T> propertyNode;

        public ConstantViewGeneric(PropertyNode<T> propertyNode)
        {
            this.propertyNode = propertyNode;
        }

        public abstract VisualElement CreateControl();
        public abstract void UpdateControl(T value);
    }
}