using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using System;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Edit.Graph.Utilities
{
    public interface IConstantPortView : ISaveable<string>
    {
        Port ReferencePort { get; }
        Type PortType { get; }

        ConstantPortInput GetConstantPortInput();
        void SetDefaultValue();
        void BindToPort(Port referencePort);
    }
}